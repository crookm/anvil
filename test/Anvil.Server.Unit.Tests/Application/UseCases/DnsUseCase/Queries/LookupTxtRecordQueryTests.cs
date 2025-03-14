using Anvil.Server.Application.UseCases.DnsUseCase.Queries;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute.ExceptionExtensions;

namespace Anvil.Server.Unit.Tests.Application.UseCases.DnsUseCase.Queries;

public class LookupTxtRecordQueryTests
{
    [Fact]
    public async Task Handle_ShouldReturnNull_WhereDomainNotExist()
    {
        // Arrange
        const string domain = "example.com";

        var dnsQueryMock = Substitute.For<IDnsQuery>();
        dnsQueryMock.QueryAsync(Arg.Any<string>(), QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>())
            .ThrowsAsync(new DnsResponseException(DnsResponseCode.NotExistentDomain));

        var query = new LookupTxtRecordQuery(DnsString.Parse(domain));
        var handler = new LookupTxtRecordQueryHandler(NullLogger<LookupTxtRecordQueryHandler>.Instance, dnsQueryMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
        await dnsQueryMock.Received()
            .QueryAsync($"{domain}.", QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhereDomainHasNoTxtRecords()
    {
        // Arrange
        const string domain = "example.com";

        var dnsResponseMock = Substitute.For<IDnsQueryResponse>();
        dnsResponseMock.Answers.Returns([]);

        var dnsQueryMock = Substitute.For<IDnsQuery>();
        dnsQueryMock.QueryAsync(Arg.Any<string>(), QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(dnsResponseMock));

        var query = new LookupTxtRecordQuery(DnsString.Parse(domain));
        var handler = new LookupTxtRecordQueryHandler(NullLogger<LookupTxtRecordQueryHandler>.Instance, dnsQueryMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
        await dnsQueryMock.Received()
            .QueryAsync($"{domain}.", QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnParsableEntries_WhereDomainHasTxtRecords()
    {
        // Arrange
        const string domain = "example.com";

        var dnsResponseMock = Substitute.For<IDnsQueryResponse>();
        dnsResponseMock.Answers.Returns([
            new TxtRecord(
                new ResourceRecordInfo(domain, ResourceRecordType.TXT, QueryClass.IN, 10, 10),
                [], ["first.example.com", "..."]),
            new TxtRecord(
                new ResourceRecordInfo(domain, ResourceRecordType.TXT, QueryClass.IN, 10, 10),
                [], ["second.example.com"])
        ]);

        var dnsQueryMock = Substitute.For<IDnsQuery>();
        dnsQueryMock.QueryAsync(Arg.Any<string>(), QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(dnsResponseMock));

        var query = new LookupTxtRecordQuery(DnsString.Parse(domain));
        var handler = new LookupTxtRecordQueryHandler(NullLogger<LookupTxtRecordQueryHandler>.Instance, dnsQueryMock);

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).Select(x => x.Value).ToArray();

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Contains("first.example.com.", result);
        Assert.Contains("second.example.com.", result);
        Assert.DoesNotContain("...", result);

        await dnsQueryMock.Received()
            .QueryAsync($"{domain}.", QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnLimitedEntries_WhereDomainHasManyTxtRecords()
    {
        // Arrange
        const string domain = "example.com";

        var txtValues = new List<string>();
        for (var i = 0; i < LookupTxtRecordQueryHandler.MaxTxtRecords + 1; i++)
            txtValues.Add($"{i}.{domain}");

        var dnsResponseMock = Substitute.For<IDnsQueryResponse>();
        dnsResponseMock.Answers.Returns([
            new TxtRecord(
                new ResourceRecordInfo(domain, ResourceRecordType.TXT, QueryClass.IN, 10, 10),
                [], txtValues.ToArray())
        ]);

        var dnsQueryMock = Substitute.For<IDnsQuery>();
        dnsQueryMock.QueryAsync(Arg.Any<string>(), QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(dnsResponseMock));

        var query = new LookupTxtRecordQuery(DnsString.Parse(domain));
        var handler = new LookupTxtRecordQueryHandler(NullLogger<LookupTxtRecordQueryHandler>.Instance, dnsQueryMock);

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).Select(x => x.Value).ToArray();

        // Assert
        Assert.Equal(LookupTxtRecordQueryHandler.MaxTxtRecords, result.Length);

        await dnsQueryMock.Received()
            .QueryAsync($"{domain}.", QueryType.TXT, QueryClass.IN, Arg.Any<CancellationToken>());
    }
}