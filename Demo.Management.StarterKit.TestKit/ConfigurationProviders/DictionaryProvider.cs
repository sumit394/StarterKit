using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace StarterKit.Test.ConfigurationProviders
{
	public class DictionaryProvider : IConfigurationProvider
	{
		private readonly IDictionary<string, string> _dictionary;

		public DictionaryProvider(IDictionary<string, string> dictionary)
		{
			_dictionary = dictionary.ToDictionary(x=> x.Key.ToUpperInvariant(), x=> x.Value);
		}

		public bool TryGet(string key, out string value)
		{
			return _dictionary.TryGetValue(key.ToUpperInvariant(), out value);
		}

		public void Set(string key, string value)
		{
			if (!_dictionary.ContainsKey(key))
				_dictionary.Add(key, value);
		}

		public IChangeToken GetReloadToken()
		{
			return new DictionaryChangeToken();
		}

		public void Load()
		{

		}

		public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
		{
			var keyToLookUp = $"{parentPath}:";
			if (!earlierKeys.Any())
			{
				var keys = _dictionary.Keys.Where(x =>
					x.StartsWith(keyToLookUp, StringComparison.InvariantCultureIgnoreCase));
				return keys.Select(x => x.Replace(keyToLookUp, string.Empty));
			}

			return new List<string>();
		}
	}
}