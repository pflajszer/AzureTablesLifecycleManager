using AzureTablesLifecycleManagement.AzureDAL.Models;
using AzureTablesLifecycleManager.Lib.Enums;
using System;

namespace AzureTablesLifecycleManager.Lib.Extensions
{
	public static class ConditionTypeExtensions
	{
		public static string DeriveLogicalOperator(this ConditionType ct)
		{
			switch (ct)
			{
				case ConditionType.Empty:
					return "";
				case ConditionType.And:
					return ODataLogicalOperators.And;
				case ConditionType.Or:
					return ODataLogicalOperators.Or;
				case ConditionType.Not:
					return ODataLogicalOperators.Not;
				default:
					throw new ArgumentException($"Logical operator {ct} not recognized.");
			}
		}
	}
}
