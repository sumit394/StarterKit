using Dapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Data.SqlClient;

namespace StarterKit
{
    public static class SqlXmlEncryptionExtensions
    {
        public static IServiceCollection PersistCookieKeysToSqlServer(this IServiceCollection collection, string connectionString)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddSingleton(new ConnectionString(connectionString));
            collection.AddScoped<ICookieKeyRepository, CookieKeyRepository>();
            EnsureTableExists(connectionString);

            collection.AddDataProtection().Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
            {
                var logger = services.GetService<ILogger>();
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlRepository =
                        new SqlXmlRepository(services, logger);
                });
            });

            return collection;
        }

        private static void EnsureTableExists(string connectionString)
        {

            using var connection = new SqlConnection(connectionString);
            const string ensureTableExistsCommand = @"
					IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CookieKeys' and xtype='U')
					CREATE TABLE [dbo].[CookieKeys]
					(
						[ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
						[EncryptionKey] TEXT NOT NULL, 
						[Xml] XML NOT NULL
					)";
            connection.Execute(ensureTableExistsCommand);
        }
    }
}