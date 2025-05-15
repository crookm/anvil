using Anvil.Server.Application.Abstractions;
using Anvil.Server.Domain.Models;
using Mediator;
using Microsoft.Extensions.Caching.Memory;

namespace Anvil.Server.Application.UseCases.SourceUseCase.Queries;

public sealed record GetSourceRepoValidityQuery(RepositoryReferenceModel Repository) : IQuery<bool>;

internal sealed partial class GetSourceRepoValidityQueryHandler : IQueryHandler<GetSourceRepoValidityQuery, bool>
{
    private readonly ILogger<GetSourceRepoValidityQueryHandler> _logger;
    private readonly ISourceProvider _sourceProvider;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public GetSourceRepoValidityQueryHandler(ILogger<GetSourceRepoValidityQueryHandler> logger,
        ISourceProvider sourceProvider, IMemoryCache cache)
    {
        _logger = logger;
        _sourceProvider = sourceProvider;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(GetSourceRepoValidityQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Repository.OwnerUserName);
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Repository.RepoName);

        if (!string.IsNullOrWhiteSpace(query.Repository.RepoRef))
            ArgumentException.ThrowIfNullOrWhiteSpace(query.Repository.RepoRef);

        var cacheKey = $"RepoValidity:{query.Repository.OwnerUserName}:{query.Repository.RepoName}:{query.Repository.RepoRef ?? "<main>"}";
        if (_cache.TryGetValue(cacheKey, out bool isValid))
            return isValid;

        try
        {
            var result = await _sourceProvider.IsRepoAvailableAsync(query.Repository, cancellationToken);
            _cache.Set(cacheKey, result, CacheDuration);

            return result;
        }
        catch (Exception e)
        {
            Log.LogException(_logger, e, query.Repository.OwnerUserName, query.Repository.RepoName,
                query.Repository.RepoRef);
            throw;
        }
    }


    internal static partial class Log
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Unable to get repo validity for reference: {RepoOwner}/{RepoName}@{RepoRef}")]
        internal static partial void LogException(ILogger logger, Exception exception, string repoOwner,
            string repoName, string? repoRef);
    }
}