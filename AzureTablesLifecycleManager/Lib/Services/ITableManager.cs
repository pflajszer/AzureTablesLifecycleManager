using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.Lib.Services
{
	public interface ITableManager
	{
		Task<DataTransferResponse<T>> CopyDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter, Expression<Func<T, bool>> dataFilter, string newTableName) where T : class, ITableEntity, new();
		Task<DataTransferResponse<T>> CopyDataFromTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery, string newTableName) where T : class, ITableEntity, new();
		Task<DataTransferResponse<T>> DeleteDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter, Expression<Func<T, bool>> dataFilter) where T : class, ITableEntity, new();
		Task<DataTransferResponse<T>> DeleteDataFromTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery) where T : class, ITableEntity, new();
		Task<DataTransferResponse<TableItem>> DropTablesAsync(Expression<Func<TableItem, bool>> tableFilter);
		Task<DataTransferResponse<TableItem>> DropTablesAsync(IQueryBuilder query);
		Task<DataTransferResponse<T>> GetDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter, Expression<Func<T, bool>> dataFilter) where T : class, ITableEntity, new();
		Task<DataTransferResponse<T>> GetDataFromTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery) where T : class, ITableEntity, new();
		Task<DataTransferResponse<T>> MoveDataBetweenTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter, Expression<Func<T, bool>> dataFilter, string newTableName) where T : class, ITableEntity, new();
		Task<DataTransferResponse<T>> MoveDataBetweenTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery, string newTableName) where T : class, ITableEntity, new();
		Task<DataTransferResponse<T>> InsertDataIntoTableAsync<T>(string tableName, IEnumerable<T> data) where T : class, ITableEntity, new();
	}
}