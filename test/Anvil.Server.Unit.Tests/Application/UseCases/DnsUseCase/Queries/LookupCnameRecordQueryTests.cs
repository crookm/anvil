using Anvil.Server.Application.UseCases.DnsUseCase.Queries;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute.ExceptionExtensions;
using System.Net;

namespace Anvil.Server.Unit.Tests.Application.UseCases.DnsUseCase.Queries;

public class LookupCnameRecordQueryTests
{
    [Fact]
    public async Task Handle_ShouldReturnNull_WhereDomainNotExist()
    {
        // Arrange
        const string domain = "example.com";

        var dnsQueryMock = Substitute.For<IDnsQuery>();
        dnsQueryMock.QueryAsync(Arg.Any<string>(), QueryType.CNAME, QueryClass.IN, Arg.Any<CancellationToken>())
            .ThrowsAsync(new DnsResponseException(DnsResponseCode.NotExistentDomain));

        var query = new LookupCnameRecordQuery(DnsString.Parse(domain));
        var handler = new LookupCnameRecordQueryHandler(NullLogger<LookupCnameRecordQuery>.Instance, dnsQueryMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        await dnsQueryMock.Received()
            .QueryAsync($"{domain}.", QueryType.CNAME, QueryClass.IN, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhereDomainHasNoCnameRecord()
    {
        // Arrange
        const string domain = "example.com";

        var dnsResponseMock = Substitute.For<IDnsQueryResponse>();
        dnsResponseMock.Answers.Returns([
            new ARecord(
                new ResourceRecordInfo(domain, ResourceRecordType.A, QueryClass.IN, 10, 10),
                IPAddress.Loopback)
        ]);

        var dnsQueryMock = Substitute.For<IDnsQuery>();
        dnsQueryMock.QueryAsync(Arg.Any<string>(), QueryType.CNAME, QueryClass.IN, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(dnsResponseMock));

        var query = new LookupCnameRecordQuery(DnsString.Parse(domain));
        var handler = new LookupCnameRecordQueryHandler(NullLogger<LookupCnameRecordQuery>.Instance, dnsQueryMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        await dnsQueryMock.Received()
            .QueryAsync($"{domain}.", QueryType.CNAME, QueryClass.IN, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFirstEntry_WhereDomainHasCnameRecords()
    {
        // Arrange
        const string domain = "example.com";

        var dnsResponseMock = Substitute.For<IDnsQueryResponse>();
        dnsResponseMock.Answers.Returns([
            new CNameRecord(
                new ResourceRecordInfo(domain, ResourceRecordType.CNAME, QueryClass.IN, 10, 10),
                DnsString.Parse("first.example.com.")),
            new CNameRecord(
                new ResourceRecordInfo(domain, ResourceRecordType.CNAME, QueryClass.IN, 10, 10),
                DnsString.Parse("other.example.com."))
        ]);

        var dnsQueryMock = Substitute.For<IDnsQuery>();
        dnsQueryMock.QueryAsync(Arg.Any<string>(), QueryType.CNAME, QueryClass.IN, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(dnsResponseMock));

        var query = new LookupCnameRecordQuery(DnsString.Parse(domain));
        var handler = new LookupCnameRecordQueryHandler(NullLogger<LookupCnameRecordQuery>.Instance, dnsQueryMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("first.example.com.", result);

        await dnsQueryMock.Received()
            .QueryAsync($"{domain}.", QueryType.CNAME, QueryClass.IN, Arg.Any<CancellationToken>());
    }
}