using AzureTablesLifecycleManagement.AzureDAL.Models;
using System;

namespace AzureTablesLifecycleManager.AzureDAL.Models
{
	public static class ODataPredefinedFilters
	{
		public static Func<string, string> TableNameExact => (tableName) => $"{TableEntityFields.TableName} {ODataComparisonOperators.Equals} '{tableName}'";
		public static Func<string, string> TableNamesWithPrefix => (tableName) => $"{TableEntityFields.TableName} {ODataComparisonOperators.Equals} '{tableName}'";
		public static Func<string, string> RowKeyExact => (rowKey) => $"{TableEntityFields.RowKey} {ODataComparisonOperators.Equals} '{rowKey}'";
		public static Func<string, string> PartitionKeyExact => (partitionKey) => $"{TableEntityFields.PartitionKey} {ODataComparisonOperators.Equals} '{partitionKey}'";
		public static Func<DateTime, string> TimestampGreaterThan => (timestamp) => $"{TableEntityFields.Timestamp} {ODataComparisonOperators.GreaterThan} '{timestamp.ToString("o")}'";
		public static Func<DateTime, string> TimestampGreaterThanOrEqual => (timestamp) => $"{TableEntityFields.Timestamp} {ODataComparisonOperators.GreaterThanOrEqual} '{timestamp.ToString("o")}'";
		public static Func<DateTime, string> TimestampLessThan => (timestamp) => $"{TableEntityFields.Timestamp} {ODataComparisonOperators.LessThan} '{timestamp.ToString("o")}'";
		public static Func<DateTime, string> TimestampLessThanOrEqual => (timestamp) => $"{TableEntityFields.Timestamp} {ODataComparisonOperators.LessThanOrEqual} '{timestamp.ToString("o")}'";
		public static Func<(string TableEntityField, string ComparisonOperator, string Value), string> Custom => (data) => $"{data.TableEntityField} {data.ComparisonOperator} '{data.Value}'";
	}
}
