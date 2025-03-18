using Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace Application.Converters;
public class PdfToMdConverter : IFileConverter
{
    public Stream Convert(IFormFile file)
    {
        throw new NotImplementedException();
    }
}