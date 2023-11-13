using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using CloudComputingA3.Models;
using System.Text.RegularExpressions;

namespace CloudComputingA3.BackgroundServices
{
    public class RouteCheckBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<RouteCheckBackgroundService> _logger;


        public RouteCheckBackgroundService(IServiceProvider services, ILogger<RouteCheckBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
         
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service is running.");
            while (!cancellationToken.IsCancellationRequested)

            {
                await DoWork(cancellationToken);

                _logger.LogInformation("People Background Service is waiting a minute.");

                await Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);
            }
            
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Route Check Background Service is working.");

            try
            {
                var client = new HttpClient();
                //Trigger Lambda function
                HttpResponseMessage response = await client.GetAsync("https://ksbcb7p0e5.execute-api.us-east-1.amazonaws.com/maps/driving-time");
                string travelTime = "";
                int duration = 0;

                if (response.IsSuccessStatusCode)
                {
                    travelTime = await response.Content.ReadAsStringAsync();
                    await StoreTravelTime(travelTime, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error encounted whie checking route: Will attempt again later. ");
            }

            _logger.LogInformation("Route CheckBackground Service complete.");
        }

        private async Task StoreTravelTime(string travelTime, CancellationToken cancellationToken)
        {
            //convert to usable data
            int duration = ConvertToInt(travelTime);
            DateTime currentDateTime = DateTime.UtcNow;
            DateTime date = currentDateTime.Date;
            TimeSpan time = currentDateTime.TimeOfDay;


            string timeString = time.ToString(@"hh\:mm\:ss");
            string dateString = date.ToString("dd/MM/yyyy");

            //If conversion failed, skip storing to DB
            if (duration != Int32.MaxValue)
            {
                var db = new AmazonDynamoDBClient();
                var context = new DynamoDBContext(db);

                var routeItem = new RouteTime
                {
                    Date = dateString,
                    Time = timeString,
                    Duration = duration
                };

                await context.SaveAsync(routeItem);

            }

        }

        private int ConvertToInt(string travelTime)
        {
            int time = 0;

            Regex regex = new Regex(@"(?:(\d+) hour[s]?)?\s*(\d+) min[s]?");

            Match match = regex.Match(travelTime);
            if (!match.Success)
            {
                //Set to max value for error handling
                time = Int32.MaxValue;
                
            } else
            {
                int hours = string.IsNullOrEmpty(match.Groups[1].Value) ? 0 : int.Parse(match.Groups[1].Value);
                int minutes = int.Parse(match.Groups[2].Value);
                time = hours * 60 + minutes;
            }
            return time;
        }

       


    }
}
