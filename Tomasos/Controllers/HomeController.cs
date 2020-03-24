using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tomasos.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tomasos.Controllers
{
    public class HomeController : Controller
    {
        private readonly TomasosContext _context;

        public HomeController(TomasosContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("Home")]
        public IActionResult Index()
        {
            var model = _context.Matratt.ToList();
            return View(model);
        }


        public IActionResult ViewMeny(int id)
        {
            var model = _context.Matratt.Include(m => m.MatrattProdukt).ThenInclude(mp => mp.Produkt)
                .Where(m => m.MatrattTyp == id).ToList();
            return PartialView("_Details", model);
        }

        [Route("About")]
        public IActionResult About()
        {
            return View();
        }
    }
}