using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.Lib.Extensions;
using AzureTablesLifecycleManager.Lib.Services;
using AzureTablesLifecycleManager.Models;
using AzureTablesLifecycleManager.TestResources.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.SystemTests
{
	public class SystemTestsFunctions
	{
		private readonly ITableManager _api;
		private readonly IQueryBuilder _query;
		public SystemTestsFunctions(ITableManager api, IQueryBuilder query)
		{
			_api = api;
			_query = query;
		}


		[FunctionName("Function1")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			await DoSomethingWithDataOlderThanAYearUsingQueryBuilder<ProductEntity>(3);
			await DoSomethingWithDataOlderThanAYearUsingLINQExpression<ProductEntity>(3);

			return new OkResult();
		}

		public async Task<DataTransferResponse<T>> DoSomethingWithDataOlderThanAYearUsingQueryBuilder<T>(int option) where T : class, ITableEntity, new()
		{
			// this will return all the tables since it's an empty query:
			var tableQuery =
				new QueryBuilder();

			// this will return all the data older than 1 year ago:
			var dataQuery =
				new QueryBuilder()
					.AppendCondition(ODataPredefinedFilters.TimestampLessThanOrEqual(DateTime.Now.AddYears(-1)));

			var dtr = new DataTransferResponse<T>();

			switch (option)
			{
				case 1:
					// this will move all the data that match the above filters to a new table:
					var newTableName = "someNewTable";
					newTableName.EnsureValidAzureTableName();
					dtr = await _api.MoveDataBetweenTablesAsync<T>(tableQuery, dataQuery, newTableName);
					break;
				case 2:
					// ...or delete it permanently:
					dtr = await _api.DeleteDataFromTablesAsync<T>(tableQuery, dataQuery);
					break;
				case 3:
					// ...or just fetch the data:
					dtr = await _api.GetDataFromTablesAsync<T>(tableQuery, dataQuery);
					break;
				default:
					break;
			}

			return dtr;
		}

		public async Task<DataTransferResponse<T>> DoSomethingWithDataOlderThanAYearUsingLINQExpression<T>(int option) where T : class, ITableEntity, new()
		{
			// this query will return all the tables:
			Expression<Func<TableItem, bool>> tableQuery = x => true;

			// this query will return all data in the above tables that matches the condition (all data older than 1 year ago)
			Expression<Func<T, bool>> dataQuery = x => x.Timestamp < DateTime.Now.AddYears(-1);

			var dtr = new DataTransferResponse<T>();

			switch (option)
			{
				case 1:
					// Moving the data to a new table:
					var newTableName = "newTableName";
					newTableName.EnsureValidAzureTableName();
					dtr = await _api.MoveDataBetweenTablesAsync<T>(tableQuery, dataQuery, newTableName);
					break;
				case 2:
					// this call will delete the data that match the above filters:
					dtr = await _api.DeleteDataFromTablesAsync<T>(tableQuery, dataQuery);
					break;
				case 3:
					// ...or just fetch the data:
					dtr = await _api.GetDataFromTablesAsync<T>(tableQuery, dataQuery);
					break;
				default:
					break;
			}

			return dtr;
		}
	}
}
