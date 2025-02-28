using Anvil.Server.Application.Abstractions;
using Anvil.Server.Domain.Models;
using Mediator;

namespace Anvil.Server.Application.UseCases.SourceUseCase.Queries;

public sealed record GetSourceRepoValidityQuery(RepositoryReferenceModel Repository) : IQuery<bool>;

internal sealed partial class GetSourceRepoValidityQueryHandler : IQueryHandler<GetSourceRepoValidityQuery, bool>
{
    private readonly ILogger<GetSourceRepoValidityQueryHandler> _logger;
    private readonly ISourceProvider _sourceProvider;

    public GetSourceRepoValidityQueryHandler(ILogger<GetSourceRepoValidityQueryHandler> logger, ISourceProvider sourceProvider)
    {
        _logger = logger;
        _sourceProvider = sourceProvider;
    }

    public async ValueTask<bool> Handle(GetSourceRepoValidityQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Repository.OwnerUserName);
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Repository.RepoName);

        if (!string.IsNullOrWhiteSpace(query.Repository.RepoRef))
            ArgumentException.ThrowIfNullOrWhiteSpace(query.Repository.RepoRef);

        try
        {
            return await _sourceProvider.IsRepoAvailableAsync(query.Repository, cancellationToken);
        }
        catch (Exception e)
        {
            Log.LogException(_logger, e, query.Repository.OwnerUserName, query.Repository.RepoName, query.Repository.RepoRef);
            throw;
        }
    }


    internal static partial class Log
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Unable to get repo validity for reference: {RepoOwner}/{RepoName}@{RepoRef}")]
        internal static partial void LogException(ILogger logger, Exception exception, string repoOwner, string repoName, string? repoRef);
    }
}