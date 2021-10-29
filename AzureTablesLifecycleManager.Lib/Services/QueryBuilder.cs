using AzureTablesLifecycleManager.Lib.Enums;
using AzureTablesLifecycleManager.Lib.Extensions;
using System.Linq;
using System.Text;

namespace AzureTablesLifecycleManager.Lib.Services
{
	public class QueryBuilder : IQueryBuilder
	{
		private readonly StringBuilder _sb;

		public QueryBuilder()
		{
			_sb = new StringBuilder();
		}


		public IQueryBuilder AppendCondition(string condition)
		{
			AddWhitespaceIfNecessary();
			_sb.Append(condition);
			return this;
		}

		public IQueryBuilder And()
		{
			AddWhitespaceIfNecessary();
			_sb.Append(ConditionType.And.DeriveLogicalOperator());
			return this;
		}

		public IQueryBuilder Or()
		{
			AddWhitespaceIfNecessary();
			_sb.Append(ConditionType.Or.DeriveLogicalOperator());
			return this;
		}

		public IQueryBuilder Not()
		{
			AddWhitespaceIfNecessary();
			_sb.Append(ConditionType.Not.DeriveLogicalOperator());
			return this;
		}

		public IQueryBuilder StartSubCondition()
		{
			AddWhitespaceIfNecessary();
			_sb.Append('(');
			return this;
		}

		public IQueryBuilder EndSubCondition()
		{
			_sb.Append(')');
			return this;
		}

		public string Build()
		{
			return _sb.ToString();
		}

		public void Flush()
		{
			_sb.Clear();
		}

		private void AddWhitespaceIfNecessary()
		{
			if (_sb.Length != 0 && _sb.ToString().Last() != '(')
			{
				_sb.Append(' ');
			}
		}



	}
}
