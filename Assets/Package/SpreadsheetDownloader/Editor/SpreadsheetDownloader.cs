using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Package.SpreadsheetDownloader.Helpers;
using UnityEditor;
using UnityEngine;

namespace Package.SpreadsheetDownloader
{
	public abstract class SpreadsheetDownloader : Editor, ISpreadsheetLoader
	{
		private static readonly StringBuilder Builder = new();
		
		public event Action<bool> Success;

		private IEnumerator _enumerator;
		private Action<string> _onComplete;
		
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
			StartCoroutine(Connect(sheet));
		}

		private IEnumerator Connect(string sheet)
		{
			var assets = AssetDatabase.FindAssets("SpreadsheetsSettings");
			if (assets.Length == 0)
				throw new Exception("Create SpreadsheetsSettings with name \"SpreadsheetsSettings\"");

			var settings = AssetDatabase.LoadAssetAtPath<SpreadsheetSettings>(AssetDatabase.GUIDToAssetPath(assets[0]));
			var connector = new SpreadsheetConnection(settings);
			var spreadsheet = new Spreadsheet(connector);
			return spreadsheet.GetTable(sheet, OnDownload);
		}

		private void StartCoroutine(IEnumerator enumerator)
		{
			_enumerator = enumerator;
			EditorApplication.update += Coroutine;
		}

		private void Coroutine()
		{
			if (_enumerator.MoveNext())
			{
				if (_enumerator.Current != null)
					_enumerator = (IEnumerator)_enumerator.Current;
			}
			else
			{
				EditorApplication.update -= Coroutine;
			}
		}

		private void OnDownload(string result)
		{
			_onComplete(result);
		}

		protected static List<T> Read<T>(string json)
		{
			json = Builder.Append("{\"List\":").Append(json).Append("}").ToString();
			Builder.Clear();
			return JsonUtility.FromJson<JsonItems<T>>(json).List;
		}

		protected void Save()
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		[Serializable]
		private class JsonItems<T>
		{
			public List<T> List;
		}
	}
}