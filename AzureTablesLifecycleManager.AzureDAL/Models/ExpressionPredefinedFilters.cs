using Azure.Data.Tables.Models;
using System;
using System.Linq.Expressions;

namespace AzureTablesLifecycleManager.AzureDAL.Models
{
	public static class ExpressionPredefinedFilters
	{
		/// <summary>
		/// https://stackoverflow.com/a/13036379/10816404
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public static Expression<Func<TableItem, bool>> HasPrefix(string prefix)
		{
			char lastChar = prefix[prefix.Length - 1];
			char nextLastChar = (char)((int)lastChar + 1);
			string nextPrefix = prefix.Substring(0, prefix.Length - 1) + nextLastChar;

			return e => e.Name.CompareTo(prefix) >= 0 && e.Name.CompareTo(nextPrefix) < 0;
		}
	}
}
