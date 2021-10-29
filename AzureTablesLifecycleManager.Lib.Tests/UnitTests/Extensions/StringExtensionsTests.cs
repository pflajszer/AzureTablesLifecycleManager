using AzureTablesLifecycleManager.Lib.Extensions;
using AzureTablesLifecycleManager.TestResources.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzureTablesLifecycleManager.Lib.Tests.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
		public StringExtensionsTests()
		{

		}

		[Fact]
		public void IsValidAzureTableName_ValidName_SuccessfullyValidates()
		{
			// Arrange:
			var name = "ThisIsValidTableName";

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.True(result);

		}

		[Fact]
		public void IsValidAzureTableName_SecondCharIsNumber_Valid()
		{
			// Arrange:
			var name = "s0metablename";

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.True(result);

		}

		[Fact]
		public void IsValidAzureTableName_StartsWithNumber_NotValid()
		{
			// Arrange:
			var name = "0sometablename";

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.False(result);

		}

		[Fact]
		public void IsValidAzureTableName_LengthEquals63_Valid()
		{
			// Arrange:
			var len = 63;
			var name = EntityFactory.GenerateAlphaStringOfLength(len);

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.Equal(len, name.Length);
			Assert.True(result);

		}

		[Fact]
		public void IsValidAzureTableName_LengthEquals3_Valid()
		{
			// Arrange:
			var len = 3;
			var name = EntityFactory.GenerateAlphaStringOfLength(len);

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.Equal(len, name.Length);
			Assert.True(result);
		}

		[Fact]
		public void IsValidAzureTableName_LengthEquals64_Invalid()
		{
			// Arrange:
			var len = 64;
			var name = EntityFactory.GenerateAlphaStringOfLength(len);

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.Equal(len, name.Length);
			Assert.False(result);
		}

		[Fact]
		public void IsValidAzureTableName_LengthEquals2_Invalid()
		{
			// Arrange:
			var len = 2;
			var name = EntityFactory.GenerateAlphaStringOfLength(len);

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.Equal(len, name.Length);
			Assert.False(result);
		}

		[Fact]
		public void IsValidAzureTableName_ContainsProhibitedChar_Invalid()
		{
			// Arrange:
			var name = "Some-Table";

			// Act:
			var result = name.IsValidAzureTableName();

			// Assert:
			Assert.False(result);
		}


	}
}
