using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public UserRole Role { get; private set; }

    public string ApplicationUserId { get; private set; } = null!;
    public string CreatedByUserId { get; private set; } = null!;

    protected Employee() { }

    public Employee(
        string employeeNumber,
        string address,
        UserRole role,
        string applicationUserId,
        string createdByUserId
    )
    {
        EmployeeNumber = RequireValue(employeeNumber, nameof(employeeNumber));
        Address = RequireValue(address, nameof(address));
        Role = role;
        ApplicationUserId = RequireValue(applicationUserId, nameof(applicationUserId));
        CreatedByUserId = RequireValue(createdByUserId, nameof(createdByUserId));
    }

    private static string RequireValue(string value, string parameterName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value is required.", parameterName)
            : value.Trim();
    }
}