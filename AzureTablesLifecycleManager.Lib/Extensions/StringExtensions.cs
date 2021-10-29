using System.Text.RegularExpressions;

namespace AzureTablesLifecycleManager.Lib.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Table names must conform to these rules:
		/// - Table names may contain only alphanumeric characters.
		/// - A table name may not begin with a numeric character.
		/// - Table names are case-insensitive.
		/// - Table names must be from 3 through 63 characters long.
		/// Source: https://social.technet.microsoft.com/Forums/azure/en-US/312a3cff-4bfb-48d0-876a-71b35a769940/azure-table-name-rules
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool IsValidAzureTableName(this string name)
		{
			var azureTableValidityPattern = @"^[A-Za-z][A-Za-z0-9]{2,62}$";
			var regex = new Regex(azureTableValidityPattern);
			var isValid = regex.Match(name).Success;
			return isValid;

		}
	}
}
