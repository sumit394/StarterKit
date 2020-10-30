using Demo.Management.StarterKit.Sample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using StarterKit;
using StarterKit.Test.ConfigurationProviders;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Demo.Management.StarterKit.IntegrationTests
{
    public class SetupServer : IDisposable
	{
		public SetupServer()
		{
			var dictionary = new Dictionary<string, string>
			{
				{"ADFS:PublicCertificatePath", "test.cer"},
				{"ADFS:Issuer", "http://syst-fs1.novanordisk.com/adfs/services/trust"},
				{"ADFS:ClientId", "https://syst-fs1.novanordisk.com/adfs"},
				{"ADFS:Url", "https://syst-fs1.novanordisk.com/adfs"},
				{"LOG:TO:CONSOLE", "false"},
			};
			var dictionarySource = new DictionarySource(dictionary);
			HostServer = Host.CreateDefaultBuilder()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseTestServer();
					webBuilder.ConfigureAppConfiguration(builder => builder.Sources.Add(dictionarySource));
					webBuilder.UseStartup<Startup>();
				}).UseStarterKit().Build();
			HostServer.Start();
			Client = HostServer.GetTestClient();
		}

		public HttpClient Client { get; }

		public IHost HostServer { get; }

		public void Dispose()
		{
			Client?.Dispose();
			HostServer.StopAsync().Wait();
		}
	}
}

