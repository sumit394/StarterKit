using Dapper;
using DbUp;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Demo.Management.StarterKit.Persistence.IntegrationTests
{

    public class SetupDatabase : IDisposable
	{
		private string _masterConnectionString;

		private const string DefaultConnectionStringFormat =
			"Data Source=localhost;Database={0};User Id=sa;Password=YourStrong@Passw0rd";
		public SetupDatabase()
		{
			var connectionStringFormat = Environment.GetEnvironmentVariable("CONNECTIONSTRING_FORMAT_INTEGRATION_TESTS");
			if (string.IsNullOrWhiteSpace(connectionStringFormat))
				connectionStringFormat = DefaultConnectionStringFormat;

			DbName = Guid.NewGuid().ToString().Substring(0, 6);
			ConnectionString = string.Format(connectionStringFormat, DbName);
			_masterConnectionString = string.Format(connectionStringFormat, DbName);
			EnsureDatabase.For.SqlDatabase(ConnectionString);
		}

		public string ConnectionString { get; set; }

		public string DbName { get; set; }

		public void Dispose()
		{
			DropDatabase();
		}

		public void DropDatabase()
		{
			var connectionString = ConnectionString.ToString();
			var db = connectionString.Split(';').FirstOrDefault(s => s.StartsWith("Database="));
			if (!string.IsNullOrEmpty(db))
			{
				connectionString = connectionString.Replace(db, string.Empty);
				var dbName = db.Replace("Database=", string.Empty);
				using (var sqlDatabaseConnection = new SqlConnection(connectionString))
				{
					sqlDatabaseConnection.Open();
					var commandString =
						$"ALTER DATABASE [{dbName}] SET OFFLINE WITH ROLLBACK IMMEDIATE " +
						$"ALTER DATABASE [{dbName}] SET SINGLE_USER " +
						$"ALTER DATABASE [{dbName}] SET ONLINE WITH ROLLBACK IMMEDIATE " +
						$"DROP DATABASE [{dbName}]";
					sqlDatabaseConnection.Execute(commandString);
				}
			}
		}
	}
}
