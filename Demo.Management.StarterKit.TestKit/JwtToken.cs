using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace StarterKit.Test
{
    public class JwtToken
	{
		private readonly Dictionary<string, string> _config;

		public JwtToken(Dictionary<string, string> config)
		{
			_config = config;
		}

		public string Generate(string fromFile, string password,
			string name = "integrationTesting")
		{
			var claims = new List<Claim>
			{
				new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", $"NT0001\\{name}"),
				new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "integrationTesting@novanordisk.com")
			};
			var certificate = new X509Certificate2(fromFile, password);
			var token = new JwtSecurityToken(
				issuer: "http://syst-fs1.novanordisk.com/adfs/services/trust",
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddDays(1),
				signingCredentials: new SigningCredentials(new X509SecurityKey(certificate), "RS256"),
				claims: claims);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static string GenerateAuthHeader(string name = "integrationTesting")
		{
			return $"Bearer {new JwtToken(null).Generate("cert.pfx", "123", name)}";
		}
	}
}