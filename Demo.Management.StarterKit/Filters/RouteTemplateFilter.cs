using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StarterKit.Filters
{
	public class RouteTemplateFilter : IActionFilter, IAsyncActionFilter
	{
		public const string RouteTemplate = "RouteTemplate";

		public void OnActionExecuting(ActionExecutingContext context)
		{
			AddTemplateItemToContext(context);
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
		}

		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			AddTemplateItemToContext(context);
			return next();
		}

		private void AddTemplateItemToContext(ActionExecutingContext context)
		{
			context.HttpContext.Items.Add(RouteTemplate, context.ActionDescriptor.AttributeRouteInfo.Template);
		}
	}
}