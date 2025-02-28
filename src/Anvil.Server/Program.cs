using Anvil.Server.Application.Abstractions;
using Anvil.Server.Application.UseCases.SourceUseCase;
using Anvil.Server.Common.Options;
using Anvil.Server.Infrastructure.SourceProviders;
using DnsClient;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateSlimBuilder(args);

// Configuration
builder.Services.AddOptions<Configuration>().ValidateOnStart().Bind(builder.Configuration);
builder.Services.AddSingleton<IValidateOptions<Configuration>, ValidationConfigurationOptions>();

// Infrastructure
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("tls-ignored_dangerous")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true });
builder.Services.AddResponseCaching(); // Default 100 MB response cache size
builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
builder.Services.AddSingleton<IDnsQuery>(_ => new LookupClient(new LookupClientOptions
{
    ThrowDnsErrors = true,
    CacheFailedResults = true,
    MinimumCacheTimeout = TimeSpan.FromSeconds(30),
    FailedResultsCacheDuration = TimeSpan.FromMinutes(10)
}));

// - Source providers
builder.Services.AddScoped<ISourceProvider, ForgejoSourceProvider>();


var app = builder.Build();
app.UseResponseCaching();

app.MapGet("/{**path}", HttpHandlers.GetRoutedSourceMedia);
app.MapGet("/_internal/dns/validate", HttpHandlers.GetInternalDomainValidity);

await app.RunAsync();