using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Exceptions;
using AzureTablesLifecycleManager.Lib.Extensions;
using AzureTablesLifecycleManager.Lib.Services;
using AzureTablesLifecycleManager.Models;
using AzureTablesLifecycleManager.TestResources;
using System.Threading.Tasks;
using Xunit;

namespace AzureTablesLifecycleManager.Lib.Tests.Extensions
{
	public class DataTransferResponseExtensionsTests
	{

		private readonly TableManager _tableMgr;
		private readonly TableRepository _repo;
		public DataTransferResponseExtensionsTests()
		{
			var client = new TableServiceClient(ConfigConstants.ConnectionString);
			_repo = new TableRepository(client);
			_tableMgr = new TableManager(_repo);
		}


		[Fact]
		public void AreOKResponses_EmptyObjectPassed_Success()
		{
			// Arrange
			var resp = new DataTransferResponse<TableItem>();

			// Act
			var result = resp.AreOKResponses();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task EnsureCorrectResponses_StateUnderTest_ExpectedBehavior()
		{
			var invalidName = "0asdasdasdas";
			var q = new QueryBuilder();
			var resp = await _tableMgr.CopyDataFromTablesAsync<TableEntity>(q, q, invalidName);

			// Act & Assert
			Assert.ThrowsAny<TransferNotSuccessfulException>(() => resp.EnsureCorrectResponses());

		}
	}
}
