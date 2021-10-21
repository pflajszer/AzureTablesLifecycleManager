using Azure.Data.Tables.Models;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManager.BRL.Services
{
    public class TableInquisitor
    {
		private readonly ITablesAPI _tables;
		//private readonly ITableEntitiesAPI _entities;
		public TableInquisitor(ITablesAPI tables /*, ITableEntitiesAPI entities*/)
		{
			_tables = tables;
			//_entities = entities;
		}

		public void DeleteTablesWithPrefix(string prefix)
		{
			var tables = _tables.GetTablesUsingFilter(HasPrefix("abc"));
			foreach (var table in tables)
			{
				_tables.DeleteTable(table); 
			}
		}

		public static Expression<Func<TableItem, bool>> HasPrefix(string prefix)
		{
			var nextASCIIChar = (char)(prefix.Last() + 1);
			return e => e.Name.CompareTo(prefix) > 0 && e.Name.CompareTo(prefix + 'd') <= 0; // how to query this. also add hassuffix
		}


	}
}
