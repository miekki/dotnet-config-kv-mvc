namespace api.Models;

public record MyAppSettings
{
    public string MySecret { get; init; } = string.Empty;
    public string Variable1 { get; init; } = string.Empty;
}

// public record MyAppSettings(string MySecret = "", string Variable1 = "");