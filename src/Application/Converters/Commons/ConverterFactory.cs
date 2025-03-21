using Application.Contracts;
using Application.Converters.FromDocx;
using Application.Converters.FromMd;
using Application.Converters.FromPdf;
using Application.Extensions;

namespace Application.Converters.Commons;
public class ConverterFactory
{
    public static List<IFileConverter> CreateStrategies(string fileName) =>
        Path.GetExtension(fileName) switch
        {
            FileTypes.MdExtension => [new MdToDocxConverter(), new MdToPdfConverter()],
            FileTypes.PdfExtension => [new PdfToDocxConverter(), new PdfToMdConverter()],
            FileTypes.DocxExtension => [new DocxToPdfConverter(), new DocxToMdConverter()],
            _ => []
        };
}