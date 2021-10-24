using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.AzureDAL.Models
{
    public class TableEntityFields
    {
        public const string TableName = "TableName";
        public const string PartitionKey = "PartitionKey";
        public const string RowKey = "RowKey";
        public const string ETag = "ETag";
        public const string Timestamp = "Timestamp";
	}
}
