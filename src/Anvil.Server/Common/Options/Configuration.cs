using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Anvil.Server.Common.Options;

[ExcludeFromCodeCoverage]
internal sealed class Configuration
{
    [Required] public required string PagesDomain { get; set; }
    [Required] public required string RepoApiBaseUrl { get; set; }
    [Required] public required string RepoApiToken { get; set; }
    
    public string? RepoApiHostOverride { get; set; }
    public bool RepoApiDangerouslyIgnoreTlsErrors { get; set; } = false;
}