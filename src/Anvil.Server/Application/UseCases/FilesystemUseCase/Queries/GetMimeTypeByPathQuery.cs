using Mediator;
using Microsoft.AspNetCore.StaticFiles;

namespace Anvil.Server.Application.UseCases.FilesystemUseCase.Queries;

public sealed record GetMimeTypeByPathQuery(string Path) : IQuery<string>;

internal sealed class GetMimeTypeByPathQueryHandler : IQueryHandler<GetMimeTypeByPathQuery, string>
{
    private readonly IContentTypeProvider _contentTypeProvider;

    private const int MaxPathLength = 2048;

    public GetMimeTypeByPathQueryHandler(IContentTypeProvider contentTypeProvider)
    {
        _contentTypeProvider = contentTypeProvider;
    }

    public ValueTask<string> Handle(GetMimeTypeByPathQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Path);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(query.Path.Length, MaxPathLength);

        var extension = Path.GetExtension(query.Path);
        if (string.IsNullOrWhiteSpace(extension)) return ValueTask.FromResult("application/octet-stream");

        if (_contentTypeProvider.TryGetContentType(query.Path, out var contentType))
            return ValueTask.FromResult(contentType);

        return ValueTask.FromResult("application/octet-stream");
    }
}