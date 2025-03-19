using Application.Contracts;
using Application.Extensions;

namespace Application.Converters;
public class ConverterFactory
{
    public static List<IFileConverter> CreateStrategies(string fileName) =>
        Path.GetExtension(fileName) switch
        {
            FileTypes.MdExtension => [new MdToDocConverter(), new MdToPdfConverter()],
            FileTypes.PdfExtension => [new PdfToDocConverter(), new PdfToMdConverter()],
            FileTypes.DocxExtension => [new DocToPdfConverter(), new DocToMdConverter()],
            _ => throw new NotImplementedException(),
        };
}