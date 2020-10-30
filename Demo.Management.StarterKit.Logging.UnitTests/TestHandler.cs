using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace Demo.Management.StarterKit.Logging.UnitTests
{
    public class TestHandler : HttpClientHandler
	{
		private readonly Func<HttpRequestMessage,
			CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

		public TestHandler()
		{
			_handlerFunc = (r, c) => Return200();
		}

		public TestHandler(Func<HttpRequestMessage,
			CancellationToken, Task<HttpResponseMessage>> handlerFunc)
		{
			_handlerFunc = handlerFunc;
		}

		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return _handlerFunc(request, cancellationToken);
		}

		public static Task<HttpResponseMessage> Return200()
		{
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}

		public static Task<HttpResponseMessage> ReturnJsonContent(object json)
		{
			var resp = new HttpResponseMessage(HttpStatusCode.OK);
			resp.Content = new StringContent(JsonConvert.SerializeObject(json));
			resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			return Task.FromResult(resp);
		}
	}
}