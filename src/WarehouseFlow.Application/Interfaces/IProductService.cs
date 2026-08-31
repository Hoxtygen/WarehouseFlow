using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface IProductService
{
    Task<Product> createProduct(
        NewProductDto newProductDto,
        CancellationToken cancellationToken = default
    );

    Task<Product> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken = default);
}
