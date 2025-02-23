using Anvil.Server.Domain.Models;

namespace Anvil.Server.Application.Abstractions;

public interface ISourceProvider
{
    ValueTask<SourceMediaStreamedModel?> GetMediaAsync(RepositoryReferenceModel repo, string path, CancellationToken cancellationToken = default);
}