using API.Models;

namespace API.Repositories;

public interface ICategoryRepository
{
	Task<List<Category>> GetAllAsync();
	Task<Category?> GetByIdAsync(int id);
	Task<bool> NameExistsAsync(string name, int? excludeId = null);
	Task<bool> HasMenusAsync(int id);
	Task<int> AddAsync(Category category);
	Task UpdateAsync(Category category);
	Task DeleteAsync(int id);
}
