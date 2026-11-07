using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class CategoryCreateDto
{
	[Required(ErrorMessage = "Nama kategori wajib diisi")]
	[StringLength(50, MinimumLength = 2, ErrorMessage = "Nama kategori 2-50 karakter")]
	public string Name { get; set; } = string.Empty;
}

public class CategoryUpdateDto
{
	[Required(ErrorMessage = "Nama kategori wajib diisi")]
	[StringLength(50, MinimumLength = 2, ErrorMessage = "Nama kategori 2-50 karakter")]
	public string Name { get; set; } = string.Empty;
}

public record CategoryResponseDto(int Id, string Name, int MenuCount);
