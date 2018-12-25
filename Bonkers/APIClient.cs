using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bonkers
{
	/*
		 var api = new ApiClient("https://api.hitbox.tv/");
		 var user = api.CreateEndpoint("user/");
		 var getUser = user.Combine(username);
		 var userData = getUser.Get<HitboxUser>();
	 */

	public class ApiClient
    {
		private Uri _url;
		private HttpClient _client;
	
		/// <summary>
		/// Create a new instance of APIClient
		/// </summary>
		/// <param name="api">Base API URL</param>
		public ApiClient(string url) : this(new Uri(url))
		{

		}

		/// <summary>
		/// Create a new instance of APIClient
		/// </summary>
		/// <param name="api">Base API URL</param>
		public ApiClient(Uri url)
		{
			_url = url;
			_client = new HttpClient();
		}

		/// <summary>
		/// Create an API endpoint
		/// </summary>
		/// <param name="url">Endpoint URL</param>
		/// <returns></returns>
		public ApiEndpoint CreateEndpoint(string url)
		{
			return new ApiEndpoint(_client, _url.Combine(url));
		}
    }
}
