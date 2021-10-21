using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.AzureDAL.APIGateway
{
	public class TableEntitiesAPI : ITableEntitiesAPI
	{
		private readonly TableClient _tableClient;
		public TableEntitiesAPI(TableClient tableClient)
		{
			_tableClient = tableClient;
		}

		public Pageable<TableEntity> Query(string filter)
		{
			return _tableClient.Query<TableEntity>(filter: filter);
		}

		public Pageable<TableEntity> Query(Expression<Func<TableEntity, bool>> filter)
		{
			return _tableClient.Query<TableEntity>(filter);
		}


		public Response DeleteEntityFromTable(TableEntity entity)
		{
			return _tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
		}

		public async Task<Response> DeleteEntityFromTableAsync(TableEntity entity)
		{
			return await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
		}

		public IEnumerable<Response> DeleteEntitiesFromTable(IEnumerable<TableEntity> entities)
		{
			var responses = new List<Response>();
			foreach (var item in entities)
			{
				var resp = _tableClient.DeleteEntity(item.PartitionKey, item.RowKey);
				responses.Add(resp);
			}
			return responses;
		}

		public async Task<IEnumerable<Response>> DeleteEntitiesFromTableAsync(IEnumerable<TableEntity> entities)
		{
			var responses = new List<Response>();
			foreach (var item in entities)
			{
				var resp = await _tableClient.DeleteEntityAsync(item.PartitionKey, item.RowKey);
				responses.Add(resp);
			}
			return responses;
		}
	}
}
