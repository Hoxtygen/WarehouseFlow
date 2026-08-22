using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos;

public class CreateEmployeeUserDto : CreateUserDto
{
    [Required(ErrorMessage = "Employee number is required")]
    [StringLength(50)]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "User role is required")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;
}
