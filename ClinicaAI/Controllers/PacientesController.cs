using Microsoft.AspNetCore.Mvc;

namespace ClinicaAI.Controllers
{
    public class PacientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
