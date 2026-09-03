namespace WarehouseFlow.Domain.ValueObjects;

public record Address(
    string Street,
    string HouseNumber,
    string City,
    string State,
    string LandMark
);
