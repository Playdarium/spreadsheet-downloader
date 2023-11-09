using Playdarium.SpreadsheetDownloader.Helpers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(TestDatabase))]
	public class TestDatabaseEditor : SingleSpreadsheetDownloader
	{
		protected override string SpreadsheetName => "Translation";

		protected override void Serialize(string json)
		{
			Debug.Log($"[{nameof(TestDatabaseEditor)}] \n{json}");
		}
	}
}