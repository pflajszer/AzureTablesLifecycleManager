using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Extensions;
using AzureTablesLifecycleManager.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.Lib.Services
{
	public class TableManager : ITableManager
	{
		private readonly ITableRepository _tableRepo;
		public TableManager(ITableRepository tableRepo)
		{
			_tableRepo = tableRepo;
		}

		public async Task<DataTransferResponse<TableItem>> DeleteTablesAsync(IQueryBuilder query)
		{
			var dtr = new DataTransferResponse<TableItem>();
			dtr.ODataTableQueryBuilt = query?.Build();
			var tables = _tableRepo.GetTablesAsync(dtr.ODataTableQueryBuilt);
			await foreach (var table in tables)
			{
				var resp = await _tableRepo.DeleteTableAsync(table);
				dtr.TableDeletedResponses.Add(resp);
			}
			return dtr;
		}

		public async Task<DataTransferResponse<TableItem>> DeleteTablesAsync(Expression<Func<TableItem, bool>> tableFilter)
		{
			var dtr = new DataTransferResponse<TableItem>();
			dtr.ExpressionTableQuery = tableFilter;
			var tables = _tableRepo.GetTablesAsync(dtr.ExpressionTableQuery);
			await foreach (var table in tables)
			{
				var resps = await _tableRepo.DeleteTableAsync(table);
				dtr.TableDeletedResponses.Add(resps);
			}
			return dtr;
		}

		public async Task<DataTransferResponse<T>> DeleteDataFromTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery)
			where T : class, ITableEntity, new()
		{
			var dtr = new DataTransferResponse<T>();
			dtr.ODataTableQueryBuilt = tableQuery.Build();
			dtr.ODataDataQueryBuilt = dataQuery.Build();

			dtr.TablesFilterResults = _tableRepo.GetTablesAsync(dtr.ODataTableQueryBuilt);
			await foreach (var table in dtr.TablesFilterResults)
			{
				var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dtr.ODataDataQueryBuilt);
				dtr.DataDeletedResponses.AddRange(resps);
			}
			return dtr;
		}

		public async Task<DataTransferResponse<T>> DeleteDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter,
																		Expression<Func<T, bool>> dataFilter)
			where T : class, ITableEntity, new()
		{
			var dtr = new DataTransferResponse<T>();
			dtr.TablesFilterResults = _tableRepo.GetTablesAsync(tableFilter);
			await foreach (var table in dtr.TablesFilterResults)
			{
				var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dataFilter);
				dtr.DataDeletedResponses.AddRange(resps);
			}
			return dtr;
		}

		public async Task<DataTransferResponse<T>> MoveDataBetweenTablesAsync<T>(IQueryBuilder tableQuery,
																	  IQueryBuilder dataQuery,
																	  string newTableName)
			where T : class, ITableEntity, new()
		{
			return await TransferDataUsingQueryBuilder<T>(tableQuery, dataQuery, newTableName, true);
		}

		public async Task<DataTransferResponse<T>> CopyDataFromTablesAsync<T>(IQueryBuilder tableQuery,
															  IQueryBuilder dataQuery,
															  string newTableName)
			where T : class, ITableEntity, new()
		{
			return await TransferDataUsingQueryBuilder<T>(tableQuery, dataQuery, newTableName, false);
		}

		public async Task<DataTransferResponse<T>> MoveDataBetweenTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter,
																		Expression<Func<T, bool>> dataFilter,
																		string newTableName)
			where T : class, ITableEntity, new()
		{
			return await TransferDataUsingExpressionFilter(tableFilter, dataFilter, newTableName, true);
		}

		public async Task<DataTransferResponse<T>> CopyDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter,
																Expression<Func<T, bool>> dataFilter,
																string newTableName)
			where T : class, ITableEntity, new()
		{
			return await TransferDataUsingExpressionFilter(tableFilter, dataFilter, newTableName, false);
		}

		public async Task<DataTransferResponse<T>> GetDataFromTablesAsync<T>(IQueryBuilder tableQuery,
															  IQueryBuilder dataQuery)
			where T : class, ITableEntity, new()
		{
			var dtr = new DataTransferResponse<T>();

			dtr.ODataTableQueryBuilt = tableQuery.Build();
			dtr.ODataDataQueryBuilt = dataQuery.Build();


			dtr.TablesFilterResults = _tableRepo.GetTablesAsync(dtr.ODataTableQueryBuilt);
			await foreach (var table in dtr.TablesFilterResults)
			{
				var dataFilterResults = _tableRepo.GetTableEntitiesAsync<T>(table.Name, dtr.ODataDataQueryBuilt);
				dtr.DataFilterResults[table.Name] = (dataFilterResults);
			}
			return dtr;
		}

		public async Task<DataTransferResponse<T>> GetDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter,
																Expression<Func<T, bool>> dataFilter)
			where T : class, ITableEntity, new()
		{
			var dtr = new DataTransferResponse<T>();

			dtr.ExpressionTableQuery = tableFilter;
			dtr.ExpressionDataQuery = dataFilter;


			dtr.TablesFilterResults = _tableRepo.GetTablesAsync(dtr.ExpressionTableQuery);
			await foreach (var table in dtr.TablesFilterResults)
			{
				var dataFilterResults = _tableRepo.GetTableEntitiesAsync<T>(table.Name, dtr.ExpressionDataQuery);
				dtr.DataFilterResults[table.Name] = (dataFilterResults);
			}
			return dtr;
		}



		private async Task<DataTransferResponse<T>> TransferDataUsingQueryBuilder<T>
			(IQueryBuilder tableQuery, IQueryBuilder dataQuery, string newTableName, bool deleteSourceData)
			where T : class, ITableEntity, new()
		{
			var dtr = new DataTransferResponse<T>();

			dtr.ODataTableQueryBuilt = tableQuery.Build();
			dtr.ODataDataQueryBuilt = dataQuery.Build();

			if (!newTableName.IsValidAzureTableName())
			{
				dtr.IsNewTableNameValid = false;
				return dtr;
			}

			dtr.TablesFilterResults = _tableRepo.GetTablesAsync(dtr.ODataTableQueryBuilt);
			await foreach (var table in dtr.TablesFilterResults)
			{
				dtr.TableAddedResponses.Add(await _tableRepo.CreateTableAsync(newTableName));
				var dataFilterResults = _tableRepo.GetTableEntitiesAsync<T>(table.Name, dtr.ODataDataQueryBuilt);
				dtr.DataFilterResults[table.Name] = (dataFilterResults);
				var dataAdded = await _tableRepo.AddTableEntitiesAsync<T>(newTableName, dataFilterResults);
				dtr.DataAddedResponses = dataAdded.ToList();
				if (deleteSourceData)
				{
					var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dtr.ODataDataQueryBuilt);
					dtr.DataDeletedResponses.AddRange(resps);
				}
			}
			return dtr;
		}

		private async Task<DataTransferResponse<T>> TransferDataUsingExpressionFilter<T>
			(Expression<Func<TableItem, bool>> tableFilter, Expression<Func<T, bool>> dataFilter, string newTableName, bool deleteSourceData)
			where T : class, ITableEntity, new()
		{
			var dtr = new DataTransferResponse<T>();
			dtr.ExpressionTableQuery = tableFilter;
			dtr.ExpressionDataQuery = dataFilter;

			if (!newTableName.IsValidAzureTableName())
			{
				dtr.IsNewTableNameValid = false;
				return dtr;
			}
			dtr.TablesFilterResults = _tableRepo.GetTablesAsync(dtr.ExpressionTableQuery);
			await foreach (var table in dtr.TablesFilterResults)
			{

				dtr.TableAddedResponses.Add(await _tableRepo.CreateTableAsync(newTableName));
				var dataFilterResults = _tableRepo.GetTableEntitiesAsync<T>(table.Name, dtr.ExpressionDataQuery);
				dtr.DataFilterResults[table.Name] = (dataFilterResults);
				var dataAdded = await _tableRepo.AddTableEntitiesAsync<T>(newTableName, dataFilterResults);
				dtr.DataAddedResponses = dataAdded.ToList();
				if (deleteSourceData)
				{
					var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dtr.ExpressionDataQuery);
					dtr.DataDeletedResponses.AddRange(resps);
				}
			}
			return dtr;
		}
	}
}
