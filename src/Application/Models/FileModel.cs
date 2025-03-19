namespace Application.Models;
public record FileModel(string FileName, Stream? File = null, string? Content = null);