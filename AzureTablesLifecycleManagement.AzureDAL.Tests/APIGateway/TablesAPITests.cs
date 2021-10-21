using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManagement.AzureDAL.Tests.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.TestResources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace AzureTablesLifecycleManagement.AzureDAL.Tests.APIGateway
{
	public class TablesAPITests
	{
		private readonly ITablesAPI _sut;

		private Func<string> GenerateTableName => () => "ABC" + DateTime.Now.Ticks.ToString();

		public TablesAPITests()
		{
			
			var client = new TableServiceClient(ConfigConstants.ConnectionString);
			_sut = new TablesAPI(client);
		}


		[Fact]
		public void GetTablesUsingFilter_ExactTableNameGivenUsingOData_FoundTable()
		{

			// Arrange
			var tableName = GenerateTableName();
			var table = _sut.CreateTable(tableName);
			string filter = $"TableName  {ODataComparisonOperators.Equals} '{tableName}'";

			// Act
			var result = _sut.GetTablesUsingFilter(
				filter).ToList();

			// Assert
			Assert.Equal(tableName, result.Single().Name);

			// Clean up
			_sut.DeleteTable(result.First());

		}

		[Fact]
		public void GetTablesUsingFilter_ExactTableNameGivenUsingExpression_FoundTable()
		{
			// Arrange
			var tableName = GenerateTableName();
			var table = _sut.CreateTable(tableName);
			Expression<Func<TableItem, bool>> filter = x => x.Name == tableName;

			// Act
			var result = _sut.GetTablesUsingFilter(
				filter).ToList();

			// Assert
			Assert.Equal(tableName, result.Single().Name);

			// Clean up
			_sut.DeleteTable(result.First());
		}

		[Fact]
		public void GetAllTables_HasTables_FindsAnyTable()
		{
			// Arrange
			var tableName = GenerateTableName();
			var dummyTable = _sut.CreateTable(tableName);

			// Act
			var result = _sut.GetAllTables().ToList();

			// Assert
			Assert.True(result.Count > 0);

			// Clean up

			_sut.DeleteTable(dummyTable);
		}

		[Fact]
		public void CreateTable_ValidNameGiven_SuccessfullyCreates()
		{
			// Arrange
			string tableName = GenerateTableName();
			var existingTable = _sut.GetTablesUsingFilter(x => x.Name == tableName);
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
		public void DeleteTable_ExistingTableNameGiven_SuccessfullyDeletes()
		{
			// Arrange
			var tableName = GenerateTableName();
			var table = _sut.CreateTable(tableName);

			// Act
			var result = _sut.DeleteTable(
				table);

			// Assert
			Assert.Equal(204, result.Status);

			// Clean up
			_sut.DeleteTable(table);
		}

		[Fact]
		public async Task DeleteTableAsync_ExistingTableGiven_SuccessfullyDeletes()
		{
			var tableName = GenerateTableName();
			var table = _sut.CreateTable(tableName);

			// Act
			var result = await _sut.DeleteTableAsync(
				table);

			// Assert
			Assert.Equal(204, result.Status);

			// Clean up
			_sut.DeleteTable(table);
		}
	}
}
