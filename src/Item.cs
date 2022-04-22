﻿using System;
using System.Data.SQLite;

namespace management_system
{
    public class Item
    {
        private static int _instanceId;
        private int Id { get; init; }
        private string Name { get; init; }
        private int Amount { get; set; }

        private int MinAmount { get; set; }

        public Item(string name, int amount, int minAmount)
        {
            Id = _instanceId;
            Name = name;
            Amount = amount;
            MinAmount = minAmount;
            _instanceId++;
        }

        public void AddToDatabase(Database db)
        {
            string query = 
                "INSERT INTO items ('id', 'name', 'amount', 'min_amount') VALUES (@id, @name, @amount, @min)";
            string getAllNames = "SELECT name FROM items";
            
            SQLiteCommand allNamesCommand = new SQLiteCommand(getAllNames, db.Connection);
            SQLiteCommand command = new SQLiteCommand(query, db.Connection);
            
            db.Connection.Open();
            SQLiteDataReader namesReader = allNamesCommand.ExecuteReader();
            while (namesReader.Read())
            {
                if (Name == Convert.ToString(namesReader["name"]))
                    throw new Exception("You have element of that name in your database");
            }

            command.Parameters.AddWithValue("@id", Id);
            command.Parameters.AddWithValue("@name", Name);
            command.Parameters.AddWithValue("@amount", Amount);
            command.Parameters.AddWithValue("@min", MinAmount);
            
            command.ExecuteNonQuery();
            db.Connection.Close();
        }

        public void ReduceItem(string name, int amount, Database db)
        {
            string query = $"SELECT amount FROM items WHERE name={name}";
            SQLiteCommand command = new SQLiteCommand(query, db.Connection);
            
            db.Connection.Open();
            amount -= Convert.ToInt32(command.ExecuteScalar());
            
            if (amount < 0)
                throw new Exception("You haven't that amount of this item in your database");
            if (amount <= MinAmount)
                Console.WriteLine("Amount of this product reached his minimum amount");
            
            string updateQuery = $"UPDATE items SET name={amount} WHERE name={name}";
            SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, db.Connection);
            
            updateCommand.ExecuteNonQuery();
            db.Connection.Close();
        }
    }
}