using Microsoft.AspNetCore.Http;

namespace Application.Contracts;
public interface IFileConverter
{
    Stream Convert(IFormFile file);
}