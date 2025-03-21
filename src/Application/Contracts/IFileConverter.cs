using Application.Models;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.Contracts;
public interface IFileConverter
{
    Task<ErrorOr<FileModel>> ConvertAsync(IFormFile file);
}