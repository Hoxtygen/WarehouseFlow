
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations;

public sealed class ProductService(AppDbContext dbContext, ILogger<ProductService> logger)
    : IProductService
{
    private const string SkuCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int RandomSkuLength = 5;
    private const int MaxSkuAttempts = 5;

    public async Task<Product> createProduct(
        NewProductDto newProductDto,
        CancellationToken cancellationToken = default
    )
    {
        var categoryName = await dbContext.ProductCategories
            .Where(category => category.Id == newProductDto.ProductCategoryId)
            .Select(category => category.CategoryName)
            .SingleOrDefaultAsync(cancellationToken);

        if (categoryName is null)
        {
            throw new InvalidOperationException("The selected product category does not exist.");
        }

        for (var attempt = 0; attempt < MaxSkuAttempts; attempt++)
        {
            var sku = GenerateSku(
                newProductDto.Brand,
                categoryName,
                newProductDto.ProductName
            );
            var skuExists = await dbContext.Products.AnyAsync(
                product => product.SKU == sku,
                cancellationToken
            );

            if (skuExists)
            {
                continue;
            }

            var newProduct = new Product
            {
                Description = newProductDto.Description,
                ProductName = newProductDto.ProductName,
                ProductCategoryId = newProductDto.ProductCategoryId,
                UnitPrice = newProductDto.UnitPrice,
                Brand = newProductDto.Brand,
                SKU = sku,
                ImageUrl = newProductDto.ImageUrl,
            };

            dbContext.Products.Add(newProduct);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Product created with SKU {Sku}", sku);
            return newProduct;
        }

        throw new InvalidOperationException("Could not generate a unique product SKU.");
    }

    private static string GenerateSku(string brand, string category, string productName)
    {
        var randomCharacters = new char[RandomSkuLength];

        for (var index = 0; index < randomCharacters.Length; index++)
        {
            randomCharacters[index] = SkuCharacters[
                RandomNumberGenerator.GetInt32(SkuCharacters.Length)
            ];
        }

        return string.Join(
            "-",
            SkuSegment(brand),
            SkuSegment(category),
            SkuSegment(productName),
            new string(randomCharacters)
        );
    }

    private static string SkuSegment(string value)
    {
        var alphanumeric = new string(value.Where(char.IsLetterOrDigit).ToArray());
        return alphanumeric.Length >= 3
            ? alphanumeric[..3].ToUpperInvariant()
            : alphanumeric.ToUpperInvariant().PadRight(3, 'X');
    }
}