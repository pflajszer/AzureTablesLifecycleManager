using Azure;
using Azure.Data.Tables;
using System;

namespace AzureTablesLifecycleManager.TestResources.Models
{
	public class ProductEntity : ITableEntity
	{
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string Product { get; set; }
		public string Price { get; set; }
		public string Quantity { get; set; }
	}
}
