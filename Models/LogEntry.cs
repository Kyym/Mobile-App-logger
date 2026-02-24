namespace MobileAppLogger.Models;

public class LogEntry
{
    public string Level { get; set; } = "Information";
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Screen { get; set; }
    public string? DeviceInfo { get; set; }
    public string? AppVersion { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}