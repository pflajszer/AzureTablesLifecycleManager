using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.Lib.Services
{
	public interface ITableManager
	{
		Task<IList<Response>> DeleteTablesAsync(IQueryBuilder query);
		Task<IList<Response>> DeleteTablesAsync(Expression<Func<TableItem, bool>> tableFilter);
		Task<IList<Response>> DeleteDataFromTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery) where T : class, ITableEntity, new();
		Task<IList<Response>> DeleteDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter, Expression<Func<T, bool>> dataFilter) where T : class, ITableEntity, new();
	}
}