using Microsoft.AspNetCore.Mvc;
using CloudComputingA3.Filters;
using CloudComputingA3.Helper;
using Newtonsoft.Json.Linq;


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

            //if (response.IsSuccessStatusCode)
            //{
            //    travelTime = await response.Content.ReadAsStringAsync();
            //}

            //ViewBag.TravelTime = travelTime;
            return View();
        }

    }
}
