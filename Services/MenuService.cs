using API.Dtos;
using API.Exceptions;
using API.Models;
using API.Repositories;

namespace API.Services;

public class MenuService(IMenuRepository menuRepo, ICategoryRepository categoryRepo) : IMenuService
{
	public async Task<List<MenuResponseDto>> GetAllAsync(string? keyword, int? categoryId, string? sort)
	{
		var list = await menuRepo.GetAllAsync(keyword, categoryId, sort);
		return list.Select(ToDto).ToList();
	}

	public async Task<MenuResponseDto> GetByIdAsync(int id)
	{
		var item = await menuRepo.GetByIdAsync(id)
			?? throw new NotFoundException($"Menu dengan id {id} tidak ditemukan");
		return ToDto(item);
	}

	public async Task<MenuResponseDto> CreateAsync(MenuCreateDto dto)
	{
		string name = dto.Name.Trim();
		if (await categoryRepo.GetByIdAsync(dto.CategoryId) is null)
			throw new NotFoundException($"Kategori dengan id {dto.CategoryId} tidak ditemukan");
		if (await menuRepo.NameExistsAsync(name))
			throw new ConflictException($"Menu '{name}' sudah ada");

		int id = await menuRepo.AddAsync(new MenuItem
		{
			Name = name,
			Price = dto.Price,
			Stock = dto.Stock,
			CategoryId = dto.CategoryId
		});

		// ambil ulang biar nama kategorinya ikut (hasil JOIN)
		return await GetByIdAsync(id);
	}

	public async Task<MenuResponseDto> UpdateAsync(int id, MenuUpdateDto dto)
	{
		var item = await menuRepo.GetByIdAsync(id)
			?? throw new NotFoundException($"Menu dengan id {id} tidak ditemukan");

		string name = dto.Name.Trim();
		if (await categoryRepo.GetByIdAsync(dto.CategoryId) is null)
			throw new NotFoundException($"Kategori dengan id {dto.CategoryId} tidak ditemukan");
		if (await menuRepo.NameExistsAsync(name, id))
			throw new ConflictException($"Menu '{name}' sudah ada");

		item.Name = name;
		item.Price = dto.Price;
		item.Stock = dto.Stock;
		item.CategoryId = dto.CategoryId;
		await menuRepo.UpdateAsync(item);

		return await GetByIdAsync(id);
	}

	public async Task DeleteAsync(int id)
	{
		_ = await menuRepo.GetByIdAsync(id)
			?? throw new NotFoundException($"Menu dengan id {id} tidak ditemukan");
		await menuRepo.DeleteAsync(id);
	}

	private static MenuResponseDto ToDto(MenuItem m) =>
		new(m.Id, m.Name, m.Price, m.Stock, m.CategoryId, m.CategoryName);
}
