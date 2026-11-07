using API.Models;

namespace API.Repositories;

public interface IMenuRepository
{
	Task<List<MenuItem>> GetAllAsync(string? keyword, int? categoryId, string? sort);
	Task<MenuItem?> GetByIdAsync(int id);
	Task<bool> NameExistsAsync(string name, int? excludeId = null);
	Task<int> AddAsync(MenuItem item); 
	Task UpdateAsync(MenuItem item);
	Task DeleteAsync(int id);
}
