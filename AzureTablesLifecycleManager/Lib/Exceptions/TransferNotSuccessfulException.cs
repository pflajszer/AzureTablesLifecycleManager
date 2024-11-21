using System;

namespace AzureTablesLifecycleManager.Lib.Exceptions
{
	public class TransferNotSuccessfulException : Exception
	{
		public TransferNotSuccessfulException() { }
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
		public TransferNotSuccessfulException(string? message) : base(message) { }
        public TransferNotSuccessfulException(string? message, Exception? innerException) : base(message, innerException) { }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}