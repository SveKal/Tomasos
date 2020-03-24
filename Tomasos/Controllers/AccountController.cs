using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tomasos.Models;

namespace Tomasos.Controllers
{
    public class AccountController : Controller
    {
        private readonly TomasosContext _context;

        public AccountController(TomasosContext context)
        {
            _context = context;
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "You looged out";
            return RedirectToAction("LogIn");
        }

        // GET: /<controller>/
        [Route("MemberPage")]
        public IActionResult LogIn()
        {
            return View();
        }

        [Route("MemberPage")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult LogIn(Kund user)
        {
            Kund savedUsers;
            var errorCode = CheckUser(user);
            switch (errorCode)
            {
                case 1:
                {
                    ViewBag.Message = "Username or E-Mail wrong";
                    return View();
                }
                case 2:
                {
                    ViewBag.Message = "You are already logged in";
                    return View();
                }
                case 3:
                {
                    ViewBag.Message = "Something went wrong. Do you have an account?";
                    return View();
                }
                case 4:
                {
                    savedUsers = new Kund();
                    savedUsers = _context.Kund.SingleOrDefault(n => n.AnvandarNamn.Equals(user.AnvandarNamn));
                    var values = JsonConvert.SerializeObject(savedUsers);
                    HttpContext.Session.SetString("Users", values);
                    return RedirectToAction("Food", "Order");
                }
                default: return View();
            }
        }

        public int CheckUser(Kund user)
        {
            var currentCustomer = _context.Kund.SingleOrDefault(n => n.AnvandarNamn.Equals(user.AnvandarNamn));
            if (currentCustomer != null)
            {
                var isUser = currentCustomer.Losenord.Equals(user.Losenord);
                var usersValues = HttpContext.Session.GetString("Users");

                if (isUser && usersValues == null)
                    return 4;
                if (!isUser && usersValues == null)
                    return 1;
                if (isUser && usersValues != null)
                    return 2;
                return 3;
            }

            return 3;
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyEmail(string email)
        {
            var isEmail = _context.Kund.SingleOrDefault(k => k.Email.Equals(email));
            var isAvailble = HttpContext.Session.GetString("Users");
            if (isAvailble != null)
            {
                var currentUser = JsonConvert.DeserializeObject<Kund>(isAvailble);
                var correctUser = currentUser.AnvandarNamn.Equals(isEmail.AnvandarNamn);
                if (isEmail != null && correctUser == false)
                    return Json($"Email {email} is already in use.");
                return Json(true);
            }

            if (isAvailble == null)
            {
                if (isEmail != null)
                    return Json($"Email {email} is already in use.");
                return Json(true);
            }

            return Json(true);
        }

        [Route("Bli-medlem")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Register(Kund newCustomer)
        {
            var isEmail = _context.Kund.SingleOrDefault(k => k.Email.Equals(newCustomer.Email));
            var isUserName = _context.Kund.SingleOrDefault(k => k.AnvandarNamn.Equals(newCustomer.AnvandarNamn));
            var isNotExisting = isEmail == null && isUserName == null;
            if (isNotExisting)
            {
                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Account saved. You can log in.";
                    _context.Kund.Add(newCustomer);
                    _context.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("LogIn");
                }

                ViewBag.Message = "Something went wrong. Try again.";
                return View();
            }

            TempData["Message"] = "Username/E-Mail already registered. Please try to log in.";

            return RedirectToAction("LogIn");
        }

        public IActionResult Account()
        {
            var currentUser = new Kund();
            var isAvailble = HttpContext.Session.GetString("Users");
            if (isAvailble != null)
            {
                currentUser = JsonConvert.DeserializeObject<Kund>(isAvailble);
                return View(currentUser);
            }

            ViewBag.Message = "You have to be logged in to use this page.";
            return View(currentUser);
        }

        [AutoValidateAntiforgeryToken]
        public IActionResult SaveChanges(Kund values)
        {
            if (ModelState.IsValid)
            {
                var kundDb = _context.Kund.SingleOrDefault(k => k.KundId == values.KundId);
                _context.Entry(kundDb).CurrentValues.SetValues(values);
                _context.SaveChanges();

                var updatedValues = JsonConvert.SerializeObject(values);
                HttpContext.Session.SetString("Users", updatedValues);

                return RedirectToAction("Account");
            }

            ViewBag.Message = "Error";
            return RedirectToAction("Account");
        }
    }
}