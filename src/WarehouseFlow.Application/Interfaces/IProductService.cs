using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponse> CreateProduct(
        NewProductDto newProductDto,
        CancellationToken cancellationToken = default
    );

    Task<ProductResponse> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken = default);
}
