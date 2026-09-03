using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos;

public class RegisterCustomerDto : CreateUserDto
{
    [Required]
    public AddressDto Address { get; set; } = null!;
}