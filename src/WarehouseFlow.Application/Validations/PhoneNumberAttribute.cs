

using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Validations;

public sealed class PhoneNumberAttribute : ValidationAttribute
{
    private static readonly HashSet<string> ValidPrefixes = new()
    {
        "701", "702", "703", "704", "705", "706", "707", "708", "709",
        "802", "803", "804", "805", "806", "807", "808", "809",
        "810", "811", "812", "813", "814", "815", "816", "817", "818", "819",
        "901", "902", "903", "904", "905", "906", "907", "908", "909",
        "911", "912", "913", "915", "916", "917", "918"
    };

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        if (value is not string rawPhone || string.IsNullOrWhiteSpace(rawPhone))
            return new ValidationResult("Phone number is required.");

        var phone = rawPhone.Trim();

        if (phone.StartsWith("+234", StringComparison.Ordinal))
            phone = phone[4..];
        else if (phone.StartsWith("0", StringComparison.Ordinal))
            phone = phone[1..];
        else
            return new ValidationResult("Phone number must start with 0 (local) or +234 (international).");

        if (phone.Length != 10 || !phone.All(char.IsDigit))
            return new ValidationResult("Phone number must contain exactly 10 digits after the prefix.");

        if (!ValidPrefixes.Contains(phone[..3]))
            return new ValidationResult("Phone number is not a valid Nigerian number.");

        return ValidationResult.Success;
    }
}