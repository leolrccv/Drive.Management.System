namespace Infrastructure.Models;
public class ClientSettings
{
    public ApiSettings GeminiApi { get; set; }
}

public class ApiSettings
{
    public string ApiKey { get; set; }
    public string BaseAddress { get; set; }
}