using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
