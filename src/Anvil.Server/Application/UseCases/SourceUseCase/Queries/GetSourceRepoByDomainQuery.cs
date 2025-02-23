using Anvil.Server.Application.Abstractions;
using Anvil.Server.Application.UseCases.DnsUseCase.Queries;
using Anvil.Server.Domain.Models;
using DnsClient;
using Mediator;
using System.Text;

namespace Anvil.Server.Application.UseCases.SourceUseCase.Queries;

/// <summary>
/// Query to get the details of a source repository, given a domain name.
/// </summary>
/// <remarks>
/// This query will first attempt to parse the <paramref name="Domain"/> given the form <c>[repo.]user.default-domain</c>, and then attempt to fallback on a TXT DNS lookup, followed by a CNAME DNS lookup.
/// </remarks>
/// <param name="Domain">The domain name of the pages request to parse into a repository reference.</param>
public sealed record GetSourceRepoByDomainQuery(string Domain) : IQuery<RepositoryReferenceModel?>
{
    public string Domain { get; } = Domain.Trim().TrimEnd('.').ToLowerInvariant();
}

internal sealed partial class GetSourceRepoByDomainQueryHandler : IQueryHandler<GetSourceRepoByDomainQuery, RepositoryReferenceModel?>
{
    private readonly ILogger<GetSourceRepoByDomainQueryHandler> _logger;
    private readonly ISourceProvider _sourceProvider;
    private readonly Mediator.Mediator _mediator;

    private const int MaxCustomDomainCheckLines = 128;

    public GetSourceRepoByDomainQueryHandler(ILogger<GetSourceRepoByDomainQueryHandler> logger, ISourceProvider sourceProvider, Mediator.Mediator mediator)
    {
        _logger = logger;
        _sourceProvider = sourceProvider;
        _mediator = mediator;
    }

    public async ValueTask<RepositoryReferenceModel?> Handle(GetSourceRepoByDomainQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Domain);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(query.Domain.Length, 3);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(query.Domain.Length, 255);

        var domain = DnsString.Parse(query.Domain);
        Log.LogTraceStarting(_logger, domain);

        // Attempt to parse a default pages domain directly
        var result = await _mediator.Send(new ParsePagesDomainQuery(domain), cancellationToken);
        if (result is not null) return result;

        // Lookup TXT record for a link to a repo
        var record = await _mediator.Send(new LookupTxtRecordQuery(domain), cancellationToken);
        if (record is not null)
        {
            result = await _mediator.Send(new ParsePagesDomainQuery(record), cancellationToken);
            if (result is not null && await IsCustomDomainValid(domain, result, cancellationToken))
                return result;
        }

        // Lookup CNAME record for a link to a repo
        record = await _mediator.Send(new LookupCnameRecordQuery(domain), cancellationToken);
        if (record is not null)
        {
            result = await _mediator.Send(new ParsePagesDomainQuery(record), cancellationToken);
            if (result is not null && await IsCustomDomainValid(domain, result, cancellationToken))
                return result;
        }

        Log.LogInfoFailed(_logger, domain);
        return null;
    }

    private async ValueTask<bool> IsCustomDomainValid(DnsString customDomain, RepositoryReferenceModel repo, CancellationToken cancellationToken)
    {
        try
        {
            var domain = customDomain.ToString().TrimEnd('.');
            var media = await _sourceProvider.GetMediaAsync(repo with { RepoRef = null }, ".domains", cancellationToken);
            if (media is null) return false;

            var linesChecked = 0;
            using var reader = new StreamReader(media.Stream, Encoding.UTF8);
            while (!reader.EndOfStream || linesChecked < MaxCustomDomainCheckLines)
            {
                linesChecked++;

                var line = await reader.ReadLineAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(line) && line.Trim().Equals(domain, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }


    internal static partial class Log
    {
        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "Looking up repo details for domain name {Domain}")]
        internal static partial void LogTraceStarting(ILogger logger, DnsString domain);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Unable to find repo details for domain name {Domain}")]
        internal static partial void LogInfoFailed(ILogger logger, DnsString domain);
    }
}