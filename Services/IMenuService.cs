using API.Dtos;

namespace API.Services;

public interface IMenuService
{
	Task<List<MenuResponseDto>> GetAllAsync(string? keyword, int? categoryId, string? sort);
	Task<MenuResponseDto> GetByIdAsync(int id);
	Task<MenuResponseDto> CreateAsync(MenuCreateDto dto);
	Task<MenuResponseDto> UpdateAsync(int id, MenuUpdateDto dto);
	Task DeleteAsync(int id);
}
