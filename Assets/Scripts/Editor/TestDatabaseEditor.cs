using Package.SpreadsheetDownloader.Helpers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(TestDatabase))]
	public class TestDatabaseEditor : SingleSpreadsheetDownloader
	{
		protected override string SpreadsheetName => "TextNotes";

		protected override void Serialize(string json)
		{
			Debug.Log($"[{nameof(TestDatabaseEditor)}] \n{json}");
		}
	}
}