using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StarterKit
{
    public class CookieKeyRepository : ICookieKeyRepository
	{
		private readonly string _connectionString;

		public CookieKeyRepository(ConnectionString connectionString)
		{
			_connectionString = connectionString.ToString();
		}

		public async Task<bool> Save(CookieKeys cookieKeys)
		{

			await using var connection = new SqlConnection(_connectionString);
			const string insertQuery = @"
					INSERT INTO [dbo].[CookieKeys]
						([EncryptionKey],
						[Xml])
					VALUES
						(@EncryptionKey,
						@Xml)";
			var result = await connection.ExecuteAsync(insertQuery, new
			{
				cookieKeys.EncryptionKey,
				cookieKeys.Xml
			});

			return result > 0;
		}

		public async Task<List<CookieKeys>> GetAll()
		{
			await using var connection = new SqlConnection(_connectionString.ToString());
			const string query = @"SELECT * FROM [dbo].[CookieKeys]";

			var result = await connection.QueryAsync<CookieKeys>(
				query);

			return result.ToList();
		}
	}
}