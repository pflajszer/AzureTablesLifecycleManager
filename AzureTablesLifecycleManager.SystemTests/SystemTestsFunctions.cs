using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.BRL.Services;

namespace AzureTablesLifecycleManager.SystemTests
{
    public class SystemTestsFunctions
    {
        private readonly TableInquisitor _api;
		public SystemTestsFunctions(TableInquisitor api)
		{
            _api = api;
		}


        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            _api.DeleteTablesWithPrefix("abc");

            return new OkResult();
        }
    }
}
