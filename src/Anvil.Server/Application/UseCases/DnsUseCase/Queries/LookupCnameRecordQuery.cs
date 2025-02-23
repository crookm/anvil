using DnsClient;
using Mediator;

namespace Anvil.Server.Application.UseCases.DnsUseCase.Queries;

/// <summary>
/// Perform a DNS lookup for a CNAME record given a domain.
/// </summary>
/// <remarks>
/// Will not throw for errors, except for input validation errors.
/// </remarks>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="Domain"/> is null.</exception>
/// <param name="Domain">The hostname to query.</param>
public sealed record LookupCnameRecordQuery(DnsString Domain) : IQuery<DnsString?>;

internal sealed partial class LookupCnameRecordQueryHandler : IQueryHandler<LookupCnameRecordQuery, DnsString?>
{
    private readonly ILogger<LookupCnameRecordQuery> _logger;
    private readonly IDnsQuery _dnsQuery;

    public LookupCnameRecordQueryHandler(ILogger<LookupCnameRecordQuery> logger, IDnsQuery dnsQuery)
    {
        _logger = logger;
        _dnsQuery = dnsQuery;
    }

    public async ValueTask<DnsString?> Handle(LookupCnameRecordQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query.Domain);

        try
        {
            Log.LogTraceStarting(_logger, query.Domain);

            var response = await _dnsQuery.QueryAsync(query.Domain, QueryType.CNAME, QueryClass.IN, cancellationToken);
            var record = response.Answers.CnameRecords().FirstOrDefault()?.CanonicalName;

            Log.LogTraceAnswer(_logger, query.Domain, record);
            return record;
        }
        catch (Exception e)
        {
            Log.LogException(_logger, e, query.Domain);
            return null;
        }
    }


    internal static partial class Log
    {
        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "Looking up DNS record for {Domain} CNAME")]
        internal static partial void LogTraceStarting(ILogger logger, DnsString domain);

        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "DNS lookup for {Domain} CNAME answered with: {Record}")]
        internal static partial void LogTraceAnswer(ILogger logger, DnsString domain, DnsString? record);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Failed to lookup DNS record for {Domain} CNAME")]
        internal static partial void LogException(ILogger logger, Exception exception, DnsString domain);
    }
}