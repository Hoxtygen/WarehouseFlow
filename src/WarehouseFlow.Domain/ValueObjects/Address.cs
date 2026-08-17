namespace WarehouseFlow.Domain.ValueObjects;

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string LandMark { get; set; } = string.Empty;

    protected Address() { }

    public Address(string street, string houseNumber, string city, string state, string landMark)
    {
        Street = street;
        HouseNumber = houseNumber;
        City = city;
        State = state;
        LandMark = landMark;
    }

    public override bool Equals(object? obj)
    {
        if(obj is not Address other) return false;
        return Street == other.Street && 
        HouseNumber == other.HouseNumber && 
        City == other.City && 
        State == other.State && 
        LandMark == other.LandMark;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, HouseNumber, City, State, LandMark);
    }
}