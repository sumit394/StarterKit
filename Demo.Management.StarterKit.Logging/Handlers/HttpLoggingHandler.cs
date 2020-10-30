using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.Logging.Handlers
{
    public class HttpLoggingHandler : DelegatingHandler
	{
		public const string Marker = "<removed>";
		public readonly string Name;
		readonly ILogger _logger;
		private readonly ICorrelationIdHandler _correlationIdHandler;

		public HttpLoggingHandler(string name, ILogger logger, ICorrelationIdHandler correlationIdHandler)
			: this(name, logger, correlationIdHandler, new HttpClientHandler())
		{

		}

		public HttpLoggingHandler(string name, ILogger logger, ICorrelationIdHandler correlationIdHandler, HttpMessageHandler handler)
		{
			Name = name;
			_logger = logger;
			_correlationIdHandler = correlationIdHandler;
			InnerHandler = handler;
		}

		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Log(request, cancellationToken);
		}

		public async Task<HttpResponseMessage> Log(
			HttpRequestMessage httpRequest, CancellationToken cancellationToken)
		{
				var sw = Stopwatch.StartNew();
				if (!httpRequest.Headers.Contains(CorrelationIdConstants.CorrelationId))
					httpRequest.Headers.Add(CorrelationIdConstants.CorrelationId, _correlationIdHandler.GetCorrelationId().ToString());
				var httpResponse = await base.SendAsync(httpRequest, cancellationToken);
				await TryLog(httpRequest, httpResponse, sw.ElapsedMilliseconds);

				return httpResponse;
		}

		private async Task TryLog(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage, long elapsedMilliseconds)
		{
			try
			{
				await LogRequest(requestMessage);
				await LogResponse(requestMessage, responseMessage, elapsedMilliseconds);
			}
			catch (Exception ex)
			{
				_logger.Error("Error parsing requestMessage/response for logging", ex);
			}
		}

		private async Task LogRequest(HttpRequestMessage request)
		{
			var isJson = IsContentJson(request.Content);
			var requestBody = request?.Content != null && isJson ? await request.Content?.ReadAsStringAsync() : string.Empty;

			var reqMsg = new RequestModel()
			{
				Method = request?.Method.Method,
				Uri = request?.RequestUri.AbsoluteUri,
				Headers = request?.Headers.GetAllowedHeaders(),
				Content = new ContentModel
				{
					Headers = request?.Content?.Headers.GetAllowedHeaders(),
					ContentBody = requestBody
				}
			};

			_logger.Information(Name + " {@reqMsg}", reqMsg);
		}

		private async Task LogResponse(HttpRequestMessage requestMessage,
			HttpResponseMessage responseMessage, 
			long elapsedMilliseconds)
		{
			var respMsg = new ResponseModel()
			{
				Method = requestMessage?.Method.Method,
				Uri = requestMessage?.RequestUri.AbsoluteUri,
				Headers = responseMessage.Headers.GetAllowedHeaders(),
				Content = new ContentModel()
				{
					Headers = responseMessage.Content?.Headers.GetAllowedHeaders(),
					ContentBody = responseMessage.Content != null ? await responseMessage.Content.ReadAsStringAsync() : string.Empty
				},
				StatusCode = (int)responseMessage.StatusCode
			};

			_logger.Information(Name + " {@respMsg},{elapsedMs}", respMsg, new[] { elapsedMilliseconds });
		}

		private bool IsContentJson(HttpContent content)
		 => content?.Headers?.ContentType?.MediaType?.ToLowerInvariant()
				.Contains("json", StringComparison.InvariantCultureIgnoreCase) ?? false;


		public class RequestModel
		{
			[JsonProperty("headers")]
			public IDictionary<string, IEnumerable<string>> Headers { get; set; }

			[JsonProperty("method")]
			public string Method { get; set; }

			[JsonProperty("uri")]
			public string Uri { get; set; }

			[JsonProperty("content")]
			public ContentModel Content { get; set; }
		}

		public class ResponseModel
		{
			[JsonProperty("headers")]
			public IDictionary<string, IEnumerable<string>> Headers { get; set; }

			[JsonProperty("statusCode")]
			public int StatusCode { get; set; }

			[JsonProperty("content")]
			public ContentModel Content { get; set; }

			[JsonProperty("method")]
			public string Method { get; set; }

			[JsonProperty("uri")]
			public string Uri { get; set; }
		}

		public class ContentModel
		{
			[JsonProperty("headers")]
			public IDictionary<string, IEnumerable<string>> Headers { get; set; }

			[JsonProperty("body")]
			public string ContentBody { get; set; }

			[JsonProperty("jsonBody")]
			public IDictionary<string, object> JsonBody { get; set; }
		}
	}
}
