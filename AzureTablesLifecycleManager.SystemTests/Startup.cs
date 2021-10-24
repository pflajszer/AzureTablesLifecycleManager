using Azure.Data.Tables;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Services;
using AzureTablesLifecycleManager.SystemTests;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzureTablesLifecycleManager.SystemTests
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddSingleton(p => new TableServiceClient(p.GetService<IConfiguration>()["AzureWebJobsStorage"]));
			builder.Services.AddSingleton<ITableRepository, TableRepository>();
			builder.Services.AddSingleton<ITableManager, TableManager>();
			builder.Services.AddSingleton<IQueryBuilder, QueryBuilder>();
		}
	}
}
