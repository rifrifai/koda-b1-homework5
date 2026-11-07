using Microsoft.Data.SqlClient;

namespace API.Data;

public class SqlConnectionFactory(IConfiguration config)
{
	private readonly string _connectionString =
		config.GetConnectionString("DefaultConnection")
		?? throw new InvalidOperationException("Connection string 'DefaultConnection' belum diisi");

	public SqlConnection Create() => new(_connectionString);
}
