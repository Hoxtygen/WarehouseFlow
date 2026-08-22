using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos;

public class AddressDto
{
    [Required, StringLength(200)]
    public string Street { get; set; } = string.Empty;

    [Required, StringLength(5)]
    public string HouseNumber { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string State { get; set; } = string.Empty;

    [StringLength(200)]
    public string LandMark { get; set; } = string.Empty;
}