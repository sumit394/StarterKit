using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StarterKit.Logging.Filters;
using StarterKit.Logging.Handlers;

namespace StarterKit.Logging
{
	public static class LoggingExtensions
	{
		public static void AddLoggingMiddleware(this IServiceCollection services, ILogger logger)
		{
			services.AddSingleton<ICorrelationIdHandler, CorrelationIdHandler>();
			services.AddTransient(x => new HttpLoggingHandler("httpClient", logger, 
				(CorrelationIdHandler)x.GetService(typeof(CorrelationIdHandler))));

			services.AddSingleton(logger);

			services.AddTransient<IStartupFilter, LoggingFilter>();
		}
	}
}
