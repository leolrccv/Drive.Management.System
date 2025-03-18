namespace Api.Providers;

public interface IErrorProvider
{
    public string GetErrorMessage(string code, string description);
}