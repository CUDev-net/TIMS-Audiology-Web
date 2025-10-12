using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TIMS_X.Core.Services
{
	public interface IRequestService
	{
		Task GetAsync(string uri, string token = "");
		Task<TResult> GetAsync<TResult>(string uri, string token = "");
		Task<TResult> GetAsync<TResult>(string uri, Action<HttpClient> options, string token = "");
		Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "");

		Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "");

		Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, Action<HttpClient> options,
			string token = "");

	}
}