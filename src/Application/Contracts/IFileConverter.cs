using Application.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Contracts;
public interface IFileConverter
{
    Task<FileModel> ConvertAsync(IFormFile file);
}