using Dapper;
using Microsoft.Extensions.DependencyInjection;
using StarterKit;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Management.StarterKit.Persistence.IntegrationTests
{
    // ReSharper disable once InconsistentNaming
    public class SqlXmlEncryptionExtensions_Should : IClassFixture<SetupDatabase>
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly SetupDatabase _database;

		public SqlXmlEncryptionExtensions_Should(ITestOutputHelper testOutputHelper, SetupDatabase database)
		{
			_testOutputHelper = testOutputHelper;
			_database = database;
		}

		[Fact]
		public async Task PersistCookieKeysToSqlServer_Creates_Table()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.PersistCookieKeysToSqlServer(_database.ConnectionString);

			await using var connection = new SqlConnection(_database.ConnectionString);
			const string cookieKeysTableQuery = "SELECT * FROM sysobjects WHERE name='CookieKeys' and xtype='U'";
			Assert.Single(connection.Query(cookieKeysTableQuery));
		}
	}
}