using System.Text;
using API.Data;
using API.Models;
using Microsoft.Data.SqlClient;

namespace API.Repositories;

public class MenuRepository(SqlConnectionFactory factory) : IMenuRepository
{
	private const string SelectSql = @"
		SELECT m.Id, m.Name, m.Price, m.Stock, m.CategoryId, c.Name AS CategoryName
		FROM MenuItems m
		INNER JOIN Categories c ON c.Id = m.CategoryId";

	public async Task<List<MenuItem>> GetAllAsync(string? keyword, int? categoryId, string? sort)
	{
		var sql = new StringBuilder(SelectSql);
		sql.Append(" WHERE 1 = 1");

		if (!string.IsNullOrWhiteSpace(keyword))
			sql.Append(" AND m.Name LIKE @Keyword");
		if (categoryId.HasValue)
			sql.Append(" AND m.CategoryId = @CategoryId");

		sql.Append(sort?.ToLower() switch
		{
			"price_asc" => " ORDER BY m.Price ASC",
			"price_desc" => " ORDER BY m.Price DESC",
			_ => " ORDER BY m.Id"
		});

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql.ToString(), conn);
		if (!string.IsNullOrWhiteSpace(keyword))
			cmd.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
		if (categoryId.HasValue)
			cmd.Parameters.AddWithValue("@CategoryId", categoryId.Value);

		await using var reader = await cmd.ExecuteReaderAsync();
		var list = new List<MenuItem>();
		while (await reader.ReadAsync())
			list.Add(Map(reader));
		return list;
	}

	public async Task<MenuItem?> GetByIdAsync(int id)
	{
		const string sql = SelectSql + " WHERE m.Id = @Id";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Id", id);
		await using var reader = await cmd.ExecuteReaderAsync();

		return await reader.ReadAsync() ? Map(reader) : null;
	}

	public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
	{
		const string sql = "SELECT COUNT(1) FROM MenuItems WHERE Name = @Name AND Id <> @ExcludeId";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Name", name);
		cmd.Parameters.AddWithValue("@ExcludeId", excludeId ?? 0);
		return (int)(await cmd.ExecuteScalarAsync())! > 0;
	}

	public async Task<int> AddAsync(MenuItem item)
	{
		const string sql = @"
			INSERT INTO MenuItems (Name, Price, Stock, CategoryId)
			OUTPUT INSERTED.Id
			VALUES (@Name, @Price, @Stock, @CategoryId)";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Name", item.Name);
		cmd.Parameters.AddWithValue("@Price", item.Price);
		cmd.Parameters.AddWithValue("@Stock", item.Stock);
		cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
		return (int)(await cmd.ExecuteScalarAsync())!;
	}

	public async Task UpdateAsync(MenuItem item)
	{
		const string sql = @"
			UPDATE MenuItems
			SET Name = @Name, Price = @Price, Stock = @Stock, CategoryId = @CategoryId
			WHERE Id = @Id";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Name", item.Name);
		cmd.Parameters.AddWithValue("@Price", item.Price);
		cmd.Parameters.AddWithValue("@Stock", item.Stock);
		cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
		cmd.Parameters.AddWithValue("@Id", item.Id);
		await cmd.ExecuteNonQueryAsync();
	}

	public async Task DeleteAsync(int id)
	{
		const string sql = "DELETE FROM MenuItems WHERE Id = @Id";

		await using var conn = factory.Create();
		await conn.OpenAsync();
		await using var cmd = new SqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("@Id", id);
		await cmd.ExecuteNonQueryAsync();
	}

	private static MenuItem Map(SqlDataReader r) => new()
	{
		Id = r.GetInt32(r.GetOrdinal("Id")),
		Name = r.GetString(r.GetOrdinal("Name")),
		Price = r.GetInt32(r.GetOrdinal("Price")),
		Stock = r.GetInt32(r.GetOrdinal("Stock")),
		CategoryId = r.GetInt32(r.GetOrdinal("CategoryId")),
		CategoryName = r.GetString(r.GetOrdinal("CategoryName"))
	};
}
