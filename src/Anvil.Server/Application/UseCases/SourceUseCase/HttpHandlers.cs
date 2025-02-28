using Anvil.Server.Application.UseCases.FilesystemUseCase.Queries;
using Anvil.Server.Application.UseCases.SourceUseCase.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Anvil.Server.Application.UseCases.SourceUseCase;

internal static class HttpHandlers
{
    public static async ValueTask<IResult> GetRoutedSourceMedia(
        HttpContext context,
        [FromRoute] string? path,
        [FromServices] Mediator.Mediator mediator,
        CancellationToken cancellationToken)
    {
        var domain = context.Request.Host.Value;
        path ??= "/";

        try
        {
            var repo = await mediator.Send(new GetSourceRepoByDomainQuery(domain), cancellationToken);
            if (repo == null) throw new HttpRequestException("Unable to find requested repository reference.", null, HttpStatusCode.NotFound);

            var media = await mediator.Send(new GetSourceRoutedMediaQuery(repo, path), cancellationToken);
            var mimeType = await mediator.Send(new GetMimeTypeByPathQuery(media.Path), cancellationToken);
            var eTag = media.ETag == null ? null : new EntityTagHeaderValue(media.ETag);

            context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=600, stale-while-revalidate=300";
            return Results.Stream(media.Stream, mimeType, null, media.LastModified, eTag);
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=60";
            return Results.NotFound();
        }
        catch (HttpRequestException e) when (e.StatusCode is not null)
        {
            return Results.StatusCode((int)e.StatusCode);
        }
    }

    public static async ValueTask<IResult> GetInternalDomainValidity(
        HttpContext context,
        [FromQuery] string domain,
        [FromServices] Mediator.Mediator mediator,
        CancellationToken cancellationToken)
    {
        domain = domain.ToLowerInvariant().Trim();

        try
        {
            var repo = await mediator.Send(new GetSourceRepoByDomainQuery(domain), cancellationToken);
            if (repo == null) return Results.NotFound();

            if (await mediator.Send(new GetSourceRepoValidityQuery(repo), cancellationToken))
                return Results.Ok();

            return Results.NotFound();
        }
        catch
        {
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}