namespace Api.Models;
public record ResponseBody(object? Data = null, IEnumerable<string>? Notifications = null)
{
    public object Data { get; set; } = Data ?? new object();
    public IEnumerable<string> Notifications { get; set; } = Notifications ?? [];
}