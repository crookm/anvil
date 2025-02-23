using DnsClient;
using Mediator;

namespace Anvil.Server.Application.UseCases.DnsUseCase.Queries;

/// <summary>
/// Perform a DNS lookup for a TXT record given a domain.
/// </summary>
/// <remarks>
/// Will not throw for errors, except for input validation errors.
/// </remarks>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="Domain"/> is null.</exception>
/// <param name="Domain">The hostname to query.</param>
public sealed record LookupTxtRecordQuery(DnsString Domain) : IQuery<DnsString?>;

internal sealed partial class LookupTxtRecordQueryHandler : IQueryHandler<LookupTxtRecordQuery, DnsString?>
{
    private readonly ILogger<LookupTxtRecordQueryHandler> _logger;
    private readonly IDnsQuery _dnsQuery;

    public LookupTxtRecordQueryHandler(ILogger<LookupTxtRecordQueryHandler> logger, IDnsQuery dnsQuery)
    {
        _logger = logger;
        _dnsQuery = dnsQuery;
    }

    public async ValueTask<DnsString?> Handle(LookupTxtRecordQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query.Domain);

        try
        {
            Log.LogTraceStarting(_logger, query.Domain);

            var response = await _dnsQuery.QueryAsync(query.Domain, QueryType.TXT, QueryClass.IN, cancellationToken);
            var record = response.Answers.TxtRecords().FirstOrDefault()?.Text.FirstOrDefault();

            Log.LogTraceAnswer(_logger, query.Domain, record);
            return record == null ? null : DnsString.Parse(record);
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
            Message = "Looking up DNS record for {Domain} TXT")]
        internal static partial void LogTraceStarting(ILogger logger, DnsString domain);

        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "DNS lookup for {Domain} TXT answered with: {Record}")]
        internal static partial void LogTraceAnswer(ILogger logger, DnsString domain, string? record);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Failed to lookup DNS record for {Domain} TXT")]
        internal static partial void LogException(ILogger logger, Exception exception, DnsString domain);
    }
}