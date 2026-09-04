using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Repositories;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Products.AsNoTracking()
            .Include(product => product.ProductCategory)
            .FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Products.AnyAsync(
            product => product.Id == productId,
            cancellationToken
        );
    }

    public async Task<bool> SkuExistsAsync(
        string sku,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Products.AnyAsync(product => product.SKU == sku, cancellationToken);
    }

    public async Task<string?> GetCategoryNameAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .ProductCategories.Where(category => category.Id == categoryId)
            .Select(category => category.CategoryName)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }
}
