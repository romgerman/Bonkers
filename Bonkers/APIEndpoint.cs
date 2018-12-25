using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;

namespace Bonkers
{
	public class ApiEndpoint
	{
		public delegate T ResponseProcessMethod<T>(HttpRequestMessage request, HttpResponseMessage response);

		public Uri FullUrl { get; internal set; }

		private HttpClient _client;
		private HttpRequestMessage _request;
		private string _relativeUrl;

		private Dictionary<string, string> _requestHeaders;
		private Dictionary<string, string> _requestContentHeaders;
		private HttpContent _requestContent;

		internal ApiEndpoint(HttpClient client, Uri url)
		{
			FullUrl = url;
			_client = client;
			_requestContentHeaders = new Dictionary<string, string>();
			_requestHeaders = new Dictionary<string, string>();
		}

		/// <summary>
		/// Combines two or more endpoints together
		/// </summary>
		/// <param name="url">Endpoint URL</param>
		/// <returns></returns>
		public ApiEndpoint Combine(params string[] url)
		{
			return new ApiEndpoint(_client, FullUrl.Combine(url));
		}

		/// <summary>
		/// Duplicate the endpoint
		/// </summary>
		/// <returns></returns>
		public ApiEndpoint Duplicate()
		{
			return new ApiEndpoint(_client, FullUrl)
			{
				_client = _client,
				_request = _request,
				_requestContent = _requestContent,
				_requestContentHeaders = _requestContentHeaders,
				_requestHeaders = _requestHeaders
			};
		}

		/// <summary>
		/// Set the default parameters on the endpoint
		/// </summary>
		/// <returns></returns>
		public ApiEndpoint Reset()
		{
			_request = null;
			_requestContentHeaders.Clear();
			return this;
		}

		/// <summary>
		/// Set a header value
		/// </summary>
		/// <param name="name">Name of header</param>
		/// <param name="value">Value of header</param>
		/// <returns></returns>
		public ApiEndpoint Header(string name, string value)
		{
			if (name.Equals("Content-Type") ||
				name.Equals("Content-Language") ||
				name.Equals("Content-Length") ||
				name.Equals("Content-Location") ||
				name.Equals("Content-MD5") ||
				name.Equals("Content-Range") ||
				name.Equals("Content-Type") ||
				name.Equals("Allow") ||
				name.Equals("Expires") ||
				name.Equals("LastModified"))
			{
				_requestContentHeaders.Add(name, value);
			}
			else
			{
				_requestHeaders.Add(name, value);
			}

			return this;
		}

		/// <summary>
		/// Sets a request body as a string
		/// </summary>
		/// <param name="body">Content</param>
		/// <param name="encoding">Content encoding</param>
		/// <returns></returns>
		public ApiEndpoint Body(string body, Encoding encoding = null)
		{
			_request.Content = new StringContent(body, encoding ?? Encoding.UTF8);
			return this;
		}

		/// <summary>
		/// Streams content to request
		/// </summary>
		/// <param name="stream">Input stream</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <returns></returns>
		public ApiEndpoint Stream(Stream stream, int bufferSize = 0)
		{
			if (bufferSize == 0 || bufferSize < 0)
				_request.Content = new StreamContent(stream);
			else
				_request.Content = new StreamContent(stream, bufferSize);

			return this;
		}

		/// <summary>
		/// Add parameters to query string
		/// </summary>
		/// <param name="name">Name of parameter</param>
		/// <param name="value">Value of parameter</param>
		/// <returns></returns>
		public ApiEndpoint Params(QueryString @params)
		{
			string url = "?";

			foreach(var param in @params)
				url += $"{param.Key}={param.Value}&";

			url = url.Remove(url.Length - 1, 1);

			FullUrl = new Uri(FullUrl.ToString() + url);

			return this;
		}

		#region HTTP Methods

		/// <summary>
		/// Basic GET request
		/// </summary>
		/// <returns></returns>
		public async Task<string> Get()
		{
			CreateRequest(HttpMethod.Get);
			SetContentHeaders();

			return await (await _client.SendAsync(_request)).Content.ReadAsStringAsync();
		}

		/// <summary>
		/// GET request with specific process method
		/// </summary>
		/// <param name="process">Method which processes the response</param>
		/// <returns></returns>
		public async Task<T> Get<T>(ResponseProcessMethod<T> process)
		{
			CreateRequest(HttpMethod.Get);
			SetContentHeaders();

			var response = await _client.SendAsync(_request);
			return process(_request, response);
		}

		/// <summary>
		/// Basic POST request
		/// </summary>
		/// <returns></returns>
		public async Task<string> Post()
		{
			CreateRequest(HttpMethod.Post);
			SetContentHeaders();

			System.Diagnostics.Debug.WriteLine(FullUrl);

			return await (await _client.SendAsync(_request)).Content.ReadAsStringAsync();
		}

		/// <summary>
		/// GET request with specific process method
		/// </summary>
		/// <param name="process">Method which processes the response</param>
		/// <returns></returns>
		public async Task<T> Post<T>(ResponseProcessMethod<T> process)
		{
			CreateRequest(HttpMethod.Post);
			SetContentHeaders();

			var response = await _client.SendAsync(_request);
			return process(_request, response);
		}

		#endregion HTTP Methods

		#region Helpers

		private void CreateRequest(HttpMethod method)
		{
			_request = new HttpRequestMessage(method, FullUrl);
			SetContentHeaders();

			// Set headers

			foreach(var header in _requestHeaders)
				_request.Headers.TryAddWithoutValidation(header.Key, header.Value);
		}

		private void SetContentHeaders()
		{
			if (_requestContentHeaders.Count == 0)
				return;

			if (_request.Content == null)
				_request.Content = new StringContent("");

			foreach(var header in _requestContentHeaders)
			{
				if (_request.Content.Headers.Contains(header.Key))
					_request.Content.Headers.Remove(header.Key);

				_request.Content.Headers.Add(header.Key, header.Value);
			}
		}

		#endregion Helpers
	}
}
