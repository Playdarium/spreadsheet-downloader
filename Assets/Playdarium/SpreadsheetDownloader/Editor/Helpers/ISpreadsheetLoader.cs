using System;

namespace Playdarium.SpreadsheetDownloader.Helpers
{
	public interface ISpreadsheetLoader
	{
		event Action<bool> Success;

		void DownloadAndSerialize();
	}
}