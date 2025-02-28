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
public sealed record LookupTxtRecordQuery(DnsString Domain) : IQuery<DnsString[]>;

internal sealed partial class LookupTxtRecordQueryHandler : IQueryHandler<LookupTxtRecordQuery, DnsString[]>
{
    private readonly ILogger<LookupTxtRecordQueryHandler> _logger;
    private readonly IDnsQuery _dnsQuery;

    private const int MaxTxtRecords = 8;

    public LookupTxtRecordQueryHandler(ILogger<LookupTxtRecordQueryHandler> logger, IDnsQuery dnsQuery)
    {
        _logger = logger;
        _dnsQuery = dnsQuery;
    }

    public async ValueTask<DnsString[]> Handle(LookupTxtRecordQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query.Domain);

        try
        {
            Log.LogTraceStarting(_logger, query.Domain);

            var response = await _dnsQuery.QueryAsync(query.Domain, QueryType.TXT, QueryClass.IN, cancellationToken);
            var rawRecords = response.Answers.TxtRecords().SelectMany(r => r.Text).ToArray();
            if (rawRecords.Length > MaxTxtRecords)
                Log.LogTraceExceededMaxCount(_logger, query.Domain, rawRecords.Length, MaxTxtRecords);

            var records = new List<DnsString>();
            foreach (var rawRecord in rawRecords.Take(MaxTxtRecords))
            {
                var record = rawRecord.ToLowerInvariant().Trim();
                try
                {
                    records.Add(DnsString.Parse(record));
                }
                catch (Exception e)
                {
                    Log.LogTraceParseException(_logger, e, query.Domain, record);
                }
            }

            Log.LogTraceAnswer(_logger, query.Domain, string.Join(", ", records.ToString()));
            return records.ToArray();
        }
        catch (Exception e)
        {
            Log.LogException(_logger, e, query.Domain);
            return [];
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
            Message = "DNS lookup for {Domain} TXT answered with: {Records}")]
        internal static partial void LogTraceAnswer(ILogger logger, DnsString domain, string records);

        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "DNS lookup for {Domain} TXT answered with {TotalRecords} records, taking the first {MaxRecords} records")]
        internal static partial void LogTraceExceededMaxCount(ILogger logger, DnsString domain, int totalRecords, int maxRecords);

        [LoggerMessage(
            Level = LogLevel.Trace,
            Message = "Failed to parse DNS record for {Domain} TXT: {Record}")]
        internal static partial void LogTraceParseException(ILogger logger, Exception exception, DnsString domain, string record);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Failed to lookup DNS record for {Domain} TXT")]
        internal static partial void LogException(ILogger logger, Exception exception, DnsString domain);
    }
}