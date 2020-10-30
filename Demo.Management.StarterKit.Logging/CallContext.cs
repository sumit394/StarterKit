using System.Collections.Concurrent;
using System.Threading;

namespace StarterKit.Logging
{
    public static class CallContext<T>
	{
		private static readonly ConcurrentDictionary<string, AsyncLocal<T>> State =
			new ConcurrentDictionary<string, AsyncLocal<T>>();

		public static void SetData(string name, T data) =>
			State.GetOrAdd(name, _ => new AsyncLocal<T>()).Value = data;

		public static T GetData(string name) =>
			State.TryGetValue(name, out AsyncLocal<T> data) ? data.Value : default(T);
	}
}