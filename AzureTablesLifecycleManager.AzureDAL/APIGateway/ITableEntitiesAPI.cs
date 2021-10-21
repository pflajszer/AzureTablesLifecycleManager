using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.AzureDAL.APIGateway
{
	public interface ITableEntitiesAPI
	{
		IEnumerable<Response> DeleteEntitiesFromTable(IEnumerable<TableEntity> entities);
		Task<IEnumerable<Response>> DeleteEntitiesFromTableAsync(IEnumerable<TableEntity> entities);
		Response DeleteEntityFromTable(TableEntity entity);
		Task<Response> DeleteEntityFromTableAsync(TableEntity entity);
		Pageable<TableEntity> Query(Expression<Func<TableEntity, bool>> filter);
		Pageable<TableEntity> Query(string filter);
	}
}