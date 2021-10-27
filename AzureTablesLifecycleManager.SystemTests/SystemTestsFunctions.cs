using Azure;
using Azure.Data.Tables;
using AzureTablesLifecycleManager.AzureDAL.Models;
using AzureTablesLifecycleManager.Lib.Services;
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
			try
			{
				Expression<Func<ProductEntity, bool>> dataQ = x => x.Product == "Marker Set";
				var x = await _api.ArchiveDataFromTablesAsync<ProductEntity>(x => true, dataQ);

				return new OkResult();
			}
			catch (Exception ex)
			{

				throw;
			}
			
		}


	}
}
