using Anvil.Server.Application.UseCases.DnsUseCase.Queries;
using Anvil.Server.Common.Options;
using DnsClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Anvil.Server.Unit.Tests.Application.UseCases.DnsUseCase.Queries;

public class ParsePagesDomainQueryTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("example.com")]
    [InlineData("example.page.example.com")]
    [InlineData("192.168.0.0")]
    public async Task Handle_ShouldReturnNull_WhereDomainIsNotPagesDomain(string domain)
    {
        // Arrange
        var optionsSnapshotMock = Substitute.For<IOptionsSnapshot<Configuration>>();
        optionsSnapshotMock.Value.Returns(new Configuration
        {
            PagesDomain = "example.page",
            RepoApiBaseUrl = "example.com",
            RepoApiToken = "xyz"
        });

        var query = new ParsePagesDomainQuery(DnsString.Parse(domain));
        var handler = new ParsePagesDomainQueryHandler(NullLogger<ParsePagesDomainQueryHandler>.Instance, optionsSnapshotMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhereDomainIsRootPagesDomain()
    {
        // NOTE: the root pages domain has no information on which repo it refers to, and so will need to respond with null.
        //  > The root pages domain can still be used, if it is treated as a custom domain, with a TXT record pointing to a repo.

        // Arrange
        const string pagesDomain = "example.page";

        var optionsSnapshotMock = Substitute.For<IOptionsSnapshot<Configuration>>();
        optionsSnapshotMock.Value.Returns(new Configuration
        {
            PagesDomain = pagesDomain,
            RepoApiBaseUrl = "example.com",
            RepoApiToken = "xyz"
        });

        var query = new ParsePagesDomainQuery(DnsString.Parse(pagesDomain));
        var handler = new ParsePagesDomainQueryHandler(NullLogger<ParsePagesDomainQueryHandler>.Instance, optionsSnapshotMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhereDomainDomainDoesNotParseExactly()
    {
        // NOTE: an endpoint is provided to validate domains for the purposes of on-demand TLS like with Caddy, so we need to make sure that adding more segments to the left of a
        //  valid domain does not parse as acceptable for this endpoint, or there could be an infinite number of valid domains for the same repo which would be given a TLS cert.

        // Arrange
        const string pagesDomain = "example.page";
        const string userPageDomain = $"a.a.repo.user.{pagesDomain}";

        var optionsSnapshotMock = Substitute.For<IOptionsSnapshot<Configuration>>();
        optionsSnapshotMock.Value.Returns(new Configuration
        {
            PagesDomain = pagesDomain,
            RepoApiBaseUrl = "example.com",
            RepoApiToken = "xyz"
        });

        var query = new ParsePagesDomainQuery(DnsString.Parse(userPageDomain));
        var handler = new ParsePagesDomainQueryHandler(NullLogger<ParsePagesDomainQueryHandler>.Instance, optionsSnapshotMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnReference_WhereDomainIsUserRepo()
    {
        // Arrange
        const string pagesDomain = "example.page";
        const string userPageDomain = $"test.{pagesDomain}";

        var optionsSnapshotMock = Substitute.For<IOptionsSnapshot<Configuration>>();
        optionsSnapshotMock.Value.Returns(new Configuration
        {
            PagesDomain = pagesDomain,
            RepoApiBaseUrl = "example.com",
            RepoApiToken = "xyz"
        });

        var query = new ParsePagesDomainQuery(DnsString.Parse(userPageDomain));
        var handler = new ParsePagesDomainQueryHandler(NullLogger<ParsePagesDomainQueryHandler>.Instance, optionsSnapshotMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.RepoRef); // refers to the default branch when null
        Assert.Equal("pages", result.RepoName);
        Assert.Equal("test", result.OwnerUserName);
    }

    [Fact]
    public async Task Handle_ShouldReturnReference_WhereDomainIsSpecificRepo()
    {
        // Arrange
        const string pagesDomain = "example.page";
        const string userPageDomain = $"repo.user.{pagesDomain}";

        var optionsSnapshotMock = Substitute.For<IOptionsSnapshot<Configuration>>();
        optionsSnapshotMock.Value.Returns(new Configuration
        {
            PagesDomain = pagesDomain,
            RepoApiBaseUrl = "example.com",
            RepoApiToken = "xyz"
        });

        var query = new ParsePagesDomainQuery(DnsString.Parse(userPageDomain));
        var handler = new ParsePagesDomainQueryHandler(NullLogger<ParsePagesDomainQueryHandler>.Instance, optionsSnapshotMock);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("pages", result.RepoRef);
        Assert.Equal("repo", result.RepoName);
        Assert.Equal("user", result.OwnerUserName);
    }
}