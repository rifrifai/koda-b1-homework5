using API.Dtos;
using API.Exceptions;
using API.Models;
using API.Repositories;

namespace API.Services;

public class CategoryService(ICategoryRepository repo) : ICategoryService
{
	public async Task<List<CategoryResponseDto>> GetAllAsync()
	{
		var list = await repo.GetAllAsync();
		return list.Select(ToDto).ToList();
	}

	public async Task<CategoryResponseDto> GetByIdAsync(int id)
	{
		var category = await repo.GetByIdAsync(id)
			?? throw new NotFoundException($"Kategori dengan id {id} tidak ditemukan");
		return ToDto(category);
	}

	public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
	{
		string name = dto.Name.Trim();
		if (await repo.NameExistsAsync(name))
			throw new ConflictException($"Kategori '{name}' sudah ada");

		int id = await repo.AddAsync(new Category { Name = name });
		return await GetByIdAsync(id);
	}

	public async Task<CategoryResponseDto> UpdateAsync(int id, CategoryUpdateDto dto)
	{
		var category = await repo.GetByIdAsync(id)
			?? throw new NotFoundException($"Kategori dengan id {id} tidak ditemukan");

		string name = dto.Name.Trim();
		if (await repo.NameExistsAsync(name, id))
			throw new ConflictException($"Kategori '{name}' sudah ada");

		category.Name = name;
		await repo.UpdateAsync(category);
		return ToDto(category);
	}

	public async Task DeleteAsync(int id)
	{
		_ = await repo.GetByIdAsync(id)
			?? throw new NotFoundException($"Kategori dengan id {id} tidak ditemukan");

		if (await repo.HasMenusAsync(id))
			throw new ConflictException("Kategori masih punya menu, hapus/pindahkan menunya dulu");

		await repo.DeleteAsync(id);
	}

	private static CategoryResponseDto ToDto(Category c) => new(c.Id, c.Name, c.MenuCount);
}
