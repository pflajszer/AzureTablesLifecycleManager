using Azure;
using Azure.Data.Tables.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AzureTablesLifecycleManager.Models
{
	public class DataTransferResponse<T>
	{
		public DataTransferResponse()
		{
			DataAddedResponses = new List<Response>();
			DataUpdatedResponses = new List<Response>();
			DataDeletedResponses = new List<Response>();
			TableAddedResponses = new List<Response<TableItem>>();
			TableDeletedResponses = new List<Response>();
			DataFilterResults = new Dictionary<string, AsyncPageable<T>>();
			IsNewTableNameValid = true;
		}

		public string ODataTableQueryBuilt { get; set; }
		public string ODataDataQueryBuilt { get; set; }

		public Expression<Func<TableItem, bool>> ExpressionTableQuery { get; set; }
		public Expression<Func<T, bool>> ExpressionDataQuery { get; set; }

		public bool IsNewTableNameValid { get; set; }

		public AsyncPageable<TableItem> TablesFilterResults { get; set; }
		public IDictionary<string, AsyncPageable<T>> DataFilterResults { get; set; }
		public List<Response<TableItem>> TableAddedResponses { get; set; }
		public List<Response> TableDeletedResponses { get; set; }
		public List<Response> DataAddedResponses { get; set; }
		public List<Response> DataDeletedResponses { get; set; }
		public List<Response> DataUpdatedResponses { get; set; }

	}
}
