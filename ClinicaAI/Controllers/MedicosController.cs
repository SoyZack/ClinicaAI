using Microsoft.AspNetCore.Mvc;

namespace ClinicaAI.Controllers
{
    public class MedicosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
