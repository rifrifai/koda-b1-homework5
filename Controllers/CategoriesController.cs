using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService service) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<CategoryResponseDto>>> GetAll() =>
		Ok(await service.GetAllAsync());

	[HttpGet("{id:int}")]
	public async Task<ActionResult<CategoryResponseDto>> GetById(int id) =>
		Ok(await service.GetByIdAsync(id));

	[HttpPost]
	public async Task<ActionResult<CategoryResponseDto>> Create(CategoryCreateDto dto)
	{
		var created = await service.CreateAsync(dto);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult<CategoryResponseDto>> Update(int id, CategoryUpdateDto dto) =>
		Ok(await service.UpdateAsync(id, dto));

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		await service.DeleteAsync(id);
		return NoContent();
	}
}
