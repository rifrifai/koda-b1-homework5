using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class MenuCreateDto
{
	[Required(ErrorMessage = "Nama menu wajib diisi")]
	[StringLength(100, MinimumLength = 2, ErrorMessage = "Nama menu 2-100 karakter")]
	public string Name { get; set; } = string.Empty;

	[Range(1, 10_000_000, ErrorMessage = "Harga harus lebih dari 0")]
	public int Price { get; set; }

	[Range(0, 100_000, ErrorMessage = "Stok tidak boleh negatif")]
	public int Stock { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "CategoryId tidak valid")]
	public int CategoryId { get; set; }
}

public class MenuUpdateDto
{
	[Required(ErrorMessage = "Nama menu wajib diisi")]
	[StringLength(100, MinimumLength = 2, ErrorMessage = "Nama menu 2-100 karakter")]
	public string Name { get; set; } = string.Empty;

	[Range(1, 10_000_000, ErrorMessage = "Harga harus lebih dari 0")]
	public int Price { get; set; }

	[Range(0, 100_000, ErrorMessage = "Stok tidak boleh negatif")]
	public int Stock { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "CategoryId tidak valid")]
	public int CategoryId { get; set; }
}

public record MenuResponseDto(int Id, string Name, int Price, int Stock, int CategoryId, string CategoryName);
