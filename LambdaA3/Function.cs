using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaA3;

public class Function
{
    private static readonly HttpClient httpClient = new HttpClient();
    private const string apiKey = "AIzaSyAhbTu4AEjff2coqWKSRP6sWVHNoaKCGgc";

    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        string origin = "Kariong, NSW, Australia";
        string destination = "Sydney City, NSW, Australia";
        string requestUri = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(origin)}&destinations={Uri.EscapeDataString(destination)}&key={apiKey}&departure_time=now";

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            if (json["rows"]?.First?["elements"]?.First?["duration_in_traffic"] is JObject durationObject)
            {
                return durationObject["text"]?.ToString() ?? "Duration not available";
            }
            else
            {
                return "No results found";
            }
        }
        catch (HttpRequestException e)
        {
            context.Logger.LogLine($"Request exception: {e.Message}");
            return "Error occurred";
        }
    }
}
