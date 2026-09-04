using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Exceptions;

namespace WarehouseFlow.Application.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<ProductService> logger
) : IProductService
{
    private const string SkuCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int RandomSkuLength = 5;
    private const int MaxSkuAttempts = 5;

    public async Task<ProductResponse> CreateProduct(
        NewProductDto newProductDto,
        CancellationToken cancellationToken
    )
    {
        var productCategoryName = await productRepository.GetCategoryNameAsync(
            newProductDto.ProductCategoryId
        );

        if (productCategoryName is null)
        {
            logger.LogInformation($"Product category [{productCategoryName}] not found/ is null", productCategoryName);
            throw new InvalidOperationException("The selected product category does not exist.");
        }

        for (var attempt = 0; attempt < MaxSkuAttempts; attempt++)
        {
            var sku = GenerateSku(
                newProductDto.Brand,
                productCategoryName,
                newProductDto.ProductName
            );

            var skuExists = await productRepository.SkuExistsAsync(sku, cancellationToken);

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

            await productRepository.AddAsync(newProduct);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Product [{ProductName}] created with SKU {Sku}", sku, newProduct.ProductName);

            return ProductResponseFactory.FromProduct(newProduct, productCategoryName);
        }
        throw new InvalidOperationException("Could not generate a unique product SKU.");
    }

    public async Task<ProductResponse> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken
    )
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        if (product is null)
        {
            logger.LogError($"Prouct wth ID {productId} not found");
            throw new NotFoundException($"Prouct wth ID {productId} not found");
        }

        return ProductResponseFactory.FromProduct(product);
    }

    public Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken = default)
    {
        return productRepository.ExistsAsync(productId, cancellationToken);
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
