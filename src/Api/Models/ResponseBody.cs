namespace Api.Models;
public record ResponseBody(object? Data = null, Dictionary<string, string>? Notifications = null)
{
    public object Data { get; set; } = Data ?? new object();
    public Dictionary<string, string> Notifications { get; set; } = Notifications ?? [];
}