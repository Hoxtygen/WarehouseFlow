using Microsoft.AspNetCore.Identity;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Dtos;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;
}
