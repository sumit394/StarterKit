//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using StarterKit.Test;
//using Xunit;
//using Xunit.Abstractions;

//namespace Internal.Management.StarterKit.IntegrationTests.Controllers
//{
//	// ReSharper disable once InconsistentNaming
//	public class HomeController_Should : IClassFixture<SetupServer>
//	{
//		private readonly ITestOutputHelper _testOutputHelper;
//		private readonly SetupServer _server;

//		public HomeController_Should(ITestOutputHelper testOutputHelper,
//			SetupServer setupServer)
//		{
//			_testOutputHelper = testOutputHelper;
//			_server = setupServer;
//		}

//		[Fact]
//		public async Task Home_Should_Redirect_To_Swagger_If_Authorized()
//		{
//			var request = new HttpRequestMessage(HttpMethod.Get, "");
//			request.Headers.Add("Authorization", JwtToken.GenerateAuthHeader());
//			var result = await _server.Client.SendAsync(request);
//			var body = await result.Content.ReadAsStringAsync();
//			Assert.Equal("swagger", result.Headers.Location.ToString());
//			AssertExtension.AssertStatusCode(HttpStatusCode.Redirect, result.StatusCode, body, _testOutputHelper);
//		}
		
//		[Fact]
//		public async Task Me_Returns_302_If_Not_Authorized()
//		{
//			var result = await _server.Client.GetAsync("");
//			var body = await result.Content.ReadAsStringAsync();
//			AssertExtension.AssertStatusCode(HttpStatusCode.Redirect, result.StatusCode, body, _testOutputHelper);
//		}

//		[Fact]
//		public async Task Swagger_Returns_301()
//		{
//			var result = await _server.Client.GetAsync("swagger");
//			var body = await result.Content.ReadAsStringAsync();
//			AssertExtension.AssertStatusCode(HttpStatusCode.Moved, result.StatusCode, body, _testOutputHelper);
//		}
//	}
//}