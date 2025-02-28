using Anvil.Server.Domain.Models;

namespace Anvil.Server.Application.Abstractions;

public interface ISourceProvider
{
    ValueTask<bool> IsRepoAvailableAsync(RepositoryReferenceModel repo, CancellationToken cancellationToken = default);
    ValueTask<SourceMediaStreamedModel?> GetMediaAsync(RepositoryReferenceModel repo, string path, CancellationToken cancellationToken = default);
}