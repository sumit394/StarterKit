using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace StarterKit
{
    public class SqlXmlRepository : IXmlRepository
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger _logger;

		public SqlXmlRepository(IServiceProvider serviceProvider, ILogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public IReadOnlyCollection<XElement> GetAllElements()
		{
			using var scope = _serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<ICookieKeyRepository>();

			return context.GetAll().Result.Select(key => TryParseKeyXml(key.Xml)).ToList().AsReadOnly();
		}

		public void StoreElement(XElement element, string friendlyName)
		{
			using var scope = _serviceProvider.CreateScope();
			var repo = scope.ServiceProvider.GetService<ICookieKeyRepository>();
			var newKey = new CookieKeys()
			{
				EncryptionKey = friendlyName,
				Xml = element.ToString(SaveOptions.DisableFormatting)
			};

			repo.Save(newKey).Wait();
		}

		private XElement TryParseKeyXml(string xml)
		{
			try
			{
				return XElement.Parse(xml);
			}
			catch (Exception e)
			{
				_logger.ForContext("Exception", e).Error("Exception occured when parsing {Xml}. See Exception field for more details.", xml);
				return null;
			}
		}
	}
}