using AzureTablesLifecycleManager.Lib.Extensions;
using AzureTablesLifecycleManager.SystemTests;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

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
