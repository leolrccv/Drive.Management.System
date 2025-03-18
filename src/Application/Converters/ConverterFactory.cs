using Application.Contracts;
using Application.Extensions;

namespace Application.Converters;
public class ConverterFactory
{
    public static List<IFileConverter> CreateStrategies(string fileName) =>
        Path.GetExtension(fileName) switch
        {
            FileTypes.MdExtension => [new MdToDocConverter()],
            FileTypes.PdfExtension => [new PdfToDocConverter()],
            FileTypes.DocxExtension => [new DocToPdfConverter()],
            _ => throw new NotImplementedException(),
        };
}