namespace Anvil.Server.Domain.Models;

public record SourceMediaStreamedModel
{
    public required string Path { get; init; }
    public required Stream Stream { get; init; }
    public DateTimeOffset? LastModified { get; init; }
    public string? ETag { get; init; }
}