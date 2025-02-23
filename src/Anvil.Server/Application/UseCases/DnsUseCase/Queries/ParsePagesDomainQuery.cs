using Anvil.Server.Common.Options;
using Anvil.Server.Domain.Models;
using DnsClient;
using Mediator;
using Microsoft.Extensions.Options;

namespace Anvil.Server.Application.UseCases.DnsUseCase.Queries;

/// <summary>
/// Query to parse a <paramref name="Domain"/> into a reference of a repository.
/// </summary>
/// <remarks>
/// Must be in the form of the default pages domain from configuration, or this query will return null. Custom domains should be resolved first.
/// </remarks>
/// <param name="Domain">The domain name to parse into a repository.</param>
public sealed record ParsePagesDomainQuery(DnsString Domain) : IQuery<RepositoryReferenceModel?>;

internal sealed partial class ParsePagesDomainQueryHandler : IQueryHandler<ParsePagesDomainQuery, RepositoryReferenceModel?>
{
    private readonly ILogger<ParsePagesDomainQueryHandler> _logger;
    private readonly Configuration _configuration;

    public ParsePagesDomainQueryHandler(ILogger<ParsePagesDomainQueryHandler> logger, IOptionsSnapshot<Configuration> configurationSnapshot)
    {
        _logger = logger;
        _configuration = configurationSnapshot.Value;
    }

    public ValueTask<RepositoryReferenceModel?> Handle(ParsePagesDomainQuery query, CancellationToken cancellationToken)
    {
        var domain = query.Domain.ToString().TrimEnd('.');
        Log.LogTraceStarting(_logger, domain);

        var result = CoreParse(domain);
        Log.LogTraceResult(_logger, domain, result?.OwnerUserName, result?.RepoName, result?.RepoRef);

        return new ValueTask<RepositoryReferenceModel?>(result);
    }

    private RepositoryReferenceModel? CoreParse(string domain)
    {
        if (!domain.EndsWith(_configuration.PagesDomain, StringComparison.OrdinalIgnoreCase))
            return null;

        var rootDomainSegments = _configuration.PagesDomain.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var domainSegments = domain.Split('.', StringSplitOptions.RemoveEmptyEntries);

        domainSegments = domainSegments[..^rootDomainSegments.Length];

        if (domainSegments.Length == 0) return null;

        // Root 'pages' repo for a user (i.e. matt.example.com -> matt/pages@<default>)
        if (domainSegments.Length == 1) return new RepositoryReferenceModel { OwnerUserName = domainSegments[0], RepoName = "pages", RepoRef = null };

        // Specific repo for a user (i.e. blog.matt.example.com -> matt/blog@pages)
        return new RepositoryReferenceModel { OwnerUserName = domainSegments[^1], RepoName = domainSegments[^2], RepoRef = "pages" };
    }


    internal static partial class Log
    {
        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "Parsing domain name into repo details for {Domain}")]
        internal static partial void LogTraceStarting(ILogger logger, string domain);

        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "Parsed domain {Domain} into repo reference {RepoOwner}/{RepoName}@{RepoRef}")]
        internal static partial void LogTraceResult(ILogger logger, string domain, string? repoOwner, string? repoName, string? repoRef);
    }
}