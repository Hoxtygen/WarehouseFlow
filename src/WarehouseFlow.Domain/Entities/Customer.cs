using WarehouseFlow.Domain.ValueObjects;

namespace WarehouseFlow.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Address Address { get; set; } = null!;

    public string? ApplicationUserId { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    protected Customer() { }

    public Customer(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        Address address
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
    }
}
