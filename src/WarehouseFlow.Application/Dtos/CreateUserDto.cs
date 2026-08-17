

using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos;
public class CreateUserDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(25, ErrorMessage = "First name must be 25 characters or less")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(25, ErrorMessage = "Last name must be 25 characters or less")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    [StringLength(25, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 25 characters")]
    public string Password { get; set; } = string.Empty;
}