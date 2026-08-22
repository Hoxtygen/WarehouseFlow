namespace WarehouseFlow.Application.Dtos;

public class LoginResponse
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = string.Empty;
    public string AccessToken { get; set; } = null!;
}
