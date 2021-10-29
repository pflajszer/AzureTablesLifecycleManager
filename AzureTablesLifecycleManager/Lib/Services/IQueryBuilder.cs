
namespace AzureTablesLifecycleManager.Lib.Services
{
	/// <summary>
	/// Query Builder using OData syntax
	/// </summary>
	public interface IQueryBuilder
	{
		IQueryBuilder AppendCondition(string condition);
		IQueryBuilder StartSubCondition();
		IQueryBuilder EndSubCondition();
		public IQueryBuilder And();
		public IQueryBuilder Or();
		public IQueryBuilder Not();
		public string Build();
		void Flush();
	}
}