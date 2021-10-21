using Azure;
using Azure.Data.Tables.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.AzureDAL.APIGateway
{
	public interface ITablesAPI
	{
		Response<TableItem> CreateTable(string tableName);
		Response DeleteTable(TableItem table);
		Task<Response> DeleteTableAsync(TableItem table);
		Pageable<TableItem> GetAllTables();
		Pageable<TableItem> GetTablesUsingFilter(Expression<Func<TableItem, bool>> filter);
		Pageable<TableItem> GetTablesUsingFilter(string filter);
	}
}