using Microsoft.AspNetCore.Mvc;
using CloudComputingA3.Filters;

namespace CloudComputingA3.Controllers
{
    public class MainController : Controller
    {
        [AuthorizeUser]
        public IActionResult Index()
        {
            return View();
        }
    }
}
