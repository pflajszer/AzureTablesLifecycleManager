using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.Lib.Extensions
{
    public static class AsyncPageableExtensions
    {
		/// <summary>
		/// Flattens and enumerates AsyncPageable<T> to a List<T>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		public static async Task<List<T>> EnumerateAsyncPageable<T>(this AsyncPageable<T> result)
		{
			var res = new List<T>();
			await foreach (var page in result.AsPages())
			{
				foreach (var v in page.Values)
				{
					res.Add(v);
				}
			}
			return res;
		}
	}
}
