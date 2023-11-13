using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json.Linq;

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


        public static async Task<string> GetRouteTimeAsync(string origin, string destination, string apiKey)
        {

            string requestUri = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(origin)}&destinations={Uri.EscapeDataString(destination)}&key={apiKey}&departure_time=now";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);

                    // Check if the response contains rows
                    if (json["rows"]?.First?["elements"]?.First?["duration"] is JObject durationObject)
                    {
                        string durationText = durationObject["text"]?.ToString();
                        return durationText ?? "Duration not available";
                    }
                    else
                    {
                        return "No results found";
                    }
                }
                catch (HttpRequestException e)
                {
                    // Handle the exception
                    Console.WriteLine($"Request exception: {e.Message}");
                    return "Error occurred";
                }
            }
        }
    }
}
