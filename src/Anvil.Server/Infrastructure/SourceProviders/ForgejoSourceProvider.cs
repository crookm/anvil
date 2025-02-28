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

    public async ValueTask<bool> IsRepoAvailableAsync(RepositoryReferenceModel repo, CancellationToken cancellationToken = default)
    {
        // A different endpoint is used based on the repository reference (branch) being null or not.
        // > This is because null infers default branch, so it is easier to just check if the repo exists in this case, rather than checking for the default branch (duh).
        var uri = string.IsNullOrWhiteSpace(repo.RepoRef)
            ? $"/api/v1/repos/{repo.OwnerUserName}/{repo.RepoName}" // Default branch, check for repo existence
            : $"/api/v1/repos/{repo.OwnerUserName}/{repo.RepoName}/branches/{repo.RepoRef}"; // Specified branch, check for branch existence

        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
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