using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MTG.Models
{
    public class Card
    {
        private int _id;
        public int Id { get { return _id; } }

        private string _name;
        public string Name { get { return _name; } }

        private string _mana;
        public string Mana { get { return _mana; } }
        
        private string _color;
        public string Color { get { return _color; } }
        
        private string _type;
        public string Type { get { return _type; } }
        
        private string _description;
        public string Description { get { return _description; } }
        
        private int _power;
        public int Power { get { return _power; } } 
        
        private int _toughness;
        public int Toughness { get { return _toughness; } }
        
        private string _card_set;
        public string Card_Set { get { return _card_set; } }
        
        private string _image_link;
        public string ImageLink { get { return _image_link; } }


        public Card(string name, string mana, string color, string type, string description, string card_set,
                        int power = -1, int toughness = -1, string image_link = " ", int id = 0)
        {
            _name = name;
            _mana = mana;
            _color = color;
            _type = type;
            _description = description;
            _power = power;
            _toughness = toughness;
            _card_set = card_set;
            _image_link = image_link;
            _id = id;
        }

        public static void ClearAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM cards;";
            cmd.ExecuteNonQuery();

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
            cmd.CommandText = @"INSERT INTO cards (name, mana, color, type, description, power, toughness, card_set, image) 
            VALUES (@Name, @Mana, @Color, @Type, @Description, @Power, @Toughness, @CardSet, @Image);";
            
            MySqlParameter name = new MySqlParameter();
            name.ParameterName = "@Name";
            name.Value = this._name;

            MySqlParameter mana = new MySqlParameter();
            mana.ParameterName = "@Mana";
            mana.Value = this._mana;

            MySqlParameter color = new MySqlParameter();
            color.ParameterName = "@Color";
            color.Value = this._color;

            MySqlParameter type = new MySqlParameter();
            type.ParameterName = "@Type";
            type.Value = this._type;

            MySqlParameter description = new MySqlParameter();
            description.ParameterName = "@Description";
            description.Value = this._description;

            MySqlParameter power = new MySqlParameter();
            power.ParameterName = "@Power";
            power.Value = this._power;

            MySqlParameter toughness = new MySqlParameter();
            toughness.ParameterName = "@Toughness";
            toughness.Value = this._toughness;

            MySqlParameter cardSet = new MySqlParameter();
            cardSet.ParameterName = "@CardSet";
            cardSet.Value = this._card_set;

            MySqlParameter image = new MySqlParameter();
            image.ParameterName = "@Image";
            image.Value = this._image_link;

            cmd.Parameters.Add(name);
            cmd.Parameters.Add(mana);
            cmd.Parameters.Add(color);
            cmd.Parameters.Add(type);
            cmd.Parameters.Add(description);
            cmd.Parameters.Add(power);
            cmd.Parameters.Add(toughness);
            cmd.Parameters.Add(cardSet);
            cmd.Parameters.Add(image);

            cmd.ExecuteNonQuery();

            _id = (int) cmd.LastInsertedId;
                        
            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }
        }

        public static List<Card> GetAll()
        {
            List<Card> allCards = new List<Card>{};

            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM cards;";
            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

            while(rdr.Read())
            {
                // int cId = rdr.GetInt32(0);
                // string cName = rdr.GetString(1);
                // string cMana = rdr.GetString(2);
                // string cColor = rdr.GetString(3);
                // string cType = rdr.GetString(4);
                // string cDescription = rdr.GetString(5);
                // int cPower = rdr.GetInt32(6);
                // int cToughness = rdr.GetInt32(7);
                // string cSet = rdr.GetString(8);
                // string cImage = "";
                // if(!rdr.IsDBNull(9))
                // {
                //     cImage = rdr.GetString(9);
                // }
            
                Card card = Card.ReadCardObj(rdr); //new Card(cName, cMana, cColor, cType, cDescription, cSet, cPower, cToughness, cImage, cId);
                allCards.Add(card);
            }

            conn.Close();           
            if(conn != null)
            {
                conn.Dispose();
            }

            return allCards;
        }

        public static List<Card> Search(string searchColumn, string search)
        {
            List<Card> cards = new List<Card>{};

            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM cards WHERE @SearchColumn LIKE '@Search';";
            MySqlParameter searchStr = new MySqlParameter();
            searchStr.ParameterName = "@Search";
            searchStr.Value = search;
            MySqlParameter searchCol = new MySqlParameter();
            searchCol.ParameterName = "@SearchColumn";
            searchCol.Value = searchColumn;

            cmd.Parameters.Add(searchStr);
            cmd.Parameters.Add(searchCol);

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
                // int cId = rdr.GetInt32(0);
                // string cName = rdr.GetString(1);
                // string cMana = rdr.GetString(2);
                // string cColor = rdr.GetString(3);
                // string cType = rdr.GetString(4);
                // string cDescription = rdr.GetString(5);
                // int cPower = rdr.GetInt32(6);
                // int cToughness = rdr.GetInt32(7);
                // string cSet = rdr.GetString(8);
                // string cImage = rdr.GetString(9);
            
                Card card = Card.ReadCardObj(rdr);//new Card(cName, cMana, cColor, cType, cDescription, cSet, cPower, cToughness, cImage, cId);
                cards.Add(card);
            }

            conn.Close();
            if(conn != null)
            {
                conn.Dispose();
            }

            return cards;
        }


        public override bool Equals(object obj)
        {
            if(!(obj is Card))
            {
                return false;
            }
            else
            {
                Card card = (Card) obj;
                return this.Id == card.Id && this.Name == card.Name && this.Card_Set == card.Card_Set;
            }
        }

        public static Card ReadCardObj(MySqlDataReader rdr)
        {
                int cId = rdr.GetInt32(0);
                string cName = rdr.GetString(1);
                string cMana = rdr.GetString(2);
                string cColor = rdr.GetString(3);
                string cType = rdr.GetString(4);
                string cDescription = rdr.GetString(5);
                int cPower = rdr.GetInt32(6);
                int cToughness = rdr.GetInt32(7);
                string cSet = rdr.GetString(8);
                string cImage = "";
                if(!rdr.IsDBNull(9))
                {
                    cImage = rdr.GetString(9);
                }
            
                return new Card(cName, cMana, cColor, cType, cDescription, cSet, cPower, cToughness, cImage, cId);
        }

        public static Card BuildCardObj(int cardId)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText =  @"SELECT * FROM cards WHERE id = @id;";
            MySqlParameter thisId = new MySqlParameter("@id", cardId);
            cmd.Parameters.Add(thisId);
            MySqlDataReader rdr = cmd.ExecuteReader();
            Card retCard = null;
            while(rdr.Read())
            {
                retCard = Card.ReadCardObj(rdr);
            }
            return retCard;
        }


    }
}
