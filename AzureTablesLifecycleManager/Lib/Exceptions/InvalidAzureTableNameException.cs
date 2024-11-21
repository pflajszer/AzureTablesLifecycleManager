using System;

namespace AzureTablesLifecycleManager.Lib.Exceptions
{
	public class InvalidAzureTableNameException : Exception
	{
		public InvalidAzureTableNameException() { }
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public InvalidAzureTableNameException(string? message) : base(message) { }
        public InvalidAzureTableNameException(string? message, Exception? innerException) : base(message, innerException) { }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	}
}