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
			return new Uri(uri.ToString() + string.Join(string.Empty, args));
		}
	}
}
