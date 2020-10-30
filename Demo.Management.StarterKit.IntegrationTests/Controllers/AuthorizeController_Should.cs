using Newtonsoft.Json;
using StarterKit.Test;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Management.StarterKit.IntegrationTests.Controllers
{
    // ReSharper disable once InconsistentNaming
    public class AuthorizeController_Should : IClassFixture<SetupServer>
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly SetupServer _server;
		private const string DefaultUserId = "BC0001";

		public AuthorizeController_Should(ITestOutputHelper testOutputHelper,
			SetupServer setupServer)
		{
			_testOutputHelper = testOutputHelper;
			_server = setupServer;
		}

		[Fact]
		public async Task Authenticated_Return_401_If_Not_Authorized()
		{
			var result = await _server.Client.GetAsync("/api/v1/authorize/authenticated");
			var body = await result.Content.ReadAsStringAsync();
			AssertExtension.AssertStatusCode(HttpStatusCode.Unauthorized, result.StatusCode, body, _testOutputHelper);
		}

		[Fact]
		public async Task Authenticated_Return_200_If_Authorized()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/authorize/authenticated");
			request.Headers.Add("Authorization", JwtToken.GenerateAuthHeader());
			var result = await _server.Client.SendAsync(request);
			var body = await result.Content.ReadAsStringAsync();
			AssertExtension.AssertStatusCode(HttpStatusCode.OK, result.StatusCode, body, _testOutputHelper);
			Assert.Equal("integrationTesting", JsonConvert.DeserializeObject<string>(body));
		}

		[Fact]
		public async Task Me_Returns_302_If_Not_Authorized()
		{
			var result = await _server.Client.GetAsync("/api/v1/authorize/me");
			var body = await result.Content.ReadAsStringAsync();
			AssertExtension.AssertStatusCode(HttpStatusCode.Redirect, result.StatusCode, body, _testOutputHelper);
		}

		[Fact]
		public async Task Me_Returns_200()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/authorize/me");
			request.Headers.Add("Authorization", JwtToken.GenerateAuthHeader(DefaultUserId));
			var result = await _server.Client.SendAsync(request);
			Assert.Equal((HttpStatusCode)200, result.StatusCode);
			Assert.Equal(DefaultUserId, result.Headers.First(x => x.Key == "X-User").Value.FirstOrDefault());
		}

		[Fact]
		public async Task Authenticate_Returns_302_If_Authorized_To_Correct_Url()
		{
			const string redirectUrl = "http://localhost/";
			var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/authorize/authenticate?redirectUrl={redirectUrl}");
			request.Headers.Add("Authorization", JwtToken.GenerateAuthHeader(DefaultUserId));
			var result = await _server.Client.SendAsync(request);
			Assert.Equal(HttpStatusCode.Redirect, result.StatusCode);
			Assert.Equal(redirectUrl, result.Headers.Location.ToString());
		}

		[Fact]
		public async Task Authenticate_Returns_400_If_No_Redirect_Url_Is_Provided()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/authorize/authenticate");
			request.Headers.Add("Authorization", JwtToken.GenerateAuthHeader(DefaultUserId));
			var result = await _server.Client.SendAsync(request);
			Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
		}
	}
}