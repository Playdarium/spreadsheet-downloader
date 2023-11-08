using UnityEngine;

namespace Playdarium.SpreadsheetDownloader
{
	[CreateAssetMenu(menuName = "Settings/Spreadsheet", fileName = "SpreadsheetsSettings")]
	public class SpreadsheetSettings : ScriptableObject
	{
		[Tooltip("Address of macros script")] public string url = "";

		[Tooltip("Table code '1pt8x...88lp8we5rk'")]
		public string spreadsheet = "";

		public string password = "passcode";
		public float timeout = 20f;
	}
}