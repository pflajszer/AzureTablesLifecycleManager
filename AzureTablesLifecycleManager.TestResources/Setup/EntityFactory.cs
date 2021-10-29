using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AzureTablesLifecycleManager.TestResources.Setup
{
	public static class EntityFactory
	{
		public static Func<string, string> GenerateTableName => (prefix) =>
		{
			var rndName = string.Empty;
			var azureTableValidityPattern = @"^[A-Za-z][A-Za-z0-9]{2,62}$";
			var regex = new Regex(azureTableValidityPattern);
			do
			{
				rndName = prefix + Guid.NewGuid().ToString().Replace("-", "");
			}
			while (!regex.Match(rndName).Success);

			return rndName;
		};

		public static List<TableEntity> GetVariedSeedData(int elements)
		{
			Random r = new Random();
			var entities = new List<TableEntity>();
			var productTypes = new string[] { "Marker Set", "Rulers", "Pencils", "Other tools", "Dog food" };
			foreach (var item in Enumerable.Range(0, elements))
			{
				entities.Add(new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
																{
																	{ "Product", $"{productTypes[r.Next(0, productTypes.Length-1)]}" },
																	{ "Price", $"{r.NextDouble()}" },
																	{ "Quantity", $"{r.Next()}" }
																}); 
			}
			return entities;
		}

		public static Func<int, string> GenerateAlphaStringOfLength => (length) =>
		{
			var random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		};
	}
}
