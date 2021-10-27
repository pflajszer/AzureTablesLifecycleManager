using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.AzureDAL.Models;
using AzureTablesLifecycleManager.Lib.Models.Shared;
using AzureTablesLifecycleManager.Lib.Services;
using AzureTablesLifecycleManager.TestResources;
using AzureTablesLifecycleManager.TestResources.Setup;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace AzureTablesLifecycleManager.Lib.Tests.IntegrationTests.Services
{
	public class TableManagerTests
	{
		private readonly TableManager _sut;
		private readonly TableRepository _repo;
		public TableManagerTests()
		{
			var client = new TableServiceClient(ConfigConstants.ConnectionString);
			_repo = new TableRepository(client);
			_sut = new TableManager(_repo);
		}

		[Fact]
		public async Task DeleteTablesAsync_EmptyFilter_SuccessfullyRemovesAllTables()
		{
			// Arrange
			var tableName = EntityFactory.GenerateTableName("TEST");
			var table = _repo.CreateTable(tableName);


			// Act
			var resp = await _sut.DeleteTablesAsync((QueryBuilder)null);

			// Assert
			foreach (var item in resp)
			{
				Assert.Equal(204, item.Status);
			}
		}

		[Fact]
		public async Task DeleteTablesAsync_PrefixFilterUsingExpression_SuccessfullyRemovesGivenTables()
		{
			// Arrange
			var prefix = "TEST";
			foreach (var item in Enumerable.Range(1, 20))
			{
				var tableName = EntityFactory.GenerateTableName(prefix);
				if (item == 10) // halfway
				{
					prefix = "TOBEREMOVED";
				}
				var table = _repo.CreateTable(tableName);
			}

			// Act
			var resp = await _sut.DeleteTablesAsync(ExpressionPredefinedFilters.HasPrefix(prefix));

			// Assert
			foreach (var item in resp)
			{
				Assert.Equal(204, item.Status);
			}
			Assert.Equal(10, resp.Count());

			// Clean up
			prefix = "TEST"; // rest of the tables
			await _sut.DeleteTablesAsync(ExpressionPredefinedFilters.HasPrefix(prefix));
		}

		[Fact]
		public async Task DeleteDataFromTablesAsync_SingleTableAndSampleDataToRemoveUsingODataFilter_SuccessfullyRemovesData()
		{
			// Arrange
			var prefix = "RmvUsiODataFilt";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeRemoved = 34;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeRemoved);
			_repo.AddTableEntities(tableName, seedData);
			var dt = DateTime.UtcNow;
			await Task.Delay(5000);
			seedData = EntityFactory.GetVariedSeedData(53);
			_repo.AddTableEntities(tableName, seedData);


			var tableQueryBuilder =
				new QueryBuilder().AppendCondition(ODataPredefinedFilters.TableNameExact(tableName));
			var dataQueryBuilder =
				new QueryBuilder().AppendCondition(ODataPredefinedFilters.TimestampLessThanOrEqual(dt));

			// Act
			var resp = await _sut.DeleteDataFromTablesAsync<TableEntity>(
				tableQueryBuilder,
				dataQueryBuilder);

			// Assert
			foreach (var item in resp)
			{
				Assert.Equal(204, item.Status);
			}
			Assert.Equal(numOfEntriesThatWillBeRemoved, resp.Count);

			// Clean up
			_repo.DeleteTable(table);
		}

		[Fact]
		public async Task DeleteDataFromTablesAsync_SingleTableAndSampleDataToRemoveUsingExpressionFilter_SuccessfullyRemovesData()
		{
			// Arrange
			var prefix = "RmvUsingExprFil";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeRemoved = 52;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeRemoved);
			_repo.AddTableEntities(tableName, seedData);
			var dt = DateTime.UtcNow;
			await Task.Delay(1000);
			seedData = EntityFactory.GetVariedSeedData(63);
			_repo.AddTableEntities(tableName, seedData);

			Expression<Func<TableItem, bool>> tableQuery = x => x.Name == tableName;
			Expression<Func<TableEntity, bool>> dataQuery = x => x.Timestamp.Value <= dt;

			// Act
			var resp = await _sut.DeleteDataFromTablesAsync<TableEntity>(
				tableQuery,
				dataQuery);

			// Assert
			foreach (var item in resp)
			{
				Assert.Equal(204, item.Status);
			}
			Assert.Equal(numOfEntriesThatWillBeRemoved, resp.Count);

			// Clean up
			_repo.DeleteTable(table);
		}

		[Fact]
		public async Task ArchiveDataFromTablesAsync_SingleTableAndSampleDataToRemoveUsingODataFilter_SuccessfullyRemovesData()
		{
			// Arrange
			var prefix = "RmvUsiODataFilt";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeRemoved = 34;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeRemoved);
			_repo.AddTableEntities(tableName, seedData);
			var dt = DateTime.UtcNow;
			await Task.Delay(1000);
			seedData = EntityFactory.GetVariedSeedData(53);
			_repo.AddTableEntities(tableName, seedData);


			var tableQueryBuilder =
				new QueryBuilder().AppendCondition(ODataPredefinedFilters.TableNameExact(tableName));
			var dataQueryBuilder =
				new QueryBuilder().AppendCondition(ODataPredefinedFilters.TimestampLessThanOrEqual(dt));

			// Act
			var resp = await _sut.ArchiveDataFromTablesAsync<TableEntity>(
				tableQueryBuilder,
				dataQueryBuilder);

			// Assert
			foreach (var item in resp)
			{
				Assert.Equal(204, item.Status);
			}
			Assert.Equal(numOfEntriesThatWillBeRemoved, resp.Count);

			// Clean up
			_repo.DeleteTable(table);
			var archiveTable = _repo
				.GetTables(ODataPredefinedFilters.TableNameExact(TablePrefixes.ArchiveTablePrefix + tableName));
			_repo.DeleteTable(archiveTable.First());
		}

		[Fact]
		public async Task ArchiveDataFromTablesAsync_SingleTableAndSampleDataToRemoveUsingExpressionFilter_SuccessfullyRemovesData()
		{
			// Arrange
			var prefix = "RmvUsingExprFil";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeRemoved = 52;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeRemoved);
			_repo.AddTableEntities(tableName, seedData);
			var dt = DateTime.UtcNow;
			await Task.Delay(1000);
			seedData = EntityFactory.GetVariedSeedData(63);
			_repo.AddTableEntities(tableName, seedData);

			Expression<Func<TableItem, bool>> tableQuery = x => x.Name == tableName;
			Expression<Func<TableEntity, bool>> dataQuery = x => x.Timestamp.Value <= dt;

			// Act
			var resp = await _sut.ArchiveDataFromTablesAsync<TableEntity>(
				tableQuery,
				dataQuery);

			// Assert
			foreach (var item in resp)
			{
				Assert.Equal(204, item.Status);
			}
			Assert.Equal(numOfEntriesThatWillBeRemoved, resp.Count);

			// Clean up
			_repo.DeleteTable(table);
			var archiveTable = _repo
				.GetTables(ODataPredefinedFilters.TableNameExact(TablePrefixes.ArchiveTablePrefix + tableName));
			_repo.DeleteTable(archiveTable.First());
		}
	}
}
