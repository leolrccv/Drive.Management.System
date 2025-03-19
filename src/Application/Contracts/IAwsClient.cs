using Application.Models;

namespace Application.Contracts;

public interface IAwsClient
{
    Task UploadToS3Async(FileModel fileModel);
}