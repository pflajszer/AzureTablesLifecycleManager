using System;

namespace AzureTablesLifecycleManager.Lib.Exceptions
{
	public class InvalidAzureTableNameException : Exception
	{
		public InvalidAzureTableNameException() { }
		public InvalidAzureTableNameException(string? message) : base(message) { }
		public InvalidAzureTableNameException(string? message, Exception? innerException) : base(message, innerException) { }
	}
}