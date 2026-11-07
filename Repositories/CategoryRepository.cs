using API.Data;
using API.Models;
using Microsoft.Data.SqlClient;

namespace API.Repositories;

public class CategoryRepository(SqlConnectionFactory factory) : ICategoryRepository
{
	private const string SelectSql = @"
		SELECT c.Id, c.Name, COUNT(m.Id) AS MenuCount
		FROM Categories c
		LEFT JOIN MenuItems m ON m.CategoryId = c.Id";

	public async Task<List<Category>> GetAllAsync()
	{
		const string sql = SelectSql + " GROUP BY c.Id, c.Name ORDER BY c.Id";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		await using var reader = await cmd.ExecuteReaderAsync();

		var list = new List<Category>();
		while (await reader.ReadAsync())
			list.Add(Map(reader));
		return list;
	}

	public async Task<Category?> GetByIdAsync(int id)
	{
		const string sql = SelectSql + " WHERE c.Id = @Id GROUP BY c.Id, c.Name";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Id", id);
		await using var reader = await cmd.ExecuteReaderAsync();

		return await reader.ReadAsync() ? Map(reader) : null;
	}

	public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
	{
		const string sql = "SELECT COUNT(1) FROM Categories WHERE Name = @Name AND Id <> @ExcludeId";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Name", name);
		cmd.Parameters.AddWithValue("@ExcludeId", excludeId ?? 0);
		return (int)(await cmd.ExecuteScalarAsync())! > 0;
	}

	public async Task<bool> HasMenusAsync(int id)
	{
		const string sql = "SELECT COUNT(1) FROM MenuItems WHERE CategoryId = @Id";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Id", id);
		return (int)(await cmd.ExecuteScalarAsync())! > 0;
	}

	public async Task<int> AddAsync(Category category)
	{
		const string sql = "INSERT INTO Categories (Name) OUTPUT INSERTED.Id VALUES (@Name)";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Name", category.Name);
		return (int)(await cmd.ExecuteScalarAsync())!;
	}

	public async Task UpdateAsync(Category category)
	{
		const string sql = "UPDATE Categories SET Name = @Name WHERE Id = @Id";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Name", category.Name);
		cmd.Parameters.AddWithValue("@Id", category.Id);
		await cmd.ExecuteNonQueryAsync();
	}

	public async Task DeleteAsync(int id)
	{
		const string sql = "DELETE FROM Categories WHERE Id = @Id";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Id", id);
		await cmd.ExecuteNonQueryAsync();
	}

	private static Category Map(SqlDataReader r) => new()
	{
		Id = r.GetInt32(r.GetOrdinal("Id")),
		Name = r.GetString(r.GetOrdinal("Name")),
		MenuCount = r.GetInt32(r.GetOrdinal("MenuCount"))
	};
}
