using AzureTablesLifecycleManager.Lib.Exceptions;
using AzureTablesLifecycleManager.Models;
using Newtonsoft.Json;
using System.Linq;

namespace AzureTablesLifecycleManager.Lib.Extensions
{
	public static class DataTransferResponseExtensions
	{
		/// <summary>
		/// Determines if the transfer operations were successfull (201/204 response codes)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resp"></param>
		/// <returns></returns>
		public static bool AreOKResponses<T>(this DataTransferResponse<T> resp)
		{
			if (!resp.IsNewTableNameValid)
			{
				return false;
			}

			if (resp.TableAddedResponses.Any(x => x.GetRawResponse().Status != 201))
			{
				return false;
			}

			if (resp.TableDeletedResponses.Any(x => x.Status != 204))
			{
				return false;
			}

			if (resp.DataAddedResponses.Any(x => x.Status != 204))
			{
				return false;
			}

			if (resp.DataDeletedResponses.Any(x => x.Status != 204))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Throws `TransferNotSuccessfulException` when any of the responses are not indicating success
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resp"></param>
		public static void EnsureCorrectResponses<T>(this DataTransferResponse<T> resp)
		{
			if (!resp.AreOKResponses())
			{
				var json = JsonConvert.SerializeObject(resp);
				throw new TransferNotSuccessfulException(json);
			}
		}
	}
}