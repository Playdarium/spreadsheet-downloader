using System;
using UnityEditor;
using UnityEngine;

namespace Playdarium.SpreadsheetDownloader.Helpers
{
	public abstract class SingleSpreadsheetDownloader : SpreadsheetDownloader
	{
		protected abstract string SpreadsheetName { get; }

		public override void DownloadAndSerialize()
		{
			EditorUtility.DisplayProgressBar("Download", $"Table: {SpreadsheetName}...", 0);
			try
			{
				Load(SpreadsheetName, OnComplete);
			}
			catch (Exception e)
			{
				EditorUtility.ClearProgressBar();
				Debug.LogError($"[SingleSpreadsheetDownloader] <b>{SpreadsheetName}:</b> {e}");
			}
		}

		private void OnComplete(string json)
		{
			EditorUtility.DisplayProgressBar("Download", $"Table: {SpreadsheetName}...", .5f);
			var success = false;
			try
			{
				Serialize(json);
				serializedObject.ApplyModifiedProperties();
				success = true;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			finally
			{
				EditorUtility.ClearProgressBar();
				InvokeComplete(success);
			}
		}

		protected abstract void Serialize(string json);
	}
}