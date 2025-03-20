using Amazon.S3;
using Amazon.S3.Transfer;
using Application.Contracts;
using Application.Models;
using ErrorOr;
using Infrastructure.Errors;

namespace Infrastructure.Clients;
public class AwsClient(IAmazonS3 _amazonS3) : IAwsClient
{
    private const string BucketName = "leo-converted-files-bucket";

    public async Task<ErrorOr<Success>> UploadToS3Async(FileModel fileModel)
    {
        try
        {
            var transferUtility = new TransferUtility(_amazonS3);

            var filePath = GetFilePath(fileModel.FileName);

            await transferUtility.UploadAsync(fileModel.File, BucketName, filePath);

            return Result.Success;
        }
        catch
        {
            return ClientError.AwsError.UploadFailed;
        }
    }

    public async Task<ErrorOr<Stream>> DownloadFromS3Async(string fileName)
    {
        try
        {
            var response = await _amazonS3.GetObjectAsync(BucketName, GetFilePath(fileName));
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
        {
            return ClientError.AwsError.FileNotFound;
        }
        catch (Exception)
        {
            return ClientError.AwsError.DownloadFailed;
        }
    }

    private static string GetFilePath(string fileName) =>
        $"{Path.GetFileNameWithoutExtension(fileName)}/{fileName}";
}