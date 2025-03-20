using Application.Models;
using ErrorOr;

namespace Application.Contracts;

public interface IAwsClient
{
    Task<ErrorOr<Success>> UploadToS3Async(FileModel fileModel);
    Task<ErrorOr<Stream>> DownloadFromS3Async(string fileName);
}