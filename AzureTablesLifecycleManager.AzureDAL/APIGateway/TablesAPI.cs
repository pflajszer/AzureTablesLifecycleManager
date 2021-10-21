using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.AzureDAL.APIGateway
{
	public class TablesAPI : ITablesAPI
	{
		private readonly TableServiceClient _tableServiceClient;
		public TablesAPI(TableServiceClient tableServiceClient)
		{
			_tableServiceClient = tableServiceClient;
		}

		public Pageable<TableItem> GetTablesUsingFilter(string filter)
		{
			return _tableServiceClient.Query(filter: filter);
		}

		public Pageable<TableItem> GetTablesUsingFilter(Expression<Func<TableItem, bool>> filter)
		{
			return _tableServiceClient.Query(filter: filter);
		}

		public Pageable<TableItem> GetAllTables()
		{
			return _tableServiceClient.Query();
		}

		public TableItem CreateTable(string tableName)
		{
			return _tableServiceClient.CreateTableIfNotExists(tableName);
		}

		public Response DeleteTable(TableItem table)
		{
			return _tableServiceClient.DeleteTable(table.Name);
		}

		public async Task<Response> DeleteTableAsync(TableItem table)
		{
			return await _tableServiceClient.DeleteTableAsync(table.Name);
		}
	}
}
