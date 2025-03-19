namespace Infrastructure.Models;

public class AwsSettings
{
    public Credentials Credentials { get; set; }
}

public class Credentials
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
}