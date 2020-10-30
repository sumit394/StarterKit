using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Management.StarterKit.IntegrationTests
{
	public class AssertExtension
	{
		public static void AssertStatusCode(HttpStatusCode expected, HttpStatusCode actual, string body, ITestOutputHelper testOutputHelper)
		{
			try
			{
				Assert.Equal(expected, actual);
			}
			catch
			{
				testOutputHelper.WriteLine(body);
				throw;
			}
		}
	}
}