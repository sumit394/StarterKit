using Microsoft.AspNetCore.Http;
using Serilog;
using StarterKit.Logging.Handlers;
using System;
using System.Threading.Tasks;

namespace StarterKit.Logging.Middleware
{
    public class CorrelationIdMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ICorrelationIdHandler _correlationIdHandler;
		private readonly ILogger _logger;
		
		public CorrelationIdMiddleware(
			RequestDelegate next,
			ICorrelationIdHandler correlationIdHandler,
			ILogger logger)
		{
			_next = next;
			_correlationIdHandler = correlationIdHandler;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				Guid correlationId;

				if (context.Request.Headers.ContainsKey(CorrelationIdConstants.CorrelationId))
				{
					if (!Guid.TryParse(context.Request.Headers[CorrelationIdConstants.CorrelationId], out correlationId))
					{
						context.Response.StatusCode = StatusCodes.Status400BadRequest;
						await context.Response.WriteAsync("CorrelationId http header must be a valid GUID");
						return;
					}
				}
				else
				{
					correlationId = Guid.NewGuid();
				}

				_correlationIdHandler.SetCorrelationId(correlationId);
				context.Response.Headers.Add(CorrelationIdConstants.CorrelationId, correlationId.ToString());
				await _next(context);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Error occurred in correlation middleware");
			}
		}
	}
}