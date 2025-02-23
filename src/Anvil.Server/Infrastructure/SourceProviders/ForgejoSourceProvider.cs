using Anvil.Server.Application.Abstractions;
using Anvil.Server.Common.Options;
using Anvil.Server.Domain.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Web;

namespace Anvil.Server.Infrastructure.SourceProviders;

internal class ForgejoSourceProvider : ISourceProvider
{
    private readonly HttpClient _httpClient;

    public ForgejoSourceProvider(IOptionsSnapshot<Configuration> configurationSnapshot, HttpClient httpClient)
    {
        _httpClient = httpClient;

        var configuration = configurationSnapshot.Value;
        _httpClient.BaseAddress = new Uri(configuration.RepoApiBaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", configuration.RepoApiToken);
    }

    public async ValueTask<SourceMediaStreamedModel?> GetMediaAsync(RepositoryReferenceModel repo, string path,
        CancellationToken cancellationToken = default)
    {
        var uri = $"/api/v1/repos/{repo.OwnerUserName}/{repo.RepoName}/media/{HttpUtility.UrlEncode(path)}";
        if (!string.IsNullOrWhiteSpace(repo.RepoRef)) uri += $"?ref={HttpUtility.UrlEncode(repo.RepoRef)}";

        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        return new SourceMediaStreamedModel
        {
            Path = path,
            Stream = await response.Content.ReadAsStreamAsync(cancellationToken),
            LastModified = response.Content.Headers.LastModified,
            ETag = response.Headers.ETag?.Tag
        };
    }
}