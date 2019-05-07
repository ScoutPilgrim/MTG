using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTG.Models;
using MySql.Data.MySqlClient;

namespace MTG.Tests
{
    [TestClass]
    public class CardTests : IDisposable
    {
        public void Dispose()
        {
            Card.ClearAll();
        }

        public CardTests()
        {
            DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=mtg_collection_test;";
        }

        [TestMethod]
        public void Equals_ReturnsTrueIfCardIdDescriptionAndSetAreTheSame_Card()
        {
            Card card1 = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com", 1);
            Card card2 = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com", 1);

            Assert.AreEqual(card1, card2);
        }

        [TestMethod]
        public void Save_AssignsIdToSavedCard_Id()
        {
            Card card1 = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com");
            card1.Save();

            List<Card> allCards = Card.GetAll();
            Card testCard = allCards[0];

            Assert.AreEqual(testCard.Id, card1.Id);
        }

        [TestMethod]
        public void Save_SaveCardToDatabase_CardList()
        {
            Card card = new Card("chad", "1G", "green", "creature", "drinks beer", "test set", 10, 3, "picture.com");
            card.Save();

            List<Card> cards = Card.GetAll();
        
            Assert.AreEqual(cards.Count, 1);

        }

        [TestMethod]
        public void GetAll_RetrievesAllCardsFromDatabase_CardList()
        {
            Card card = new Card("test name", "1B", "black", "creature", "badass", "test set", 69, 420, "picture.com");
            card.Save();
            
            Card card2 = new Card("test name2", "1W", "white", "creature", "badass", "test set", 69, 420, "picture.com");
            card2.Save();

            List<Card> testCards = new List<Card> {card, card2};
            List<Card> result = Card.GetAll();
                                  
            CollectionAssert.AreEqual(result, testCards);
        }
    }
}