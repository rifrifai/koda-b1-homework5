using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController(IMenuService service) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<MenuResponseDto>>> GetAll(
		[FromQuery] string? keyword, [FromQuery] int? categoryId, [FromQuery] string? sort) =>
		Ok(await service.GetAllAsync(keyword, categoryId, sort));

	[HttpGet("{id:int}")]
	public async Task<ActionResult<MenuResponseDto>> GetById(int id) =>
		Ok(await service.GetByIdAsync(id));

	[HttpPost]
	public async Task<ActionResult<MenuResponseDto>> Create(MenuCreateDto dto)
	{
		var created = await service.CreateAsync(dto);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult<MenuResponseDto>> Update(int id, MenuUpdateDto dto) =>
		Ok(await service.UpdateAsync(id, dto));

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		await service.DeleteAsync(id);
		return NoContent();
	}
}
