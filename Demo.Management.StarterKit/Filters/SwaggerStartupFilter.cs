using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StarterKit.Filters
{
	public class SwaggerStartupFilter : IStartupFilter
	{
		private readonly IConfiguration _configuration;

		public SwaggerStartupFilter(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return builder =>
			{
				builder.UseSwaggerRoutes(_configuration);
				next(builder);
			};
		}
	}
}