using Anvil.Server.Application.Abstractions;
using Anvil.Server.Domain.Models;
using Mediator;
using System.Net;

namespace Anvil.Server.Application.UseCases.SourceUseCase.Queries;

public sealed record GetSourceRoutedMediaQuery(RepositoryReferenceModel Repository, string Path) : IQuery<SourceMediaStreamedModel>;

internal sealed class GetSourceRoutedMediaQueryHandler : IQueryHandler<GetSourceRoutedMediaQuery, SourceMediaStreamedModel>
{
    private readonly ISourceProvider _sourceProvider;

    public GetSourceRoutedMediaQueryHandler(ISourceProvider sourceProvider)
    {
        _sourceProvider = sourceProvider;
    }

    public async ValueTask<SourceMediaStreamedModel> Handle(GetSourceRoutedMediaQuery query, CancellationToken cancellationToken)
    {
        var path = query.Path;
        if (Path.EndsInDirectorySeparator(path) || !Path.HasExtension(path))
        {
            // If the path ends in directory separator, it is referring to a folder - short circuit to default index.html.
            path = Path.Combine(path, "index.html");

            try
            {
                var result = await _sourceProvider.GetMediaAsync(query.Repository, path, cancellationToken);
                if (result is not null) return result;
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
            }
        }

        try
        {
            // Direct lookup of the file.
            var result = await _sourceProvider.GetMediaAsync(query.Repository, path, cancellationToken);
            if (result is not null) return result;
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
        }

        throw new HttpRequestException("Unable to satisfy routed media request.", null, HttpStatusCode.NotFound);
    }
}