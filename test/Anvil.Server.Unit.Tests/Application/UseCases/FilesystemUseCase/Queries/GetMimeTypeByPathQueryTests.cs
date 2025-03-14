using Anvil.Server.Application.UseCases.FilesystemUseCase.Queries;
using Microsoft.AspNetCore.StaticFiles;

namespace Anvil.Server.Unit.Tests.Application.UseCases.FilesystemUseCase.Queries;

public class GetMimeTypeByPathQueryTests
{
    [Fact]
    public async Task Handle_ShouldReturnOctetStream_WherePathHasNoExtension()
    {
        // Arrange
        const string path = "something/else";
        var query = new GetMimeTypeByPathQuery(path);
        var handler = new GetMimeTypeByPathQueryHandler(new FileExtensionContentTypeProvider());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("application/octet-stream", result);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnOctetStream_WhereExtensionIsUnknown()
    {
        // Arrange
        const string path = "weird.asdfjklsm";
        var query = new GetMimeTypeByPathQuery(path);
        var handler = new GetMimeTypeByPathQueryHandler(new FileExtensionContentTypeProvider());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("application/octet-stream", result);
    }

    [Theory]
    [InlineData("test.txt", "text/plain")]
    [InlineData("test.htm", "text/html")]
    [InlineData("test.html", "text/html")]
    [InlineData("test.js", "text/javascript")]
    [InlineData("test.json", "application/json")]
    [InlineData("test.css", "text/css")]
    [InlineData("test.png", "image/png")]
    [InlineData("test.gif", "image/gif")]
    [InlineData("test.ico", "image/x-icon")]
    [InlineData("something/test.txt", "text/plain")]
    public async Task Handle_ShouldReturnExpectedContentType_WherePathHasExtension(string path, string expectedContentType)
    {
        // Arrange
        var query = new GetMimeTypeByPathQuery(path);
        var handler = new GetMimeTypeByPathQueryHandler(new FileExtensionContentTypeProvider());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedContentType, result);
    }
}