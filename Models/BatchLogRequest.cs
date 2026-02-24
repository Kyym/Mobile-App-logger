namespace MobileAppLogger.Models;

public class BatchLogRequest
{
    public List<LogEntry> Logs { get; set; } = new();
    public string? SessionId { get; set; }
    public DateTime ClientTimestamp { get; set; }
}