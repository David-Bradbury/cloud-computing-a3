using Microsoft.AspNetCore.Mvc;
using CloudComputingA3.Filters;
using CloudComputingA3.Models;
using CloudComputingA3.Helper;
using Newtonsoft.Json.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;


namespace CloudComputingA3.Controllers
{
    public class MainController : Controller
    {
        private string _apiKey = "AIzaSyAhbTu4AEjff2coqWKSRP6sWVHNoaKCGgc";
        private string _destination = "Sydney City, NSW, Australia";
        private string _startingPoint = "Kariong, NSW, Australia";

        [AuthorizeUser]
        public async Task<IActionResult> Index()
        {
            string today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            string yesterday = DateTime.UtcNow.AddDays(-1).ToString("dd/MM/yyyy");

            var todaysAverage = await AverageForGivenDayAsync(today);
            var todaysAverageString = ConvertToHoursMinutesString(todaysAverage);

            var yesterdaysAverage = await AverageForGivenDayAsync(yesterday);
            var yesterdaysAverageString = ConvertToHoursMinutesString(yesterdaysAverage);

            var routesToday = GetLatestRoutesAsync(today);

            //ViewBag.TodaysAverageString = todaysAverageString;
            //ViewBag.YesterdaysAverageString = yesterdaysAverageString;
            //ViewBag.RoutesToday = routesToday;

            var viewModel = new MainPageViewModel();
            viewModel.TodaysAverage = todaysAverageString;
            viewModel.YesterdaysAverage = yesterdaysAverageString;
            viewModel.RouteTimes = await GetLatestRoutesAsync(today);

            return View(viewModel);
        }

        public async Task<double> AverageForGivenDayAsync(string date)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            DynamoDBContext context = new DynamoDBContext(client);

            double averageDuration = 0;

            //string todaysDate = DateTime.UtcNow.ToString("dd/MM/yyyy");

            var conditions = new List<ScanCondition>
            {
                new ScanCondition("Date", ScanOperator.Equal, date)
            };

            var routesToday = await context.ScanAsync<RouteTime>(conditions).GetRemainingAsync();

            if(routesToday.Count == 0)
            {
                return 0;
            }

            averageDuration = routesToday.Average(route => route.Duration);

            //round to 2 deciaml places and return
            return Math.Round(averageDuration, 2);
        }

        public async Task<List<RouteTime>> GetLatestRoutesAsync(string date)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            DynamoDBContext context = new DynamoDBContext(client);

            var conditions = new List<ScanCondition>
            {
                new ScanCondition("Date", ScanOperator.Equal, date)
            };
            var routesForDate = await context.QueryAsync<RouteTime>(date, new DynamoDBOperationConfig
            {
                // Additional configuration if needed
            }).GetRemainingAsync();
            //////////////////////////////////////////////////////////////////////////////////
            return routesForDate.OrderByDescending(route => route.Time).ToList();
        }

        public string ConvertToHoursMinutesString(double mins)
        {
            string result = "";

            int hours = (int)(mins / 60);
            int minutes = (int)(mins % 60);

            result = hours + " hours and " + minutes + " minutes";
            return result;
        }



    }
}
