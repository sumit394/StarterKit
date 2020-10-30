using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace StarterKit
{
	public static class ApplicationBuilderExtension
	{
		/// <summary>
		/// Includes both UseSwaggerUI and UseSwagger
		/// </summary>
		/// <param name="applicationBuilder"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseSwaggerRoutes(this IApplicationBuilder applicationBuilder, IConfiguration configuration)
		{
			applicationBuilder.UseSwagger(options => { options.RouteTemplate = $"{configuration["USE_SUB_PATH"]}swagger/{{documentName}}/swagger.json"; });

            applicationBuilder.UseSwaggerUI(c =>
            {
				c.RoutePrefix = $"{configuration["USE_SUB_PATH"]}swagger";
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "api");
            });

			return applicationBuilder;
		}
	}
}