using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using StarterKit.Logging;

namespace Demo.Management.StarterKit.Sample
{
    public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection collection)
		{
			var logger = new LoggerConfiguration()
				.WriteTo.Console(new ElasticsearchJsonFormatter())
				.Enrich.FromLogContext()
				.CreateLogger();
			collection.AddLoggingMiddleware(logger);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
		}
	}
}
