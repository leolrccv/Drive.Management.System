namespace Application.Extensions;
public static class FileTypes
{
    public const string PdfExtension = ".pdf";
    public const string MdExtension = ".md";
    public const string DocxExtension = ".docx";

    public static IList<string> GetAllowedTypes() => [PdfExtension, MdExtension, DocxExtension];
}