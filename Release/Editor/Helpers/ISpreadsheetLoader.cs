using System;

namespace Package.SpreadsheetDownloader.Helpers
{
	public interface ISpreadsheetLoader
	{
		event Action<bool> Success;

		void DownloadAndSerialize();
	}
}