using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace StarterKit
{
	public class UserContext : IUserContext
	{
		private readonly HttpContext _httpContext;

		public UserContext(IHttpContextAccessor accessor)
		{
			_httpContext = accessor?.HttpContext;
			LoadCredentialsFromHttpContext();
		}

		public string Username { get; private set; }
		public string Email { get; private set; }

		private void LoadCredentialsFromHttpContext()
		{
			var claims = _httpContext?.User?.Claims?.ToDictionary(cl => cl.Type, cl => cl.Value);
			if (claims != null && claims.Any())
			{
				var bUser = claims?[ClaimTypes.Name];
				Email = claims?[ClaimTypes.Upn];
				Username = bUser.Split("\\").LastOrDefault();
			}
		}
	}
}