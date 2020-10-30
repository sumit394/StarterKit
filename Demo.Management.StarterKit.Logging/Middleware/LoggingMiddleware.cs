using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using StarterKit.Filters;
using StarterKit.Logging.Handlers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StarterKit.Logging.Middleware
{
    public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;
		private readonly ICorrelationIdHandler _correlationIdHandler;

		public LoggingMiddleware(
			RequestDelegate next,
			ILogger logger,
			ICorrelationIdHandler correlationIdHandler)
		{
			_next = next;
			_logger = logger;
			_correlationIdHandler = correlationIdHandler;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var correlationId = _correlationIdHandler.GetCorrelationId().ToString();
			using (LogContext.PushProperty(CorrelationIdConstants.CorrelationId, correlationId))
			{
				try
				{
					var requestHeaders = context.Request.Headers.GetAllowedHeaders();
					using (LogContext.PushProperty(DefaultLoggingConstants.RequestHeaders, requestHeaders))
						_logger.Information("Request {Path} {Method}",
							context.Request.Path, context.Request.Method);

					await _next(context);

					var responseHeaders = context.Response.Headers.GetAllowedHeaders();
					var userId = GetUser(context);
					var routeTemplate = GetRouteTemplate(context);

					using (LogContext.PushProperty(DefaultLoggingConstants.UserId, userId))
					using (LogContext.PushProperty(DefaultLoggingConstants.RouteTemplate, routeTemplate))
					using (LogContext.PushProperty(DefaultLoggingConstants.ResponseHeaders, responseHeaders))
					{
						_logger.Information("Response {Path} {Method}. StatusCode: '{StatusCode}'. Took: {ElapsedMilliseconds}",
							context.Request.Path, context.Request.Method, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
					}
				}
				catch (Exception e)
				{
					_logger.Error(e, "Exception happened");
					throw;
				}
			}
		}

		private string GetUser(HttpContext context)
			=> context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value;

		private string GetRouteTemplate(HttpContext context)
		{
			context.Items.TryGetValue(RouteTemplateFilter.RouteTemplate, out var routeTemplate);
			return routeTemplate?.ToString();
		}

	}
}