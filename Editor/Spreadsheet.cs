using System;
using System.Collections;
using System.Collections.Generic;

namespace Playdarium.SpreadsheetDownloader
{
	public class Spreadsheet
	{
		private readonly SpreadsheetConnection _connection;

		public Spreadsheet(SpreadsheetConnection connection)
		{
			_connection = connection;
		}

		public IEnumerator GetTable(string name, Action<string> onComplete)
		{
			yield return _connection.Create(new Dictionary<string, string>
				{
					{ "type", name }
				},
				onComplete
			);
		}
	}
}