using System;
using System.Collections.Generic;

namespace Bonkers
{
	public class QueryString : Dictionary<string, string>
	{
		public QueryString()
		{

		}
		
		public QueryString(string queryString)
		{
			queryString = queryString.Trim();

			if (queryString.Length < 1)
				return;

			if (queryString[0] == '?')
				queryString = queryString.Remove(0, 1);

			string[] @params = queryString.Split('&');

			foreach (var param in @params)
			{
				var kvp = param.Split('=');
				this.Add(kvp[0], kvp[1]);
			}
		}

		public string Get(string name)
		{
			string val;
			this.TryGetValue(name, out val);

			return val;
		}
	}
}
