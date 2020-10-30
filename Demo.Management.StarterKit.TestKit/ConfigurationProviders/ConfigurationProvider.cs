using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace StarterKit.Test.ConfigurationProviders
{
	public class DictionarySource : IConfigurationSource
	{
		private readonly IDictionary<string, string> _dictionary;

		public DictionarySource(IDictionary<string, string> dictionary)
		{
			_dictionary = dictionary;
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new DictionaryProvider(_dictionary);
		}
	}
}
