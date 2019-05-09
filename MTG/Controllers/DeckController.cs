using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MTG.Models;

namespace MTG.Controllers
{
    public class DeckController : Controller
    {
        
        [HttpGet("/decks")]
        public ActionResult Index()
        {
            List<Deck> allDecks = Deck.GetDecks();
            return View(allDecks);
        }

        [HttpPost("/decks/new")]
        public ActionResult Create(string name)
        {
            Deck deck = new Deck(name);
            deck.Save();

            return RedirectToAction("Index");
        }
    }
}