using Amazon.S3;
using Amazon.S3.Transfer;
using Application.Contracts;
using Application.Models;

namespace Infrastructure.Clients;
public class AwsClient(IAmazonS3 _amazonS3) : IAwsClient
{
    private const string BucketName = "leo-converted-files-bucket";

    public async Task UploadToS3Async(FileModel fileModel)
    {
        var transferUtility = new TransferUtility(_amazonS3);

        var filePath = $"{Path.GetFileNameWithoutExtension(fileModel.FileName)}/{fileModel.FileName}";

        await transferUtility.UploadAsync(fileModel.File, BucketName, filePath);
    }
}