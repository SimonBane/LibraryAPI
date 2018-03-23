using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}