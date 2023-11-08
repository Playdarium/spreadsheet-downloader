using System;
using System.Collections.Generic;
using System.Linq;

namespace Playdarium.SpreadsheetDownloader.Runtime
{
	public static class LinqExtensions
	{
		public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> list, Predicate<T> predicate)
		{
			var outList = new List<List<T>>();
			var items = new List<T>();

			void AddItems(IEnumerable<T> inItems)
			{
				if (inItems.Any())
					outList.Add(items);
			}

			foreach (var item in list)
			{
				if (predicate.Invoke(item))
				{
					AddItems(items);
					items = new List<T>();
				}

				items.Add(item);
			}

			AddItems(items);

			return outList;
		}
	}
}