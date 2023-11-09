using System;
using System.Collections;
using Playdarium.SpreadsheetDownloader.Helpers;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Playdarium.SpreadsheetDownloader
{
	public abstract class SpreadsheetDownloader : Editor, ISpreadsheetLoader
	{
		public event Action<bool> Success;

		private EditorCoroutine _loadRoutine;
		private Action<string> _onComplete;

		protected virtual void OnDisable()
		{
			if (_loadRoutine != null)
				EditorCoroutineUtility.StopCoroutine(_loadRoutine);
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Download"))
				DownloadAndSerialize();

			base.OnInspectorGUI();
		}

		public abstract void DownloadAndSerialize();

		public void InvokeComplete(bool success) => Success?.Invoke(success);

		protected void Load(string sheet, Action<string> onComplete)
		{
			_onComplete = onComplete;
			_loadRoutine = EditorCoroutineUtility.StartCoroutineOwnerless(Connect(sheet));
		}

		private IEnumerator Connect(string sheet)
		{
			var assets = AssetDatabase.FindAssets($"t:{nameof(SpreadsheetSettings)}");
			if (assets.Length == 0)
				throw new Exception("Create SpreadsheetsSettings with name \"SpreadsheetsSettings\"");

			var settings = AssetDatabase.LoadAssetAtPath<SpreadsheetSettings>(AssetDatabase.GUIDToAssetPath(assets[0]));
			var connector = new SpreadsheetConnection(settings);
			var spreadsheet = new Spreadsheet(connector);

			yield return spreadsheet.GetTable(sheet, OnDownload);

			_loadRoutine = null;
		}

		private void OnDownload(string result)
		{
			_onComplete(result);
		}

		protected static T[] Read<T>(string json) => JsonUtility.FromJson<ArrayJson<T>>(json).array;

		[Serializable]
		private class ArrayJson<T>
		{
			public T[] array;
		}
	}
}