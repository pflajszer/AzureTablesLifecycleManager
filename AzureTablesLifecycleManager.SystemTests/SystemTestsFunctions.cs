using AzureTablesLifecycleManager.AzureDAL.Models;
using AzureTablesLifecycleManager.Lib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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

			//_api.DeleteTablesWithPrefix("abc");

			var query = _query
				.AppendCondition(ODataPredefinedFilters.TableNameExact("abc"));

			await _api.DeleteTablesAsync(query);
				

			return new OkResult();
		}
	}
}
