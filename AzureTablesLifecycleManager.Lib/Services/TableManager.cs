using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Models.Shared;
using System;
using System.Collections.Generic;
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

		public async Task<IList<Response>> DeleteTablesAsync(IQueryBuilder query)
		{
			var tableFilter = query?.Build();
			var responses = new List<Response>();
			var tables = _tableRepo.GetTablesAsync(tableFilter);
			await foreach (var table in tables)
			{
				var resps = await _tableRepo.DeleteTableAsync(table);
				responses.Add(resps);
			}
			return responses;
		}

		public async Task<IList<Response>> DeleteTablesAsync(Expression<Func<TableItem, bool>> tableFilter)
		{
			var responses = new List<Response>();
			var tables = _tableRepo.GetTablesAsync(tableFilter);
			await foreach (var table in tables)
			{
				var resps = await _tableRepo.DeleteTableAsync(table);
				responses.Add(resps);
			}
			return responses;
		}

		public async Task<IList<Response>> DeleteDataFromTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery) 
			where T : class, ITableEntity, new()
		{
			var tableFilter = tableQuery.Build();
			var dataFilter = dataQuery.Build();

			var responses = new List<Response>();
			var tables = _tableRepo.GetTablesAsync(tableFilter);
			await foreach (var table in tables)
			{
				var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dataFilter);
				responses.AddRange(resps);
			}
			return responses;
		}

		public async Task<IList<Response>> DeleteDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter,
																		Expression<Func<T, bool>> dataFilter) 
			where T : class, ITableEntity, new()
		{

			var responses = new List<Response>();
			var tables = _tableRepo.GetTablesAsync(tableFilter);
			await foreach (var table in tables)
			{
				var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dataFilter);
				responses.AddRange(resps);
			}
			return responses;
		}

		public async Task<IList<Response>> ArchiveDataFromTablesAsync<T>(IQueryBuilder tableQuery, IQueryBuilder dataQuery)
			where T : class, ITableEntity, new()
		{
			var tableFilter = tableQuery.Build();
			var dataFilter = dataQuery.Build();

			var responses = new List<Response>();
			var tables = _tableRepo.GetTablesAsync(tableFilter);
			await foreach (var table in tables)
			{
				var newTable = await _tableRepo.CreateTableAsync(TablePrefixes.ArchiveTablePrefix + table.Name);
				var dataToTransfer = _tableRepo.GetTableEntitiesAsync<T>(table.Name, dataFilter);
				await _tableRepo.AddTableEntitiesAsync<T>(newTable.Value.Name, dataToTransfer);
				var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dataFilter);
				responses.AddRange(resps);
			}
			return responses;
		}

		public async Task<IList<Response>> ArchiveDataFromTablesAsync<T>(Expression<Func<TableItem, bool>> tableFilter,
																		Expression<Func<T, bool>> dataFilter)
			where T : class, ITableEntity, new()
		{
			var responses = new List<Response>();
			var tables = _tableRepo.GetTablesAsync(tableFilter);
			await foreach (var table in tables)
			{
				var newTable = await _tableRepo.CreateTableAsync(TablePrefixes.ArchiveTablePrefix + table.Name);
				var dataToTransfer = _tableRepo.GetTableEntitiesAsync<T>(table.Name, dataFilter);
				await _tableRepo.AddTableEntitiesAsync<T>(newTable.Value.Name, dataToTransfer);
				var resps = await _tableRepo.DeleteTableEntitiesAsync<T>(table.Name, dataFilter);
				responses.AddRange(resps);
			}
			return responses;
		}

	}
}
