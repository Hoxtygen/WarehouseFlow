
namespace WarehouseFlow.Domain.Services;
public static class WarehouseCodeGenerator
{
    private static readonly Dictionary<string, string> LocationCodes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Ikeja"] = "IKJ",
            ["Lekki"] = "LEK",
            ["Victoria Island"] = "VIC",
            ["Yaba"] = "YAB",
            ["Surulere"] = "SUR",
            ["Ikorodu"] = "IKD",
            ["Alaba"] = "ALB",
            ["Mushin"] = "MSH",
            ["Agege"] = "AGE",
            ["Badagry"] = "BDG"
        };

    public static string Generate(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException(
                "Warehouse location is required.",
                nameof(location));

        if (!LocationCodes.TryGetValue(location.Trim(), out var locationCode))
            throw new ArgumentException(
                $"Unsupported warehouse location: {location}",
                nameof(location));

        var year = DateTime.UtcNow.Year;

        var uniqueCode = Guid.NewGuid()
            .ToString("N")
            .Substring(0, 6)
            .ToUpperInvariant();

        return $"WH-{locationCode}-{year}-{uniqueCode}";
    }
}