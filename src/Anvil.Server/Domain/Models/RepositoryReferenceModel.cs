namespace Anvil.Server.Domain.Models;

/// <summary>
/// A reference to a repository.
/// </summary>
public sealed record RepositoryReferenceModel
{
    /// <summary>
    /// The username of the owner of the repo.
    /// </summary>
    /// <example>matt</example>
    public required string OwnerUserName { get; init; }

    /// <summary>
    /// The name of the repo.
    /// </summary>
    /// <example>anvil</example>
    public required string RepoName { get; init; }

    /// <summary>
    /// The name of the commit/branch/tag in the repo.
    /// </summary>
    /// <remarks>
    /// Defaults to the <c>pages</c> branch.
    /// </remarks>
    public string? RepoRef { get; init; }
}