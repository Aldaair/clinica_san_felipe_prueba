namespace KardexQueryService.Application.Common.Settings;

public sealed class DownstreamServicesSettings
{
    public string ProductServiceBaseUrl { get; init; } = string.Empty;
    public string MovementServiceBaseUrl { get; init; } = string.Empty;
}