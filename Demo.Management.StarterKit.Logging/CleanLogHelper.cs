using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace StarterKit.Logging
{
    public static class CleanLogHelper
	{
		private static readonly List<string> HiddenHeaders = new List<string>()
		{
			"Cookie", "Set-Cookie", "Authorization", "X-Client-Id","X-Client-Secret", "X-Api-Key"
		};

		public static IDictionary<string, string> GetAllowedHeaders(this IHeaderDictionary headers)
		{
			return headers.ToDictionary(x => x.Key,
				x => HiddenHeaders.Contains(x.Key) ? HideValue(x.Value) : x.Value.ToString());
		}

		public static IDictionary<string, IEnumerable<string>> GetAllowedHeaders(this HttpRequestHeaders headers)
		{
			return headers.ToDictionary(x => x.Key,
				x => HiddenHeaders.Contains(x.Key) ? x.Value.Select(HideValue) : x.Value);
		}

		public static IDictionary<string, IEnumerable<string>> GetAllowedHeaders(this HttpResponseHeaders headers)
		{
			return headers.ToDictionary(x => x.Key,
				x => HiddenHeaders.Contains(x.Key) ? x.Value.Select(HideValue) : x.Value);
		}

		public static IDictionary<string, IEnumerable<string>> GetAllowedHeaders(this HttpContentHeaders headers)
		{
			return headers.ToDictionary(x => x.Key,
				x => HiddenHeaders.Contains(x.Key) ? x.Value.Select(HideValue) : x.Value);
		}


		private static string HideValue(string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			if (value.Length < 6)
				return "******";

			return $"{value.Substring(0, 3)}****************{value.Substring(value.Length-3,3)}";
		}
	}
}