
using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudComputingA3.Models
{
    [DynamoDBTable("users")]
    public class Login
    {

        [DynamoDBHashKey("email")]
        [Required, Display(Name = "Email")]
        public string Email { get; set; }

        [DynamoDBProperty("username")]
        [Required, Display(Name = "Username")]
        public string Username { get; set; }


        [DynamoDBProperty("password")]
        [Required, Display(Name = "Password")]
        public string Password { get; set; }

        [DynamoDBProperty("img_url")]
        public string ImgUrl    { get; set; }


         

    }
}
