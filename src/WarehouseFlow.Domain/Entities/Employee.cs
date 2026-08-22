namespace WarehouseFlow.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string Role { get; private set; } = null!;

    public string ApplicationUserId { get; private set; } = null!;
    public string CreatedByUserId { get; private set; } = null!;

    protected Employee() { }

    public Employee(
        string employeeNumber,
        string address,
        string role,
        string applicationUserId,
        string createdByUserId
    )
    {
        EmployeeNumber = RequireValue(employeeNumber, nameof(employeeNumber));
        Address = RequireValue(address, nameof(address));
        Role = RequireValue(role, nameof(role));
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