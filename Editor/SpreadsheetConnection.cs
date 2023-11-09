using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Playdarium.SpreadsheetDownloader
{
	public class SpreadsheetConnection
	{
		private readonly SpreadsheetSettings _settings;

		public SpreadsheetConnection(SpreadsheetSettings settings)
		{
			_settings = settings;
		}

		public IEnumerator Create(Dictionary<string, string> form, Action<string> callback)
		{
			form.Add("ssid", _settings.spreadsheet);
			form.Add("pass", _settings.password);
			var request = UnityWebRequest.Post(_settings.url, form);
			request.timeout = _settings.timeout;
			yield return request.SendWebRequest();

			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.LogError($"Request error: {request.error}");
				callback("Error");
				yield break;
			}

			callback(request.downloadHandler.text);

			request.Dispose();
		}
	}
}