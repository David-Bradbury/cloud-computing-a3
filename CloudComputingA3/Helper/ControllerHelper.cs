using Amazon.S3;
using Amazon.S3.Model;
namespace CloudComputingA3.Helper
{
    public static class ControllerHelper
    {
        public static async Task UploadImage(string filename, Stream imageS, string bucketName)
        {
            IAmazonS3 s3Client = new AmazonS3Client();

            

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = filename,
                InputStream = imageS,
                CannedACL = S3CannedACL.PublicRead,


            };
            try
            {
                await s3Client.PutObjectAsync(request);
            }
            catch (AmazonS3Exception e)
            {

                throw; 
            }
          
        }
    }
}
