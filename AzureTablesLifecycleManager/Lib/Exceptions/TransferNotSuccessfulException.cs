using System;

namespace AzureTablesLifecycleManager.Lib.Exceptions
{
	public class TransferNotSuccessfulException : Exception
	{
		public TransferNotSuccessfulException() { }
		public TransferNotSuccessfulException(string? message) : base(message) { }
		public TransferNotSuccessfulException(string? message, Exception? innerException) : base(message, innerException) { }
	}
}