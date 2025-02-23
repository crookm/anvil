using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Anvil.Server.Common.Options;

[OptionsValidator]
[ExcludeFromCodeCoverage]
internal partial class ValidationConfigurationOptions : IValidateOptions<Configuration>;