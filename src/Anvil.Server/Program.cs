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
builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
builder.Services.AddResponseCaching(options => options.SizeLimit = 1024 * 1024 * 100); // 100 MB response cache
builder.Services.AddMemoryCache(options => options.SizeLimit = 1024 * 1024 * 10); // 10 MB operational cache

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
app.Use(async (context, next) =>
{
    // Security headers
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=(), payment=()");
    
    await next.Invoke();
});

app.UseResponseCaching();

app.MapGet("/{**path}", HttpHandlers.GetRoutedSourceMedia);
app.MapGet("/_internal/dns/validate", HttpHandlers.GetInternalDomainValidity);

await app.RunAsync();