using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.AzureDAL.APIGateway
{
	public interface ITableRepository
	{
		IList<string> TableNames { get; }

		Response<TableItem> CreateTable(string tableName);
		Task<Response<TableItem>> CreateTableAsync(string tableName);
		Response DeleteTable(TableItem table);
		Task<Response> DeleteTableAsync(TableItem table);
		Pageable<TableItem> GetTables(Expression<Func<TableItem, bool>> filter = null);
		AsyncPageable<TableItem> GetTablesAsync(Expression<Func<TableItem, bool>> filter = null);
		AsyncPageable<TableItem> GetTablesAsync(string filter);
		Pageable<T> GetTableEntities<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new();
		AsyncPageable<T> GetTableEntitiesAsync<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new();
		Pageable<T> GetTableEntities<T>(string tableName, string filter) where T : class, ITableEntity, new();
		AsyncPageable<T> GetTableEntitiesAsync<T>(string tableName, string filter) where T : class, ITableEntity, new();
		Pageable<TableItem> GetTables(string filter);
		IList<Response> AddTableEntities<T>(string tableName, List<T> entities) where T : class, ITableEntity, new();
		Task<IList<Response>> AddTableEntitiesAsync<T>(string tableName, List<T> entities) where T : class, ITableEntity, new();
		IList<Response> DeleteTableEntities<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new();
		Task<IList<Response>> DeleteTableEntitiesAsync<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new();
		Task<IList<Response>> DeleteTableEntitiesAsync<T>(string tableName, string filter) where T : class, ITableEntity, new();
		IList<Response> DeleteTableEntities<T>(string tableName, string filter) where T : class, ITableEntity, new();
	}
}