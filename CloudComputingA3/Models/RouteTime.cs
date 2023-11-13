using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudComputingA3.Models
{
    [DynamoDBTable("routes")]
    public class RouteTime
    {
        [DynamoDBHashKey("date")]

        public string Date { get; set; }

        [DynamoDBRangeKey("time")]

        public string Time { get; set; }


        [DynamoDBProperty("duration")]
        public int Duration { get; set; }

    }
}
