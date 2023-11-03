using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace CloudComputingA3.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {

            var db = new AmazonDynamoDBClient();


            var request = new GetItemRequest
            {
                TableName = "users",
                Key = new Dictionary<string, AttributeValue>
            {
                { "email", new AttributeValue { S = email } }
            },
            };

            try
            {
                var response = await db.GetItemAsync(request);

                if (response.Item != null && response.Item.TryGetValue("password", out var passwordValue))
                {

                    string storedPassword = passwordValue.S;
                    response.Item.TryGetValue("username", out var storedUsername);

                    if (password == storedPassword)
                    {

                        HttpContext.Session.SetString("Username", storedUsername.S);
                        HttpContext.Session.SetString("Email", email);

                        return RedirectToAction("Index", "Main");
                    }
                    else
                    {
                        ModelState.AddModelError("LoginFailure", "Email or password is invalid.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("LoginFailure", "Error communicating with database: " + ex);
                return View("Login");
            }



            ModelState.AddModelError("LoginFailure", "ID or password is invalid.");
            return View();
        }

        // Attempt Logout
        [Route("LoggingOut")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Login");
        }
    }
}
