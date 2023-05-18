
# AzureTablesLifecycleManager
Helper library to manage the lifecycle of Azure Table tables and entities.

<p align="center">
    <img src="Resources/logo.png">
</p>

## Change Log

### v2.1.0 / v2.1.0-beta > Updated the way the library services are registered
- `RegisterAzureTablesLifecycleManagement` marked as `Obsolete` and will be removed in v3.0.0
- Added `AddAzureTablesLifecycleManagement` extension method of `IServiceCollection` to register the services. If you already have the old method in use, just replace `builder.RegisterAzureTablesLifecycleManagement()` with `builder.Services.AddAzureTablesLifecycleManagement()`
- The new method is now using the static `Environment.GetEnvironmentVariable()` method instead of grabbing a registered `IConfiguration` instance. We're still checking for `AzureWebJobsStorage` setting. Alternatively, you can use an overload of that method and pass in your connection string directly: `AddAzureTablesLifecycleManagement(this IServiceCollection services, string connectionString)`.
- Updated readme to reflect the changes

### v2.0.0 / v2.0.0-beta > .NET Standard  2.1 support
- You can now install the library using .NET Standard 2.1. No other changes or dependencies changed

### v1.2.0 / v1.2.0-beta > Update Entities Feature
- You can now update existing entities using a `UpdateDataInTableAsync<T>` method. It'll update all entries passed in as `IEnumerable<T>` based on the items `ETag`, so it's important the updated entities are pulled directly from the table (i.e. using `GetDataFromTablesAsync<T>` method) and not generated manually. You can control how the data is updated by passing in an enum parameter `TableUpdateMode` - `Merge` (default) will only update changed properties, and `Replace` will replace the entire row.
- Added a test for the happy path of the feature 

## A word of warning

Misusing this library can have some serious consequences - Please play around with it using a Storage Emulator or `Azurite` (recommended) before connecting to prod. If you connect to your live storage account, this library has the power to wipe all data from it if used incorrectly. In example - providing two empty filters will return all the tables and all data within them, if you then invoke the delete method... you know what will happen ;-)

## Business problem

If you have used Azure Storage for some time, you might have learned about `Azure Lifecycle Management` provided by Azure for the hosted resources. Unfortunately, but probably by design, there is no way of controlling the lifecycle of your Azure Tables. While we should be extremely careful deleting data from a database, Azure Tables costs can pile up pretty quickly with audit trails and logs, but also some temp data that we leave there "for later" and never come back to it.
I decided to tackle that problem and implemented Azure Tables Lifecycle Manager.

## Capabilities

Currently, `ITableManager` supports the following functionalities:

| Feature | Description | Methods |
|---------|-------------|---------|
| Move data between tables | This allows to **move** existing data from one or mutliple tables that match a provided query to a new or existing table | `MoveDataBetweenTablesAsync<T>` (overloads using `LINQ` and `IQueryBuilder` are available) |
| Copy data between tables | This allows to **copy** existing data from one or mutliple tables that match a provided query to a new or existing table | `CopyDataFromTablesAsync<T>` (overloads using `LINQ` and `IQueryBuilder` are available) |
| Delete data from tables | Delete data from one/mulitple tables matching given filters | `DeleteDataFromTablesAsync<T>` (overloads using `LINQ` and `IQueryBuilder` are available) |
| Drop tables | Delete one/mulitple tables matching given filters | `DropTablesAsync<T>` (overloads using `LINQ` and `IQueryBuilder` are available) |
| Fetch data from tables | Fetch data from one/mulitple tables matching given filters | `GetDataFromTablesAsync<T>` (overloads using `LINQ` and `IQueryBuilder` are available) |
| Insert data into a table | Insert given collection of `ITableEntity` into a new or existing table | `InsertDataIntoTableAsync<T>` |
| Update data in a table | Update given collection of `ITableEntity` in an existing table based on the items' `ETag`. `TableUpdateMode` - `Merge` (default) will only update changed properties, and `Replace` will replace the entire row. | `UpdateDataInTableAsync<T>` |

## Installation

#### Package Manager:
`Install-Package AzureTablesLifecycleManager`
#### .NET CLI:
`dotnet add package AzureTablesLifecycleManager`

## Setup

When using dependency injection in .NET Core, you can register type like so, by registering a type in the ```ConfigureServices()``` method. To use the below extension method, you need to have an evironment variable called `AzureWebJobsStorage` with your Azure Storage Connection String as a value. Alternatively, you can pass in the connection string directly.


### Azure Function app
Startup.cs:
```csharp
public override void Configure(IFunctionsHostBuilder builder)
{
	builder.Services.AddAzureTablesLifecycleManagement();

	// or pass the connection string directly:
	builder.Services.AddAzureTablesLifecycleManagement("UseDevelopmentStorage=true");
}
```

### Web app
Startup.cs/Program.cs:
```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddAzureTablesLifecycleManagement();

	// or pass the connection string directly:
	services.AddAzureTablesLifecycleManagement("UseDevelopmentStorage=true");
}
```


Alternatively you can just call the below to register your services:
```csharp
var myConnectionString = "UseDevelopmentStorage=true";
builder.Services.AddSingleton(p => new TableServiceClient(myConnectionString));
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

	public MyService(ITableManager api, IQueryBuilder _queryBuilder)
	{
		_api = api;
		_queryBuilder = queryBuilder;
	}
}
```

## Usage


### ITableManager

The library allows you to manage your tables using a LINQ expressions, or OData filters. While LINQ is widely known, OData - not so much. For this reason, the OData filtering is hidden behind an `IQueryBuilder`


The sample project can be found under `AzureTablesLifecycleManager.SystemTests` (Azure Functions app). Feel free to clone and play around!


#### Query tables using LINQ Expression:

```csharp
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
```

#### Query tables using `IQueryBuilder` (OData filters under the surface)

```csharp
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
```

For a runnable example, run the `AzureTablesLifecycleManager.SystemTests` project. Be careful with the connection string you provide!

### IQueryBuilder

If using LINQ expressions is not enough (while they're powerful, some of them aren't supported by Azure yet), then `IQueryBuilder` is worth looking into. It's a helper class using a builder pattern with fluent syntax. Under the hood, it builds OData queries.

```csharp
var rowKey = new Guid("512ef724-17dc-44a9-8e32-93fc212dbb4a").ToString();
var partitionKey = new Guid("f1142899-b6e4-4e0a-aecb-58fdc23df10f").ToString();
var date1 = DateTime.Parse("2021-10-21T20:42:03.2034035+01:00");
var date2 = DateTime.Parse("2021-10-24T20:42:03.2039446+01:00");

// some predefined queries already exist in ODataPredefinedFilters class:
string condition1 = ODataPredefinedFilters.RowKeyExact(rowKey);
string condition2 = ODataPredefinedFilters.PartitionKeyExact(partitionKey);
string condition3 = ODataPredefinedFilters.TimestampGreaterThan(date1);
string condition4 = ODataPredefinedFilters.TimestampLessThan(date2);

// you can also write your custom logic with help of the Custom query method, ODataComparisonOperators and TableEntityFields (or your own name of entity) classes like so:
string condition5 = ODataPredefinedFilters.Custom(("MyTableEntityField", ODataComparisonOperators.GreaterThanOrEqual, "SomeRowValue"));
// or
string condition6 = ODataPredefinedFilters.Custom((TableEntityFields.ETag, ODataComparisonOperators.Equals, "SomeETagValue"));

// Act
var queryBuilder = new QueryBuilder();
var result = queryBuilder
	.AppendCondition(condition1)
	.And()
	.StartSubCondition()
		.AppendCondition(condition2)
		.Or()
		.AppendCondition(condition3)
	.EndSubCondition()
	.And()
	.AppendCondition(condition4)
	.And()
	.AppendCondition(condition5)
	.Build();

// result: 
// "RowKey eq '512ef724-17dc-44a9-8e32-93fc212dbb4a' and (PartitionKey eq 'f1142899-b6e4-4e0a-aecb-58fdc23df10f' or Timestamp gt '2021-10-21T20:42:03.2034035+01:00') and Timestamp lt '2021-10-24T20:42:03.2039446+01:00' and MyTableEntityField ge 'SomeRowValue' and ETag eq 'SomeRowValue'"
```

Note: The `Build()` method allows you to check the output of the query building, but you normally pass the instance of `IQueryBuilder` to `ITableManager` methods without building it.

If you're re-using `IQueryBuilder`, i.e. you're using Dependency Injection, you can use the `Flush()` method like so to reset the builder:

```csharp
var queryBuilder = new QueryBuilder();
var query = queryBuilder
	.AppendCondition(firstCondition)
	.Build();

queryBuilder.Flush();

var anotherQuery = queryBuilder
	.AppendCondition(someOtherCondition)
	.Build();
```

### Extensions

There are a few extensions methods for you to use:

| Method       | Description | Extension of | Returns |
|--------------|-------------|--------------|----------
| `EnumerateAsyncPageable<T>()` | Enumerates and flattens the `AsyncPageable<T>` to a `IList<T>`, since this is the return type of Azure library methods       | `AsyncPageable<T>` | `IList<T>` |
| `RegisterAzureTablesLifecycleManagement()`      | **OBSOLETE**. Registers all types needed in Dependency Injection container    | `IFunctionsHostBuilder` | `Task` |
| `AddAzureTablesLifecycleManagement()`      | Registers all types needed in Dependency Injection container    | `IServiceCollection` | `Task` |
| `IsValidAzureTableName()`      | Determines if the given Azure Table name is valid with Azure requirements    | `string` | `bool` |
| `EnsureValidAzureTableName()`      | Throws `InvalidAzureTableNameException` when the string doesn't match Azure table naming requirements    | `string` | `void` |
| `AreOKResponses()`      | Determines if the transfer operations were successfull (201/204 response codes)     | `DataTransferResponse<T>` | `bool` |
| `EnsureCorrectResponses()`      | Throws `TransferNotSuccessfulException` when any of the responses are not indicating success    | `DataTransferResponse<T>` | `void` |

### Filters

#### OData

There are some premade filters in `ODataPredefinedFilters` class for you to explore.

- TimestampLessThanOrEqual
- TimestampLessThan
- TimestampGreaterThanOrEqual
- TimestampGreaterThan
- PartitionKeyExact
- RowKeyExact
- TableNameExact
- Custom (where you provide the column name, operator and the value)

#### Expression

There are some premade filters in `ExpressionPredefinedFilters` class for you to explore.

- HasPrefix



## Testing

Some Integration and System tests are available in the repo.
You can control which Azure Storage account is used for the tests by changing the value of `AzureTablesLifecycleManager.TestResources.ConfigConstants.ConnectionString` constant. 
By default, it's using a local account (`Storage Emulator`/`Azurite`) and I'd suggest leaving it that way. Running the tests inserts and delete some data, so please be careful with it.


## Development

### Adding new predefined queries

To make the usage of the library easier, you can add some predefined filters by adding methods to both `ExpressionPredefinedFilters` and `ODataPredefinedFilters` classes, following the existing conventions. Those are surfaced to the users.

### New ITableManager methods

This is pretty straight forward, just make sure your methods go via `ITableRepository` interface and not directly with Azure Tables library.

## Deployment

Use tags for versioning. Check the current iteration (tag) and in cmd:

```git
git checkout [test/master]
git pull
git tag v[Major].[Minor].[Patch]-[beta if test branch]
git push origin [version]
```

Then, push the code to test and merge into master.
