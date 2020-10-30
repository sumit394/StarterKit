using AutoFixture.Xunit2;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StarterKit;
using System.Xml.Linq;
using Xunit;

namespace Demo.Management.StarterKit.Persistence.IntegrationTests
{
    // ReSharper disable once InconsistentNaming
    public class SqlXmlRepository_Should : IClassFixture<SetupDatabase>
	{
		private readonly ServiceCollection _serviceCollection;

		public SqlXmlRepository_Should(SetupDatabase database)
		{
			_serviceCollection = new ServiceCollection();
			_serviceCollection.PersistCookieKeysToSqlServer(database.ConnectionString);
		}

		[Theory, AutoData]
		public void Store_And_Get_Elements(XElement element, string friendlyName)
		{
			var serviceProvider = new DefaultServiceProviderFactory().CreateServiceProvider(_serviceCollection);
			var sut = new SqlXmlRepository(serviceProvider, Log.Logger);

			sut.StoreElement(element, friendlyName);
			var result = sut.GetAllElements();

			Assert.Single(result, x=> x.Value == element.Value);
		}
	}
}