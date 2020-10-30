using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StarterKit.Configuration;
using StarterKit.Filters;

namespace StarterKit
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder UseStarterKit(this IHostBuilder builder, string title = "api")
		{
			builder.ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders());
			return builder.ConfigureServices((context, collection) =>
			{
				collection.AddMvcCore(options =>
					{
						options.EnableEndpointRouting = false;
						options.Filters.Add<RouteTemplateFilter>();
					})
					.AddApiExplorer().AddAuthorization();
				collection.UseSwagger(title);
				collection.UseAdfs(context);

				collection.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
				collection.AddScoped<BaseSettings>();
				collection.AddScoped<IUserContext, UserContext>();
			});
		}

		public static void UseSwagger(this IServiceCollection collection, string title = "api")
		{
			collection.AddSwaggerGen(c =>
			{
				c.MapType<IFormFile>(() => new OpenApiSchema { Type = "file" });
				c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });
			});

			collection.ConfigureSwaggerGen(options => { });
			collection.AddTransient<IStartupFilter, SwaggerStartupFilter>();
		}

		public static void UseAdfs(this IServiceCollection collection, HostBuilderContext hostContext)
		{
			var adfsSettings = hostContext.Configuration.GetSection(AdfsSettings.Key).Get<AdfsSettings>();

            if (adfsSettings == null)
                return;

			var baseSettings = hostContext.Configuration.Get<BaseSettings>();
			var certificate = !string.IsNullOrEmpty(adfsSettings.PublicCertificatePath)
				? new System.Security.Cryptography.X509Certificates.X509Certificate2(
					adfsSettings.PublicCertificatePath)
				: null;

			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				ValidIssuer = adfsSettings.Issuer,
				ValidateAudience = false,
				IssuerSigningKey = certificate != null ? new X509SecurityKey(certificate) : null,
				ValidateLifetime = true,
				RequireSignedTokens = true
			};

			const string smart = "smart";
			collection.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = smart;
					options.DefaultChallengeScheme = smart;
					options.DefaultSignInScheme = smart;
				})
				.AddPolicyScheme(smart, "OpenId or JwtBearer", options =>
				{
					options.ForwardSignIn = CookieAuthenticationDefaults.AuthenticationScheme;
					options.ForwardChallenge = OpenIdConnectDefaults.AuthenticationScheme;
					options.ForwardDefaultSelector = context =>
					{
						var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
						if (authHeader?.StartsWith("Bearer ") == true)
						{
							return JwtBearerDefaults.AuthenticationScheme;
						}

						return CookieAuthenticationDefaults.AuthenticationScheme;
					};
				})
				.AddCookie(option => option.Cookie.SameSite = SameSiteMode.None)
				.AddJwtBearer(o => o.TokenValidationParameters = validationParameters)
				.AddOpenIdConnect(o =>
					{
						o.CallbackPath = new PathString($"/{baseSettings?.SubPath}signin-oidc");
						o.ClientId = adfsSettings.ClientId;
						o.Authority = adfsSettings.Url;
						o.ResponseType = "code";
						o.GetClaimsFromUserInfoEndpoint = false;
						o.TokenValidationParameters = validationParameters;
					}
				);

			collection.AddTransient<IStartupFilter, BaseStartupFilter>();
		}
	}
}
