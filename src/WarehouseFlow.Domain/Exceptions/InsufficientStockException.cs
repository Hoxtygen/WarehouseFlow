namespace WarehouseFlow.Domain.Exceptions;

public class InsufficientStockException(string message) : Exception(message);
