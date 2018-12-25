using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonkers
{
	static class UriExtensions
	{
		public static Uri Combine(this Uri uri, params string[] args)
		{
			Uri result = uri;

			foreach(var arg in args)
				result = new Uri(result, arg);

			return result;
		}
	}
}
