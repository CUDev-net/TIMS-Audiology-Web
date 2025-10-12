using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using TIMS_X.Core.Exceptions;
using TIMS_X.Core.Services;
using TIMS_X.Core.Utils;

namespace TIMS_X.CoreServices
{
	public class RequestService : IRequestService
	{
		private readonly JsonSerializerSettings _serializerSettings;
		private readonly Stopwatch _timer;

		public RequestService()
		{
			_timer = new Stopwatch();
			_serializerSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				NullValueHandling = NullValueHandling.Ignore
			};

			_serializerSettings.Converters.Add(new StringEnumConverter());
		}

		public async Task GetAsync(string uri, string token = "")
		{
			await GetAsync<object>(uri, token);
		}

		public async Task<TResult> GetAsync<TResult>(string uri, string token = "")
		{
			var httpClient = CreateHttpClient(token);
			long timeElapsed = 0;
			var uriObject = new Uri(uri);
			long bytes = 0;
			TResult result;
			HttpStatusCode statusCode;
			try
			{
				ServicePointManager.SecurityProtocol =
					SecurityProtocolType.Tls |
					(SecurityProtocolType)768 | //TLS 1.1
					(SecurityProtocolType)3072; //TLS 1.2
				_timer.Start();
				{
					var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
					statusCode = response.StatusCode;
					await HandleResponse(response);
					var serialized = await response.Content.ReadAsStringAsync();
					if (response.Content.Headers.ContentLength != null)
						bytes = response.Content.Headers.ContentLength.Value;
					result = await Task.Run(() =>
						JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));
				}
				_timer.Stop();
				timeElapsed = _timer.ElapsedMilliseconds;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Request to {url} threw an exception", uri);
				throw;
			}


			Log.Information("[GET] {0:l} |{1:l}|{2:l}| {3}-{4}",
				uriObject.PathAndQuery.Split('?')[0],
				(timeElapsed + " ms").PadCenter(12, ' '),
				(bytes + " B").PadCenter(12, ' '),
				(int)statusCode,
				statusCode
			);

			return result;
		}


		public async Task<TResult> GetAsync<TResult>(string uri, Action<HttpClient> options, string token = "")
		{
			var httpClient = CreateHttpClient(token);
			options?.Invoke(httpClient);
			long timeElapsed = 0;
			var uriObject = new Uri(uri);
			long bytes = 0;
			TResult result;
			HttpStatusCode statusCode;
			try
			{
				ServicePointManager.SecurityProtocol =
					SecurityProtocolType.Tls |
					(SecurityProtocolType)768 | //TLS 1.1
					(SecurityProtocolType)3072; //TLS 1.2
				_timer.Start();
				{
					var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
					statusCode = response.StatusCode;
					await HandleResponse(response);
					var serialized = await response.Content.ReadAsStringAsync();
					if (response.Content.Headers.ContentLength != null)
						bytes = response.Content.Headers.ContentLength.Value;
					result = await Task.Run(() =>
						JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));
				}
				_timer.Stop();
				timeElapsed = _timer.ElapsedMilliseconds;
			}
			catch (Exception ex)
			{
				Log.Error(ex,
					$"Request to {uri} threw an exception: {ex.Message}, \r\n\tInner Exception: {ex.InnerException.Message}",
					uri, ex);
				throw;
			}


			Log.Information("[GET] {0:l} |{1:l}|{2:l}| {3}-{4}",
				uriObject.PathAndQuery.Split('?')[0],
				(timeElapsed + " ms").PadCenter(12, ' '),
				(bytes + " B").PadCenter(12, ' '),
				(int)statusCode,
				statusCode
			);

			return result;
		}

		public Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "")
		{
			return PostAsync<TResult, TResult>(uri, data, token);
		}

		public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "")
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Tls |
				(SecurityProtocolType)768 | //TLS 1.1
				(SecurityProtocolType)3072; //TLS 1.2
			var httpClient = CreateHttpClient(token);
			var serialized = await Task.Run(() => JsonConvert.SerializeObject(data, _serializerSettings));
			var response = await httpClient
				.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"))
				.ConfigureAwait(false);

			await HandleResponse(response);

			var responseData = await response.Content.ReadAsStringAsync();

			return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings));
		}

		public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, Action<HttpClient> options,
			string token = "")
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Tls |
				(SecurityProtocolType)768 | //TLS 1.1
				(SecurityProtocolType)3072; //TLS 1.2
			var httpClient = CreateHttpClient(token);
			options?.Invoke(httpClient);
			var serialized = await Task.Run(() => JsonConvert.SerializeObject(data, _serializerSettings))
				.ConfigureAwait(false);
			var response = await httpClient
				.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"))
				.ConfigureAwait(false);

			await HandleResponse(response);

			var responseData = await response.Content.ReadAsStringAsync();

			return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings));
		}

		private HttpClient CreateHttpClient(string token = "")
		{
			// We can enforce using a wifi connection and not allow mobile data.
			// Need to reference Xamarin.Essentials for this to work
			/*if (Connectivity.NetworkAccess != NetworkAccess.Internet)
			{
			    throw new ConnectivityException();
			}*/
			var handler = new HttpClientHandler();
//#if DEBUG

			handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true;
//#endif
			var httpClient = new HttpClient(handler);

			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			if (!string.IsNullOrEmpty(token))
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


			return httpClient;
		}

		private async Task HandleResponse(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();

				if (response.StatusCode == HttpStatusCode.Forbidden ||
				    response.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedUserException();

				throw new HttpRequestException(content);
			}
		}
	}
}