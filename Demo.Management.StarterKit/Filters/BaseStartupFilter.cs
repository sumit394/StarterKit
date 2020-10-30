using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace StarterKit.Filters
{
	public class BaseStartupFilter : IStartupFilter
	{
		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return builder =>
			{
				builder.UseAuthentication();
				builder.UseMvc();
				next(builder);
			};
		}
	}
}