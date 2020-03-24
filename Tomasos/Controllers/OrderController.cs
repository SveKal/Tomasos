using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tomasos.Models;
using Tomasos.ViewModels;

namespace Tomasos.Controllers
{
    public class OrderController : Controller
    {
        private readonly TomasosContext _context;

        public OrderController(TomasosContext context)
        {
            _context = context;
        }

        public IActionResult Food()
        {
            var model = new ViewModelOrder();
            model.DishList = _context.Matratt.ToList();
            model.CategoryList = _context.MatrattTyp.ToList();
            return View(model);
        }

        public IActionResult ViewFoodToOrder(int id)
        {
            var model = _context.Matratt.Include(m => m.MatrattProdukt).ThenInclude(mp => mp.Produkt)
                .Where(m => m.MatrattTyp == id).ToList();
            return PartialView("_Dishes", model);
        }

        public IActionResult AddToOrder(int id)
        {
            var currentFood = _context.Matratt.SingleOrDefault(n => n.MatrattId == id);
            List<Matratt> foodToOrder;

            if (HttpContext.Session.GetString("Order") == null)
            {
                foodToOrder = new List<Matratt>();
            }
            else
            {
                var existingValues = HttpContext.Session.GetString("Order");
                foodToOrder = JsonConvert.DeserializeObject<List<Matratt>>(existingValues);
            }

            foodToOrder.Add(currentFood);
            var values = JsonConvert.SerializeObject(foodToOrder);
            HttpContext.Session.SetString("Order", values);
            return RedirectToAction("Food");
        }

        public IActionResult CartView()
        {
            var model = new OrderCheckOut();

            var user = HttpContext.Session.GetString("Users");
            var orderValues = HttpContext.Session.GetString("Order");

            if (user == null)
            {
                ViewBag.Message = "You have to be logged in to use this page.";
                return RedirectToAction("LogIn", "Account");
            }

            if (user != null && orderValues == null) return RedirectToAction("Food");

            model.ListOfMatrattToOrder = GetKopplingsTabell();
            model.ShoppingUser = JsonConvert.DeserializeObject<Kund>(user);

            model.TodoBestallning.Totalbelopp = GetSum(model);


            return View(model);
        }

        public List<BestallningMatratt> GetKopplingsTabell()
        {
            var model = new OrderCheckOut();
            var orderValues = HttpContext.Session.GetString("Order");
            var matratter = JsonConvert.DeserializeObject<List<Matratt>>(orderValues);
            foreach (var mat in matratter)
            {
                var isAlready = model.ListOfMatrattToOrder.SingleOrDefault(p => p.MatrattId == mat.MatrattId);
                if (isAlready == null)
                {
                    var antal = _context.Matratt.Count(m => m.MatrattId == mat.MatrattId);
                    var koppling = new BestallningMatratt();
                    koppling.MatrattId = mat.MatrattId;
                    koppling.Antal = antal;
                    model.ListOfMatrattToOrder.Add(koppling);
                }
                else
                {
                    var koppling = model.ListOfMatrattToOrder.SingleOrDefault(m => m.MatrattId == mat.MatrattId);
                    koppling.Antal = koppling.Antal + 1;
                }
            }

            return model.ListOfMatrattToOrder;
        }

        public int GetSum(OrderCheckOut model)
        {
            var sum = new List<int>();
            var priset = 0;
            foreach (var ratt in model.ListOfMatrattToOrder)
            {
                var dish = _context.Matratt.SingleOrDefault(p => p.MatrattId == ratt.MatrattId);
                model.FoodOrder.Add(dish);
                priset = dish.Pris * ratt.Antal;
                sum.Add(priset);
            }

            var totalBelopp = sum.Sum();

            return model.TodoBestallning.Totalbelopp = totalBelopp;
            ;
        }

        public IActionResult RemovefromOrder(int id)
        {
            var model = new OrderCheckOut();

            var orderValues = HttpContext.Session.GetString("Order");
            var foodToOrder = JsonConvert.DeserializeObject<List<Matratt>>(orderValues);

            var isAlready = foodToOrder.First(p => p.MatrattId == id);

            foodToOrder.Remove(isAlready);
            var values = JsonConvert.SerializeObject(foodToOrder);
            HttpContext.Session.SetString("Order", values);

            return RedirectToAction("CartView");
        }

        public IActionResult CheckOut()
        {
            var model = new OrderCheckOut();
            model.TodoBestallning.BestallningDatum = DateTime.Now;
            var user = HttpContext.Session.GetString("Users");
            var currentUser = JsonConvert.DeserializeObject<Kund>(user);
            model.ShoppingUser = _context.Kund.SingleOrDefault(k => k.AnvandarNamn.Equals(currentUser.AnvandarNamn));
            model.TodoBestallning.KundId = model.ShoppingUser.KundId;
            model.ListOfMatrattToOrder = GetKopplingsTabell();
            model.TodoBestallning.Totalbelopp = GetSum(model);
            _context.Add(model.TodoBestallning);
            _context.SaveChanges();
            var currentOrder = _context.Bestallning.OrderByDescending(d => d.BestallningDatum)
                .Where(k => k.KundId == model.ShoppingUser.KundId).Take(1);
            foreach (var matratt in model.ListOfMatrattToOrder)
            {
                matratt.BestallningId = model.TodoBestallning.BestallningId;
                _context.Add(matratt);
                _context.SaveChanges();
            }

            return View(model);
        }
    }
}