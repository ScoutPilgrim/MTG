using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MTG.Models;

namespace MTG.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public ActionResult Index()
        {
            List<Card> allCards = Card.GetAll();
            return View(allCards);
        }

        [HttpPost("/create")]
        public ActionResult Create(string name, string mana, string type, string color, string description, int power, int toughness, string set, string image)
        {
            Card card = new Card(name, mana, color, type, description, set, power, toughness, image);
            card.Save();

            return RedirectToAction("Index");
        }

        [HttpPost("/search")]
        public ActionResult Show(string search, string column)
        {

            Console.WriteLine("search: "+search);
            Console.WriteLine("column: "+column);
            

            //List<Card> list = Card.ShowColor(color);
            List<Card> searchResults = Card.Search(column, search);
            return View(searchResults);
        }
    }
}