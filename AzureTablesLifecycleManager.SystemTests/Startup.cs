using Azure.Data.Tables;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Services;
using AzureTablesLifecycleManager.SystemTests;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AzureTablesLifecycleManager.Lib.Extensions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzureTablesLifecycleManager.SystemTests
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.RegisterAzureTablesLifecycleManagement();
		}
	}
}
