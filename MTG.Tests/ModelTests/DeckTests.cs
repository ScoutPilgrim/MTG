using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTG.Models;
using MySql.Data.MySqlClient;

namespace MTG.Tests
{
    [TestClass]
    public class DeckTests : IDisposable
    {
        public void Dispose()
        {
            Deck.ClearAll();
            Card.ClearAll();
        }

        public DeckTests()
        {
            DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=mtg_collection_test;";
        }
        
        [TestMethod]
        public void Save_SavesDeckToDatabase_Deck()
        {
            Deck deck = new Deck("Test Deck");
            deck.Save();

            List<Deck> allDecks = Deck.GetDecks();

            Assert.AreEqual(1, allDecks.Count);
        }

        [TestMethod]
        public void AddCard_CardIsInsertedToCardsInDeckTable_Int()
        {
            Deck deck = new Deck("test deck");
            deck.Save();

            Card card = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com", 1);
            card.Save();

            deck.AddCard(card);
            List<Card> result = deck.GetAllCardsInDeck();

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void AddCard_CardCountIncreasesWhenSameCardIsAdded_Int()
        {
            Deck deck = new Deck("test deck");
            deck.Save();

            Card card = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com", 1);
            card.Save();

            deck.AddCard(card);
            deck.AddCard(card);

            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"SELECT count FROM cards_in_deck WHERE deckId = @DeckId AND cardId = @CardId;";
            MySqlParameter deckId = new MySqlParameter("@DeckId", deck.Id);
            MySqlParameter cardId = new MySqlParameter("@CardId", card.Id);
            cmd.Parameters.Add(deckId);
            cmd.Parameters.Add(cardId);

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            int result = 0;
            while(rdr.Read())
            {
                result = rdr.GetInt32(0);
            }
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void AddCard_CardCountIncreasesToMax4_Int()
        {
            Deck deck = new Deck("test deck");
            deck.Save();

            Card card = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com", 1);
            card.Save();

            deck.AddCard(card);
            deck.AddCard(card);
            deck.AddCard(card);
            deck.AddCard(card);
            deck.AddCard(card);
       
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"SELECT count FROM cards_in_deck WHERE deckId = @DeckId AND cardId = @CardId;";
            MySqlParameter deckId = new MySqlParameter("@DeckId", deck.Id);
            MySqlParameter cardId = new MySqlParameter("@CardId", card.Id);
            cmd.Parameters.Add(deckId);
            cmd.Parameters.Add(cardId);

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            int result = 0;
            while(rdr.Read())
            {
                result = rdr.GetInt32(0);
            }
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void RemoveCard_CardCountInDatabaseDecreases_Int()
        {
            Deck deck = new Deck("test deck");
            deck.Save();

            Card card = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com", 1);
            card.Save();

            deck.AddCard(card);
            deck.AddCard(card);
            deck.AddCard(card);

            deck.RemoveCard(card);

            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"SELECT count FROM cards_in_deck WHERE deckId = @DeckId AND cardId = @CardId;";
            MySqlParameter deckId = new MySqlParameter("@DeckId", deck.Id);
            MySqlParameter cardId = new MySqlParameter("@CardId", card.Id);
            cmd.Parameters.Add(deckId);
            cmd.Parameters.Add(cardId);

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            int result = 0;
            while(rdr.Read())
            {
                result = rdr.GetInt32(0);
            }
            Assert.AreEqual(2, result);

        }

        [TestMethod]
        public void RemoveCard_CardIsDeletedFromDatabaseIfCountZero_Int()
        {
            Deck deck = new Deck("test deck");
            deck.Save();

            Card card = new Card("name", "1B", "black", "creature", "description", "test set", 1, 1, "picture.com", 1);
            card.Save();

            deck.AddCard(card);
            deck.RemoveCard(card);

            List<Card> cards = deck.GetAllCardsInDeck();
            Assert.AreEqual(0, cards.Count);
        }
    }
}