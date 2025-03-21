using ErrorOr;

namespace Application.Errors;
internal static class ConverterError
{
    internal static class Docx
    {
        public static Error ToMd => Error.Validation("Converter.DocxToMd");
        public static Error ToPdf => Error.Validation("Converter.DocxToPdf");
    }
    internal static class Md
    {
        public static Error ToDocx => Error.Validation("Converter.MdToDocx");
        public static Error ToPdf => Error.Validation("Converter.MdToPdf");
    }
    internal static class Pdf
    {
        public static Error ToDocx => Error.Validation("Converter.PdfToDocx");
        public static Error ToMd => Error.Validation("Converter.PdfToMd");
    }
}