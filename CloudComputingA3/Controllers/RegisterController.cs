using Microsoft.AspNetCore.Mvc;
using CloudComputingA3.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using CloudComputingA3.Helper;

namespace CloudComputingA3.Controllers
{
    [Route("/Register")]
    public class RegisterController : Controller
    {
        [Route("/Register/Register",
        Name = "register")]
        public IActionResult Register()
        {
            var viewModel = new RegisterViewModel();
            return View(viewModel);
        }

        [HttpPost, Route("/Register/Register",
        Name = "register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            var db = new AmazonDynamoDBClient();
            var request = new GetItemRequest
            {
                TableName = "users",
                Key = new Dictionary<string, AttributeValue>
            {
                { "email", new AttributeValue { S = viewModel.Email } }
            },
            };

            var response = await db.GetItemAsync(request);
            if (response.Item.Count != 0)
            {
                ModelState.AddModelError("RegisterFailure", "The email already exists");
                return View(viewModel);
            }

            var scanRequest = new ScanRequest
            {
                TableName = "users",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":usernameVal", new AttributeValue { S = viewModel.Username }}
                },
                FilterExpression = "username = :usernameVal",
            };

            var scanResponse = await db.ScanAsync(scanRequest);
            if (scanResponse.Items.Count > 0)
            {
                // If we find any items, that means the username already exists.
                ModelState.AddModelError("RegisterFailure", "The username already exists");
                return View(viewModel);
            }
            string bucketName = "a3userimages";
            string imageUrl = "https://a3userimages.s3.amazonaws.com/";
            if (viewModel.UserImage != null && viewModel.UserImage.Length > 0)
            {
                string fileExtension = Path.GetExtension(viewModel.UserImage.FileName);
                string uniqueFilename = Guid.NewGuid().ToString() + fileExtension;
                uniqueFilename = uniqueFilename + fileExtension;

                using (var fileStream = viewModel.UserImage.OpenReadStream())
                {

                    await ControllerHelper.UploadImage(uniqueFilename, fileStream, bucketName);
                }
                imageUrl = imageUrl + uniqueFilename;

            }
            else
            {
                imageUrl = imageUrl + "default.png";
            }


            var context = new DynamoDBContext(db);

            var loginItem = new Login
            {
                Email = viewModel.Email,
                Username = viewModel.Username,
                Password = viewModel.Password,
                ImgUrl = imageUrl
            };

            await context.SaveAsync(loginItem);



            return RedirectToAction("Login", "Login");
        }



    }
}
