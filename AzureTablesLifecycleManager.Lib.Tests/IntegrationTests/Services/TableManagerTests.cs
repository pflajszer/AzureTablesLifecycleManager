using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Extensions;
using AzureTablesLifecycleManager.Lib.Services;
using AzureTablesLifecycleManager.Models;
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
			Assert.True(resp.AreOKResponses());
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
			Assert.True(resp.AreOKResponses());
			Assert.Equal(10, resp.TableDeletedResponses.Count);

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
			Assert.True(resp.AreOKResponses());
			Assert.Equal(numOfEntriesThatWillBeRemoved, resp.DataDeletedResponses.Count);

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
			Assert.True(resp.AreOKResponses());
			Assert.Equal(numOfEntriesThatWillBeRemoved, resp.DataDeletedResponses.Count);

			// Clean up
			_repo.DeleteTable(table);
		}

		[Fact]
		public async Task MoveDataBetweenTablesAsync_SingleTableAndSampleDataToTransferUsingODataFilter_SuccessfullyMoveData()
		{
			// Arrange
			var prefix = "MoveUsiODataFilt";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var newTableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeTransferred = 34;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeTransferred);
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
			var resp = await _sut.MoveDataBetweenTablesAsync<TableEntity>(
				tableQueryBuilder,
				dataQueryBuilder,
				newTableName);

			// Assert
			Assert.True(resp.AreOKResponses());
			Assert.Equal(numOfEntriesThatWillBeTransferred, resp.DataDeletedResponses.Count);
			Assert.Equal(numOfEntriesThatWillBeTransferred, resp.DataAddedResponses.Count);

			// Clean up
			_repo.DeleteTable(table);
			var archiveTable = _repo
				.GetTables(ODataPredefinedFilters.TableNameExact(newTableName));
			_repo.DeleteTable(archiveTable.First());
		}

		[Fact]
		public async Task MoveDataBetweenTablesAsync_SingleTableAndSampleDataToTransferUsingExpressionFilter_SuccessfullyMovesData()
		{
			// Arrange
			var prefix = "MoveUsingExprFil";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var newTableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeTransferred = 52;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeTransferred);
			_repo.AddTableEntities(tableName, seedData);
			var dt = DateTime.UtcNow;
			await Task.Delay(1000);
			seedData = EntityFactory.GetVariedSeedData(63);
			_repo.AddTableEntities(tableName, seedData);

			Expression<Func<TableItem, bool>> tableQuery = x => x.Name == tableName;
			Expression<Func<TableEntity, bool>> dataQuery = x => x.Timestamp.Value <= dt;

			// Act
			var resp = await _sut.MoveDataBetweenTablesAsync<TableEntity>(
				tableQuery,
				dataQuery,
				newTableName);

			// Assert
			Assert.True(resp.AreOKResponses());
			Assert.Equal(numOfEntriesThatWillBeTransferred, resp.DataDeletedResponses.Count);
			Assert.Equal(numOfEntriesThatWillBeTransferred, resp.DataAddedResponses.Count);

			// Clean up
			_repo.DeleteTable(table);
			var archiveTable = _repo
				.GetTables(ODataPredefinedFilters.TableNameExact(newTableName));
			_repo.DeleteTable(archiveTable.First());
		}

		[Fact]
		public async Task MoveDataFromTablesAsync_SingleTableAndSampleDataToCopyUsingODataFilter_SuccessfullyCopiesData()
		{
			// Arrange
			var prefix = "CopyUsiODataFilt";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var newTableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeTransferred = 34;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeTransferred);
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
			var resp = await _sut.MoveDataBetweenTablesAsync<TableEntity>(
				tableQueryBuilder,
				dataQueryBuilder,
				newTableName);

			// Assert
			Assert.True(resp.AreOKResponses());
			Assert.Equal(numOfEntriesThatWillBeTransferred, resp.DataDeletedResponses.Count);
			Assert.Equal(numOfEntriesThatWillBeTransferred, resp.DataAddedResponses.Count);

			// Clean up
			_repo.DeleteTable(table);
			var archiveTable = _repo
				.GetTables(ODataPredefinedFilters.TableNameExact(newTableName));
			_repo.DeleteTable(archiveTable.First());
		}

		[Fact]
		public async Task CopyDataFromTablesAsync_SingleTableAndSampleDataToCopyUsingExpressionFilter_SuccessfullyCopiedData()
		{
			// Arrange
			var prefix = "CopyUsingExprFil";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var newTableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesThatWillBeCopied = 52;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesThatWillBeCopied);
			_repo.AddTableEntities(tableName, seedData);
			var dt = DateTime.UtcNow;
			await Task.Delay(1000);
			seedData = EntityFactory.GetVariedSeedData(63);
			_repo.AddTableEntities(tableName, seedData);

			Expression<Func<TableItem, bool>> tableQuery = x => x.Name == tableName;
			Expression<Func<TableEntity, bool>> dataQuery = x => x.Timestamp.Value <= dt;

			// Act
			var resp = await _sut.CopyDataFromTablesAsync<TableEntity>(
				tableQuery,
				dataQuery,
				newTableName);

			// Assert
			Assert.True(resp.AreOKResponses());
			Assert.Equal(0, resp.DataDeletedResponses.Count);
			Assert.Equal(numOfEntriesThatWillBeCopied, resp.DataAddedResponses.Count);

			// Clean up
			_repo.DeleteTable(table);
			var archiveTable = _repo
				.GetTables(ODataPredefinedFilters.TableNameExact(newTableName));
			_repo.DeleteTable(archiveTable.First());
		}

		[Fact]
		public async Task GetDataFromTablesAsync_SingleTableAndSampleDataToGetUsingExpressionFilter_SuccessfullyFetches()
		{
			// Arrange
			var prefix = "CopyUsingExprFil";
			var tableName = EntityFactory.GenerateTableName(prefix);
			var table = _repo.CreateTable(tableName);
			int numOfEntriesFetched = 23;
			var seedData = EntityFactory.GetVariedSeedData(numOfEntriesFetched);
			_repo.AddTableEntities(tableName, seedData);
			var dt = DateTime.UtcNow;
			await Task.Delay(1000);
			seedData = EntityFactory.GetVariedSeedData(43);
			_repo.AddTableEntities(tableName, seedData);

			Expression<Func<TableItem, bool>> tableQuery = x => x.Name == tableName;
			Expression<Func<TableEntity, bool>> dataQuery = x => x.Timestamp.Value <= dt;

			// Act
			var resp = await _sut.GetDataFromTablesAsync<TableEntity>(
				tableQuery,
				dataQuery);
			var tfResults = await resp.TablesFilterResults.EnumerateAsyncPageable();
			var dfResults = await resp.DataFilterResults[tableName].EnumerateAsyncPageable();

			// Assert
			Assert.True(resp.AreOKResponses());
			Assert.Equal(1, tfResults.Count);
			Assert.Equal(numOfEntriesFetched, dfResults.Count);

			// Clean up
			_repo.DeleteTable(table);
		}
	}
}
