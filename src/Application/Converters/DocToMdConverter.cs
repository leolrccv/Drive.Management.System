using Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace Application.Converters;
public class DocToMdConverter : IFileConverter
{
    public Stream Convert(IFormFile file)
    {
        throw new NotImplementedException();
    }
}