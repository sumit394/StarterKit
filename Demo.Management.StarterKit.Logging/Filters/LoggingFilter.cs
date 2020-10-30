using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using StarterKit.Logging.Middleware;
using System;

namespace StarterKit.Logging.Filters
{
    public class LoggingFilter : IStartupFilter
	{
		private readonly IConfiguration _configuration;

		public LoggingFilter(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return builder =>
			{
				builder.UseMiddleware<CorrelationIdMiddleware>();
				builder.UseMiddleware<LoggingMiddleware>();
				next(builder);
			};
		}
	}

}