using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    protected User() { }

    public User(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string password,
        UserRole role=UserRole.Customer,
        Guid? customerId = null,
        Customer? customer = null
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        Role = role;
        CustomerId = customerId;
        Customer = customer;
    }
}
