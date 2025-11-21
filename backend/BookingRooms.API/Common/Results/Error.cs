namespace BookingRooms.API.Common.Results;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new("", "");

    public static Error NotFound(string entityName, object? key = null) =>
        new("NotFound", key is null
            ? $"{entityName} was not found."
            : $"{entityName} with key '{key}' was not found.");

    public static Error Validation(string description) =>
        new("ValidationError", description);

    public static Error Conflict(string description) =>
        new("Conflict", description);

    public static Error Failure(string description) =>
        new("Failure", description);
    
    public static Error Unauthorized(string description) =>
        new("Unauthorized", description);
}