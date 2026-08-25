
using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos;

public record NewWarehouseDto(
    [Required]
    [StringLength(100)]
    string WarehouseName,

    [Required]
    [StringLength(20)]
    string Location,

    [Required]
    [StringLength(255)]
    string Address,

    [Range(1, int.MaxValue)]
    int Capacity
);