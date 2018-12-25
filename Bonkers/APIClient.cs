using System;
using System.Collections.Generic;
using System.Linq;
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

	public class APIClient
    {
		private Uri _url;
	
		/// <summary>
		/// Create a new instance of APIClient
		/// </summary>
		/// <param name="api">Base API URL</param>
		public APIClient(Uri api)
		{
			_url = api;
		}

		/// <summary>
		/// Create an API endpoint
		/// </summary>
		/// <param name="url">Endpoint URL</param>
		/// <returns></returns>
		public APIEndpoint CreateEndpoint(string url)
		{
			var endpoint = new APIEndpoint();
			endpoint.FullUrl = new Uri(_url.ToString() + url);

			return endpoint;
		}
    }
}
