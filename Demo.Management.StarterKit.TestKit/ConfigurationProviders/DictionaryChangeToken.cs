using System;
using Microsoft.Extensions.Primitives;

namespace StarterKit.Test.ConfigurationProviders
{
	public class DictionaryChangeToken : IChangeToken
	{
		public IDisposable RegisterChangeCallback(Action<object> callback, object state)
		{
			return null;
		}

		public bool HasChanged => false;
		public bool ActiveChangeCallbacks => false;
	}
}