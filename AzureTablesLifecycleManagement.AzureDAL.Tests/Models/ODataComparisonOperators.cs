using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTablesLifecycleManagement.AzureDAL.Tests.Models
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/search/search-query-odata-comparison-operators
    /// </summary>
    public static class ODataComparisonOperators
    {
        /// <summary>
        /// Test whether a field is equal to a constant value
        /// </summary>
        public const string Equals = "eq";

        /// <summary>
        /// Test whether a field is not equal to a constant value
        /// </summary>
        public const string NotEquals = "ne";

        /// <summary>
        /// Test whether a field is greater than a constant value
        /// </summary>
        public const string GreaterThan = "gt";

        /// <summary>
        /// Test whether a field is less than a constant value
        /// </summary>
        public const string LessThan = "lt";

        /// <summary>
        /// Test whether a field is greater than or equal to a constant value
        /// </summary>
        public const string GreaterThanOrEqual = "ge";

        /// <summary>
        /// Test whether a field is less than or equal to a constant value
        /// </summary>
        public const string LessThanOrEqual = "le";
    }
}

