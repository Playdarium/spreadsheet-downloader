using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace Package.SpreadsheetDownloader
{
	public class SpreadsheetConnection
	{
		private readonly SpreadsheetSettings _settings;

		private UnityWebRequest _request;

		public SpreadsheetConnection(SpreadsheetSettings settings)
		{
			_settings = settings;
		}

		public IEnumerator Create(Dictionary<string, string> form, Action<string> callback)
		{
			form.Add("ssid", _settings.spreadsheet);
			form.Add("pass", _settings.password);
			_request = UnityWebRequest.Post(_settings.url, form);
			_request.SendWebRequest();
			var begin = Time.realtimeSinceStartup;
			while (!_request.isDone)
			{
				if (Time.realtimeSinceStartup - begin >= _settings.timeout)
				{
					Debug.LogError("Time out: " + (Time.realtimeSinceStartup - begin));
					callback("Timeout");
					break;
				}

				yield return null;
			}

			var elapsedTime = Time.realtimeSinceStartup - begin;
			if (!string.IsNullOrEmpty(_request.error))
			{
				Debug.LogError(
					$"Connection error after {elapsedTime.ToString(CultureInfo.InvariantCulture)} " +
					$"seconds: {_request.error} {elapsedTime}");
				callback("Error");
				yield break;
			}

			callback(_request.downloadHandler.text);
		}
	}
}