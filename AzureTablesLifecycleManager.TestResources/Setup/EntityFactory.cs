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
			string pattern = @"^[A-Za-z][A-Za-z0-9]{2,62}$";
			// Create a Regex  
			var rg = new Regex(pattern);
			do
			{
				rndName = prefix + Guid.NewGuid().ToString().Replace("-", "");
			}
			while (!rg.Match(rndName).Success);

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
	}
}
