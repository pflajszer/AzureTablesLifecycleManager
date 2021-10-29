using Azure.Data.Tables;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureTablesLifecycleManager.Lib.Extensions
{
	public static class FunctionsBuilderExtensions
	{
		public static void RegisterAzureTablesLifecycleManagement(this IFunctionsHostBuilder builder)
		{
			builder.Services.AddSingleton(p => new TableServiceClient(p.GetService<IConfiguration>()["AzureWebJobsStorage"]));
			builder.Services.AddSingleton<ITableRepository, TableRepository>();
			builder.Services.AddSingleton<ITableManager, TableManager>();
			builder.Services.AddSingleton<IQueryBuilder, QueryBuilder>();
		}
	}
}
