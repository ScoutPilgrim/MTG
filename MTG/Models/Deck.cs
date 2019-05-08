using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace MTG.Models
{
    public class Deck
    {       
        private string _name;
        public string Name { get { return _name; } }

        private int _id;
        public int Id { get { return _id; } }

        public Deck(string name, int id = 0)
        {
            _name = name;
            _id = id;
        }
        public static void ClearAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            
            cmd.CommandText = @"DELETE FROM decks; DELETE FROM cards_in_deck;";
            cmd.ExecuteNonQuery();

            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }
        }

        public void RemoveCard(Card card)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"SELECT count FROM cards_in_deck WHERE deckId = @deckId AND cardId = @cardId;";
            MySqlParameter cardId = new MySqlParameter("@cardId", card.Id);
            MySqlParameter deckId = new MySqlParameter("@deckId", this.Id);
            cmd.Parameters.Add(cardId);
            cmd.Parameters.Add(deckId);

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            int count = 0;
            while(rdr.Read())
            {
                if(rdr.GetInt32(0) > 0)
                {
                    count = rdr.GetInt32(0);
                    count--;
                }
            }
            rdr.Close();
            if(count <= 0)
            {
                cmd.CommandText = @"DELETE FROM cards_in_deck WHERE deckId = @deckId AND cardId = @cardId;";
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd.CommandText = @"UPDATE cards_in_deck SET count = @Count WHERE cardId = @cardId AND deckId = @deckId;";
                MySqlParameter newCount = new MySqlParameter("@Count", count);
                cmd.Parameters.Add(newCount);
                cmd.ExecuteNonQuery();
            }

            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }
        }
        
        
        public void AddCard(Card card)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"SELECT count FROM cards_in_deck WHERE deckId = @deckId AND cardId = @cardId;";
            MySqlParameter cardId = new MySqlParameter("@cardId", card.Id);
            MySqlParameter deckId = new MySqlParameter("@deckId", this.Id);
            cmd.Parameters.Add(cardId);
            cmd.Parameters.Add(deckId);
 
            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            
            bool wasFound = false;
            int count = 0;
            while(rdr.Read())
            {
                if(rdr.HasRows)
                {
                    count = rdr.GetInt32(0);
                    if(count < 4)
                    {
                        count++;
                    }
                    wasFound = true;
                }
            }
            rdr.Close();

            if(wasFound)
            {
                cmd.CommandText = @"UPDATE cards_in_deck SET count = @Count WHERE cardId = @cardId AND deckId = @deckId;";
                MySqlParameter newCount = new MySqlParameter("@Count", count);
                cmd.Parameters.Add(newCount);
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd.CommandText = @"INSERT INTO cards_in_deck (cardId, deckId, count) VALUES (@cardId, @deckId, 1);";
                cmd.ExecuteNonQuery();
            }

            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }
        }

        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"INSERT INTO decks (name) VALUES (@Name);";
            MySqlParameter name = new MySqlParameter("@Name", this.Name);
            cmd.Parameters.Add(name);
      
            cmd.ExecuteNonQuery();
            this._id = (int) cmd.LastInsertedId;

            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }
        }

        public static List<Deck> GetDecks()
        {
            List<Deck> allDecks = new List<Deck>{};

            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            
            cmd.CommandText = @"SELECT * FROM decks;";
            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);

                Deck deck = new Deck(name, id);
                allDecks.Add(deck);
            }
            
            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }

            return allDecks;
        }

        // public Card GetCardInDeck()
        // {
        //     MySqlConnection conn = DB.Connection();
        //     conn.Open();
            
        //     MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
        //     cmd.CommandText = @"SELECT * FROM cards_in_deck WHERE deckId = @deckId AND cardId = @cardId;";
            
        //     MySqlParameter deckId = new MySqlParameter("@deckId", this.Id);
        //     cmd.Parameters.Add(deckId);
        //     MySqlParameter cardId = new MySqlParameter("@cardId", card.Id);
        //     cmd.Parameters.Add(deckId);
            
        //     MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
        //     while(rdr.Read())
        //     {
        //         int cardId= rdr.GetInt32(1);
        //         foundCards.Add(Card.BuildCardObj(cardId));
        //     }
        //     conn.Close();
        //     if(conn != null)
        //     {
        //         conn.Dispose();
        //     }
        //     return foundCards;

        // }

        public List<Card> GetAllCardsInDeck()
        {
            List<Card> foundCards = new List<Card>();
            MySqlConnection conn = DB.Connection();
            conn.Open();
            
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM cards_in_deck WHERE deckId = @deckId;";
            
            MySqlParameter deckId = new MySqlParameter("@deckId", this.Id);
            cmd.Parameters.Add(deckId);
            
            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
                int cardId= rdr.GetInt32(1);
                foundCards.Add(Card.BuildCardObj(cardId));
            }
            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }
            return foundCards;
        }
    }
}