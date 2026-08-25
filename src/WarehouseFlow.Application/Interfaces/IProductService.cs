using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;


public interface IProductService
{
    Task<Product> createProduct(
        NewProductDto newProductDto,
        CancellationToken cancellationToken = default
    );
}