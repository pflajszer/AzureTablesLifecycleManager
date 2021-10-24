using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureTablesLifecycleManager.TestResources.Setup
{
	public static class EntityFactory
	{
		public static Func<string, string> GenerateTableName => (prefix) => prefix + DateTime.Now.Ticks.ToString();

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
