using AutoFixture.Xunit2;
using Moq;
using Serilog;
using StarterKit.Logging.Handlers;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Demo.Management.StarterKit.Logging.UnitTests
{
    // ReSharper disable once InconsistentNaming
    public class HttpLoggingHandler_Should
	{
		[Theory, AutoMoqData]
		public async Task Log_request_and_response(
			string name,
			[Frozen] Mock<ILogger> loggerMock,
			HttpRequestMessage request,
			CancellationToken cancellationToken,
			Mock<ICorrelationIdHandler> correlationIdHandlerMock)
		{
			var sut = new HttpLoggingHandler(name, loggerMock.Object, correlationIdHandlerMock.Object, new TestHandler());

			await sut.Log(request, cancellationToken);

			loggerMock.Verify(x => x.Information(
				sut.Name + " {@reqMsg}", It.IsAny<HttpLoggingHandler.RequestModel>()), Times.Once);
			loggerMock.Verify(x => x.Information(
				sut.Name + " {@respMsg},{elapsedMs}", It.IsAny<HttpLoggingHandler.ResponseModel>(), It.IsAny<long[]>()), Times.Once);
		}

		[Theory, AutoMoqData]
		public async Task SendAsync_used_by_httpClient_get_should_log_request_and_response(
			string name,
			[Frozen] Mock<ILogger> loggerMock,
			Mock<ICorrelationIdHandler> correlationIdHandlerMock)
		{
			var sut = new HttpLoggingHandler(name, loggerMock.Object, correlationIdHandlerMock.Object, new TestHandler());
			using (var http = new HttpClient(sut))
				await http.GetAsync(new Uri("http://somewhere.com"));

			loggerMock.Verify(x => x.Information(
				sut.Name + " {@reqMsg}", It.IsAny<HttpLoggingHandler.RequestModel>()), Times.Once);
			loggerMock.Verify(x => x.Information(
				sut.Name + " {@respMsg},{elapsedMs}", It.IsAny<HttpLoggingHandler.ResponseModel>(), It.IsAny<long[]>()), Times.Once);
		}

		[Theory, AutoData]
		public async Task Log_error_when_info_logging_throw_exception(
			string name,
			[Frozen] Mock<ILogger> loggerMock,
			Uri uri,
			StringContent stringContent, 
			Exception ex,
			CancellationToken cancellationToken,
			Mock<ICorrelationIdHandler> correlationIdHandlerMock)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, uri) {Content = stringContent};

			var sut = new HttpLoggingHandler(name, loggerMock.Object, correlationIdHandlerMock.Object, new TestHandler());
			loggerMock.Setup(x => x.Information(
				sut.Name + " {@reqMsg}", It.IsAny<HttpLoggingHandler.RequestModel>())).Throws(ex);

			await sut.Log(request, cancellationToken);

			loggerMock.Verify(x => x.Error("Error parsing requestMessage/response for logging", ex), Times.Once);
		}
	}
}