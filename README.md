
# AzureTablesLifecycleManager
Helper library to manage the lifecycle of Azure Table tables and entities

## A Word of warning

Misusing this library can have some serious consequences - Please play around with it using a Storage Emulator or `Azurite` (recommended) before connecting to prod. If you connect to your live storage account, this library has the power to wipe all data from it if used incorrectly. In example - providing two empty filters will return all the tables and all data within them, if you then invoke the delete method... you know what will happen ;-)

## Installation

#### Package Manager:
`Install-Package AzureTablesLifecycleManager`
#### .NET CLI:
`dotnet add package AzureTablesLifecycleManager`

## Setup

When using dependency injection in .NET Core 3.X, you can register type like so, by registering a type in the ```ConfigureServices()``` method. To use the below extension method, you need to have an evironment variable called `AzureWebJobsStorage` with your Azure Storage Connection String as a value and ability to inject `IConfiguration`.

Startup.cs:
```csharp
	public override void Configure(IFunctionsHostBuilder builder)
	{
		builder.RegisterAzureTablesLifecycleManagement();
	}
```

Alternatively you can just call the below to register your services:
```csharp
	builder.Services.AddSingleton(p => new TableServiceClient(p.GetService<IConfiguration>()["AzureWebJobsStorage"]));
	builder.Services.AddSingleton<ITableRepository, TableRepository>();
	builder.Services.AddSingleton<ITableManager, TableManager>();
	builder.Services.AddSingleton<IQueryBuilder, QueryBuilder>();
```

MyClass.cs:

```csharp
public class MyClass
{
	private readonly ITableManager _api;
	private readonly IQueryBuilder _queryBuilder;

	public MyService(IOpenWeatherMapAPIClient api, IQueryBuilder _queryBuilder)
	{
		_api = api;
		_queryBuilder = queryBuilder;
	}
}
```

## Usage

The library allows you to manage your tables using a LINQ expressions, or OData filters. While LINQ is widely known, OData - not so much. For this reason, the OData filtering is hidden behind an IQueryBuilder


The sample project can be found under `AzureTablesLifecycleManager.SystemTests` (Azure Functions app). Feel free to clone and play around!


Query tables using LINQ Expression:

```csharp
	public async Task ArchiveEverythingThatsOlderThanAYear()
	{
		try
		{
			// this query will return all the tables:
			Expression<Func<TableItem, bool>> tableQuery = x => true;
			
			// this query will return all data in the above tables that matches the condition (all data older than 1 year ago)
			Expression<Func<ProductEntity, bool>> dataQuery = x => x.Timestamp < DateTime.Now.AddYears(-1);

			// this call will delete the data that match the above filters:
			var dataDeletedResponse = await _api.DeleteDataFromTablesAsync<ProductEntity>(tableQuery, dataQuery);
			
			// the alternative to the above would be Archiving the tables first. 
			// It's nothing fancy, but it will duplicate the table and add ARCHIVE prefix to its name
			// which, at least for the initial period of using the library allows you to see what data would you delete:
			var dataArchivedResponse = await _api.ArchiveDataFromTablesAsync<ProductEntity>(tableQuery, dataQuery);
		}
		catch (Exception ex)
		{

			throw;
		}
	}

```

Query tables using `IQueryBuilder` (OData filters under the surface)

```csharp
	public async Task<IActionResult> ArchiveEverythingThatsOlderThanAYear()
	{
		try
		{
			// this will return all the tables since it's an empty query:
			var tableQuery =
				new QueryBuilder();

			// this will return all the data older than 1 year ago:
			var dataQuery =
				new QueryBuilder()
					.AppendCondition(ODataPredefinedFilters.TimestampLessThanOrEqual(DateTime.Now.AddYears(-1)));

			
			// this will archive all the data that match the above filters:
			var dataArchiveResponse = await _api.ArchiveDataFromTablesAsync<ProductEntity>(tableQuery, dataQuery);

			// or delete it permanently:
			var dataDeleteResponse = await _api.ArchiveDataFromTablesAsync<ProductEntity>(tableQuery, dataQuery);


			return new OkResult();
		}
		catch (Exception ex)
		{
			throw;
		}
	}
```
## Testing

Some Integration and System tests are available in the repo.
You can control which Azure Storage account is used for the tests by changing the value of `AzureTablesLifecycleManager.TestResources.ConfigConstants.ConnectionString` constant. 
By default, it's using a local account (Emulator/Azurite) and I'd suggest leaving it that way. Running the tests inserts and delete some data, so please be careful with it.


## Development

N/A

## Deployment


Use tags for versioning. Check the current iteration (tag) and in cmd:

```git
git checkout [test/master]
git pull
git tag v[Major].[Minor].[Patch]-[beta if test branch]
git push origin [version]
```

Then, push the code to test and merge into master.
