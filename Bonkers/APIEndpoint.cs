using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Bonkers
{
	public class APIEndpoint
	{
		public delegate T ResponseProcessMethod<T>(HttpRequestMessage request, HttpResponseMessage response);

		public Uri FullUrl { get; internal set; }

		private HttpClient _client;
		private HttpRequestMessage _request;
		private string _relativeUrl;

		private Dictionary<string, string> _requestHeaders;
		private Dictionary<string, string> _requestContentHeaders;
		private HttpContent _requestContent;

		internal APIEndpoint()
		{
			_client = new HttpClient();
			_requestContentHeaders = new Dictionary<string, string>();
			_requestHeaders = new Dictionary<string, string>();
		}

		/// <summary>
		/// Combines two or more endpoints together
		/// </summary>
		/// <param name="url">Endpoint URL</param>
		/// <returns></returns>
		public APIEndpoint Combine(params string[] url)
		{
			var endpoint = new APIEndpoint();
			endpoint.FullUrl = endpoint.FullUrl.Combine(url);

			return endpoint;
		}

		/// <summary>
		/// Duplicate the endpoint
		/// </summary>
		/// <returns></returns>
		public APIEndpoint Duplicate()
		{
			return new APIEndpoint
			{
				FullUrl = FullUrl,
				_client = _client,
				_request = _request
			};
		}

		/// <summary>
		/// Set the default parameters on the endpoint
		/// </summary>
		/// <returns></returns>
		public APIEndpoint Reset()
		{
			_request = null;
			_requestContentHeaders.Clear();
			return this;
		}

		/// <summary>
		/// Set a header value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public APIEndpoint Header(string name, string value)
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
		public APIEndpoint Body(string body, Encoding encoding = null)
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
		public APIEndpoint Stream(Stream stream, int bufferSize = 0)
		{
			if (bufferSize == 0 || bufferSize < 0)
				_request.Content = new StreamContent(stream);
			else
				_request.Content = new StreamContent(stream, bufferSize);

			return this;
		}

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
