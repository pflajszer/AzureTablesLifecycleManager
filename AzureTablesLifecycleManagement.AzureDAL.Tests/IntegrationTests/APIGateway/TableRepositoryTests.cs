using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.AzureDAL.Models;
using AzureTablesLifecycleManager.TestResources;
using AzureTablesLifecycleManager.TestResources.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace AzureTablesLifecycleManagement.AzureDAL.Tests.IntegrationTests.APIGateway
{
	public class TableRepositoryTests
	{
		private readonly ITableRepository _sut;

		public TableRepositoryTests()
		{

			var client = new TableServiceClient(ConfigConstants.ConnectionString);
			_sut = new TableRepository(client);
		}


		[Fact]
		public void GetTables_StorageAccountHasTablesAndExactTableNameGivenUsingOData_FoundTable()
		{

			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			string filter = ODataPredefinedFilters.TableNameExact(tableName);

			// Act
			var result = _sut.GetTables(
				filter).ToList();

			// Assert
			Assert.Equal(tableName, result.Single().Name);

			// Clean up
			_sut.DeleteTable(result.First());

		}

		[Fact]
		public void GetTables_StorageAccountHasTablesAndExactTableNameGivenUsingExpression_FoundTable()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			Expression<Func<TableItem, bool>> filter = x => x.Name == tableName;

			// Act
			var result = _sut.GetTables(
				filter).ToList();

			// Assert
			Assert.Equal(tableName, result.Single().Name);

			// Clean up
			_sut.DeleteTable(result.First());
		}

		[Fact]
		public void GetTables_StorageAccountHasTables_FindsAnyTable()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var dummyTable = _sut.CreateTable(tableName);

			// Act
			var result = _sut.GetTables().ToList();

			// Assert
			Assert.True(result.Count > 0);

			// Clean up

			_sut.DeleteTable(dummyTable);
		}

		[Fact]
		public async Task GetTablesAsync_StorageAccountHasTables_FindsAnyTable()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var dummyTable = _sut.CreateTable(tableName);
			var resultList = new List<TableItem>();


			// Act
			var result = _sut.GetTablesAsync();

			await EnumerateAsyncPageable(resultList, result).ConfigureAwait(false);

			// Assert
			Assert.True(resultList.Count > 0);

			// Clean up

			_sut.DeleteTable(dummyTable);
		}

		[Fact]
		public async Task GetTablesAsync_StorageAccountHasTablesAndExactTableNameGivenUsingExpression_FoundTable()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var resultList = new List<TableItem>();
			Expression<Func<TableItem, bool>> filter = x => x.Name == tableName;

			// Act
			var result = _sut.GetTablesAsync(
				filter);

			await EnumerateAsyncPageable(resultList, result);

			// Assert
			Assert.Equal(tableName, resultList.Single().Name);

			// Clean up
			_sut.DeleteTable(resultList.Single());
		}

		[Fact]
		public async Task GetTablesAsync_StorageAccountHasTablesAndExactTableNameGivenUsingOData_FoundTable()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var resultList = new List<TableItem>();
			var filter = ODataPredefinedFilters.TableNameExact(tableName);

			// Act
			var result = _sut.GetTablesAsync(
				filter);

			await EnumerateAsyncPageable(resultList, result);

			// Assert
			Assert.Equal(tableName, resultList.Single().Name);

			// Clean up
			_sut.DeleteTable(resultList.Single());
		}

		[Fact]
		public void CreateTable_ValidNameGiven_SuccessfullyCreates()
		{
			// Arrange
			string tableName = EntityFactory.GenerateTableName("TEST");
			var existingTable = _sut.GetTables(x => x.Name == tableName);
			if (existingTable.Any())
			{
				_sut.DeleteTable(existingTable.First());
			}

			// Act
			var result = _sut.CreateTable(
				tableName);

			// Assert
			Assert.Equal(tableName, result.Value.Name);

			// Clean up
			_sut.DeleteTable(result);
		}

		[Fact]
		public void CreateTable_ValidNameGiven_TableClientGetsUpdated ()
		{
			// Arrange
			string tableName = EntityFactory.GenerateTableName("TEST");
			var existingTable = _sut.GetTables(x => x.Name == tableName);
			var numberOfTables = _sut.TableNames.Count;
			if (existingTable.Any())
			{
				_sut.DeleteTable(existingTable.First());
			}

			// Act
			var result = _sut.CreateTable(
				tableName);

			// Assert
			Assert.Equal(_sut.TableNames.Count, numberOfTables + 1);

			// Clean up
			_sut.DeleteTable(result);
			Assert.Equal(_sut.TableNames.Count, numberOfTables);
		}

		[Fact]
		public async Task CreateTableAsync_ValidNameGiven_SuccessfullyCreates()
		{
			// Arrange
			string tableName = EntityFactory.GenerateTableName("TEST");
			var existingTable = _sut.GetTables(x => x.Name == tableName);
			if (existingTable.Any())
			{
				_sut.DeleteTable(existingTable.First());
			}

			// Act
			var result = await _sut.CreateTableAsync(
				tableName);

			// Assert
			Assert.Equal(tableName, result.Value.Name);

			// Clean up
			_sut.DeleteTable(result);
		}

		[Fact]
		public async Task CreateTableAsync_ValidNameGiven_TableClientGetsUpdated()
		{
			// Arrange
			string tableName = EntityFactory.GenerateTableName("TEST");
			var existingTable = _sut.GetTables(x => x.Name == tableName);
			var numberOfTables = _sut.TableNames.Count;
			if (existingTable.Any())
			{
				_sut.DeleteTable(existingTable.First());
			}

			// Act
			var result = await _sut.CreateTableAsync(
				tableName);

			// Assert
			Assert.Equal(_sut.TableNames.Count, numberOfTables + 1);

			// Clean up
			_sut.DeleteTable(result);
			Assert.Equal(_sut.TableNames.Count, numberOfTables);
		}

		[Fact]
		public void DeleteTable_ExistingTableNameGiven_SuccessfullyDeletes()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);

			// Act
			var result = _sut.DeleteTable(
				table);

			// Assert
			Assert.Equal(204, result.Status);
		}

		[Fact]
		public async Task DeleteTableAsync_ExistingTableGiven_SuccessfullyDeletes()
		{
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);

			// Act
			var result = await _sut.DeleteTableAsync(
				table);

			// Assert
			Assert.Equal(204, result.Status);
		}

		[Fact]
		public void AddTableEntities_SeedDataProvided_SuccessfullyAddedEntities()
		{
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var seedData = EntityFactory.GetVariedSeedData(27);


			// Act
			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);


			// Assert
			foreach (var item in resps)
			{
				Assert.Equal(204, item.Status); 
			}

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public async Task AddTableEntitiesAsync_SeedDataProvided_SuccessfullyAddedEntities()
		{
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var seedData = EntityFactory.GetVariedSeedData(25);


			// Act
			var resps = await _sut.AddTableEntitiesAsync<TableEntity>(tableName, seedData);


			// Assert
			foreach (var item in resps)
			{
				Assert.Equal(204, item.Status);
			}

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public void DeleteTableEntities_NoFilterProvided_SuccessfullyDeletesAllEntities()
		{
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var seedData = EntityFactory.GetVariedSeedData(54);

			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);

			// Act
			var result = _sut.DeleteTableEntities<TableEntity>(
				tableName);

			// Assert
			foreach (var item in resps)
			{
				Assert.Equal(204, item.Status);
			}

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public async Task DeleteTableEntitiesAsync_NoFilterProvided_SuccessfullyDeletesAllEntities()
		{
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var seedData = EntityFactory.GetVariedSeedData(100);

			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);

			// Act
			var result = await _sut.DeleteTableEntitiesAsync<TableEntity>(
				tableName);

			// Assert
			foreach (var item in resps)
			{
				Assert.Equal(204, item.Status);
			}

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public void GetTableEntities_NoFilterProvided_SuccessfullyFetches()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			int numOfElements = 1000;
			var seedData = EntityFactory.GetVariedSeedData(numOfElements);
			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);


			// Act
			var result = _sut.GetTableEntities<TableEntity>(tableName).ToList();

			// Assert
			Assert.NotEmpty(result);
			Assert.Equal(numOfElements, result.Count);

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public async Task GetTableEntitiesAsync_NoFilterProvided_SuccessfullyFetches()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var resultList = new List<TableEntity>();
			var seedData = EntityFactory.GetVariedSeedData(10);
			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);


			// Act
			var result = _sut.GetTableEntitiesAsync<TableEntity>(tableName);

			await EnumerateAsyncPageable(resultList, result);

			// Assert
			Assert.NotEmpty(resultList);

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public void GetTableEntities_ExpressionFilterProvided_SuccessfullyFetches()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var dummyEntity = new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			{
				{ "Product", "Other product" },
				{ "Price", "10.00" },
				{ "Quantity", "999" },
			};
			var seedData = EntityFactory.GetVariedSeedData(10);
			seedData.Add(dummyEntity);
			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);


			// Act
			var result = _sut.GetTableEntities<TableEntity>
				(tableName, 
				x => x.RowKey == dummyEntity.RowKey)
				.ToList();

			// Assert
			Assert.NotEmpty(result);
			Assert.Single(result);

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public async void GetTableEntitiesAsync_ExpressionFilterProvided_SuccessfullyFetches()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var resultList = new List<TableEntity>();
			var dummyEntity = new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			{
				{ "Product", "Other product" },
				{ "Price", "10.00" },
				{ "Quantity", "999" },
			};
			var seedData = EntityFactory.GetVariedSeedData(10);
			seedData.Add(dummyEntity);
			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);


			// Act
			var result = _sut.GetTableEntitiesAsync<TableEntity>
				(tableName,
				x => x.RowKey == dummyEntity.RowKey);

			await EnumerateAsyncPageable(resultList, result);

			// Assert
			Assert.NotEmpty(resultList);
			Assert.Single(resultList);

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public void GetTableEntities_ODataFilterProvided_SuccessfullyFetches()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var dummyEntity = new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			{
				{ "Product", "Other product" },
				{ "Price", "10.00" },
				{ "Quantity", "999" },
			};
			var seedData = EntityFactory.GetVariedSeedData(10);
			seedData.Add(dummyEntity);
			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);
			var filter = ODataPredefinedFilters.PartitionKeyExact(dummyEntity.PartitionKey);


			// Act
			var result = _sut.GetTableEntities<TableEntity>
				(tableName,
				filter)
				.ToList();

			// Assert
			Assert.NotEmpty(result);
			Assert.Single(result);

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public async Task GetTableEntitiesAsync_ODataFilterProvided_SuccessfullyFetches()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _sut.CreateTable(tableName);
			var resultList = new List<TableEntity>();

			var dummyEntity = new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			{
				{ "Product", "Other product" },
				{ "Price", "10.00" },
				{ "Quantity", "999" },
			};
			var seedData = EntityFactory.GetVariedSeedData(10);
			seedData.Add(dummyEntity);
			var resps = _sut.AddTableEntities<TableEntity>(tableName, seedData);
			var filter = ODataPredefinedFilters.PartitionKeyExact(dummyEntity.PartitionKey);


			// Act
			var result = _sut.GetTableEntitiesAsync<TableEntity>
				(tableName,
				filter);

			await EnumerateAsyncPageable(resultList, result);

			// Assert
			Assert.NotEmpty(resultList);
			Assert.Single(resultList);

			// Clean up
			_sut.DeleteTable(table);
		}

		private static async Task EnumerateAsyncPageable<T>(List<T> resultList, AsyncPageable<T> result)
		{
			await foreach (var page in result.AsPages())
			{
				// enumerate through page items
				foreach (var v in page.Values)
				{
					resultList.Add(v);
				}

				// get continuation token that can be used in AsPages call to resume enumeration
				Console.WriteLine(page.ContinuationToken);
			}
		}
	}
}
