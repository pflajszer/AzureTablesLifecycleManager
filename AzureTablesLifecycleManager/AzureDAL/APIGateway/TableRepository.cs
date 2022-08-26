using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.AzureDAL.APIGateway
{
	public class TableRepository : ITableRepository
	{
		private readonly TableServiceClient _tableServiceClient;
		private readonly IList<TableClient> _tableClients;

		public IList<string> TableNames { get => _tableClients.Select(x => x.Name).ToList(); } 
		
		public TableRepository(TableServiceClient tableServiceClient)
		{
			_tableServiceClient = tableServiceClient;
			_tableClients = IntializeAllTableClients();
		}

		public Pageable<TableItem> GetTables(string filter)
		{
			return _tableServiceClient
				.Query(filter: filter);
		}

		public Pageable<TableItem> GetTables(Expression<Func<TableItem, bool>> filter = null)
		{
			return _tableServiceClient
				.Query(filter: filter is null ? x => true : filter);
		}

		public AsyncPageable<TableItem> GetTablesAsync(Expression<Func<TableItem, bool>> filter = null)
		{
			return _tableServiceClient
				.QueryAsync(filter: filter is null ? x => true : filter);
		}

		public AsyncPageable<TableItem> GetTablesAsync(string filter)
		{
			return _tableServiceClient
				.QueryAsync(filter: filter);
		}

		public Response<TableItem> CreateTable(string tableName)
		{
			var table =  _tableServiceClient.CreateTableIfNotExists(tableName);
			var tableClient = _tableServiceClient.GetTableClient(tableName);
			if (!_tableClients.Any(x => x.Name == tableName))
			{
				_tableClients.Add(tableClient);
			}
			return table;
		}

		public async Task<Response<TableItem>> CreateTableAsync(string tableName)
		{
			var table = await _tableServiceClient.CreateTableIfNotExistsAsync(tableName);
			var tableClient = _tableServiceClient.GetTableClient(tableName);
			if (!_tableClients.Any(x => x.Name == tableName))
			{
				_tableClients.Add(tableClient); 
			}
			return table;
		}

		public Response DeleteTable(TableItem table)
		{
			var tableClient = _tableClients.First(x => x.Name == table.Name);
			_tableClients.Remove(tableClient);
			return _tableServiceClient.DeleteTable(table.Name);
		}

		public async Task<Response> DeleteTableAsync(TableItem table)
		{
			var tableClient = _tableClients.First(x => x.Name == table.Name);
			_tableClients.Remove(tableClient);
			return await _tableServiceClient.DeleteTableAsync(table.Name);

		}

		public Pageable<T> GetTableEntities<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new()
		{
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			var result = tableClient.Query<T>(filter: filter is null ? x => true : filter);
			return result;
		}

		public AsyncPageable<T> GetTableEntitiesAsync<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new()
		{
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			var result = tableClient.QueryAsync<T>(filter: filter is null ? x => true : filter);
			return result;
		}

		public Pageable<T> GetTableEntities<T>(string tableName, string filter) where T : class, ITableEntity, new()
		{
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			var result = tableClient.Query<T>(filter: filter);
			return result;
		}

		public AsyncPageable<T> GetTableEntitiesAsync<T>(string tableName, string filter) where T : class, ITableEntity, new()
		{
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			var result = tableClient.QueryAsync<T>(filter: filter);
			return result;
		}

		public IList<Response> DeleteTableEntities<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new()
		{
			var responses = new List<Response>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			tableClient.Query<T>(filter: filter is null ? x => true : filter )
				.ToList()
				.ForEach(x =>
				{
					var resp = tableClient.DeleteEntity(x.PartitionKey, x.RowKey);
					responses.Add(resp);
				});
			return responses;
		}

		public async Task<IList<Response>> DeleteTableEntitiesAsync<T>(string tableName, Expression<Func<T, bool>> filter = null) where T : class, ITableEntity, new()
		{
			var responseTasks = new List<Task<Response>>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			tableClient.Query<T>(filter: filter is null ? x => true : filter)
				.ToList()
				.ForEach(x =>
				{
					var resp = tableClient.DeleteEntityAsync(x.PartitionKey, x.RowKey);
					responseTasks.Add(resp);
				});
			var responses = await Task.WhenAll(responseTasks);
			return responses;
		}

		public IList<Response> DeleteTableEntities<T>(string tableName, string filter) where T : class, ITableEntity, new()
		{
			var responses = new List<Response>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			tableClient.Query<T>(filter: filter)
				.ToList()
				.ForEach(x =>
				{
					var resp = tableClient.DeleteEntity(x.PartitionKey, x.RowKey);
					responses.Add(resp);
				});
			return responses;
		}

		public async Task<IList<Response>> DeleteTableEntitiesAsync<T>(string tableName, string filter) where T : class, ITableEntity, new()
		{
			var responseTasks = new List<Task<Response>>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			tableClient.Query<T>(filter: filter)
				.ToList()
				.ForEach(x =>
				{
					var resp = tableClient.DeleteEntityAsync(x.PartitionKey, x.RowKey);
					responseTasks.Add(resp);
				});
			var responses = await Task.WhenAll(responseTasks);
			return responses;
		}

		public IList<Response> AddTableEntities<T>(string tableName, List<T> entities) where T : class, ITableEntity, new()
		{
			var responses = new List<Response>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			entities.ForEach(x =>
				{
					var resp = tableClient.AddEntity<T>(x);
					responses.Add(resp);
				});
			return responses;
		}

		public async Task<IList<Response>> AddTableEntitiesAsync<T>(string tableName, List<T> entities) where T : class, ITableEntity, new()
		{
			var responseTasks = new List<Task<Response>>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			entities.ForEach(x =>
			{
				var resp = tableClient.AddEntityAsync<T>(x);
				responseTasks.Add(resp);
			});

			var responses = await Task.WhenAll(responseTasks);
			return responses;
		}

		public async Task<IList<Response>> UpdateTableEntitiesAsync<T>(string tableName, List<T> entities, TableUpdateMode updateMode = TableUpdateMode.Merge) where T : class, ITableEntity, new()
		{
			var responseTasks = new List<Task<Response>>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			entities.ForEach(x =>
			{
				var resp = tableClient.UpdateEntityAsync<T>(x, x.ETag, updateMode);
				responseTasks.Add(resp);
			});

			var responses = await Task.WhenAll(responseTasks);
			return responses;
		}

		public async Task<IList<Response>> AddTableEntitiesAsync<T>(string tableName, AsyncPageable<T> entities) where T : class, ITableEntity, new()
		{
			var responseTasks = new List<Task<Response>>();
			var tableClient = _tableClients
				.Single(x => x.Name == tableName);
			await foreach (var entity in entities)
			{
				var resp = tableClient.AddEntityAsync<T>(entity);
				responseTasks.Add(resp);
			}

			var responses = await Task.WhenAll(responseTasks);
			return responses;
		}

		private IList<TableClient> IntializeAllTableClients()
		{
			var clients = new List<TableClient>();
			_tableServiceClient.Query().ToList().ForEach(table =>
			{
				var tableClient = _tableServiceClient.GetTableClient(table.Name);
				clients.Add(tableClient);
			});
			return clients;
		}


	}
}
