namespace WarehouseFlow.Domain.Exceptions;

public sealed class ValidationException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(string message, IEnumerable<string> errors)
        : base(message)
    {
        Errors = errors.ToArray();
    }
}