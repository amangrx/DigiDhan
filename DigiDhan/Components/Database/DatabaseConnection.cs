using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Xml.Linq;

public class DatabaseConnection
{
    string connectionDB = "Data Source=E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db;Version=3;";
    public void CheckForExistingDatabase()
    {
        if (File.Exists("E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db"))
        {
            Console.WriteLine("Database digidhan already exists");
        }
        else
        {
            Console.WriteLine("Database file does not exist.");
            InitializeTable();
            DatabaseValueInsertion databaseValueInsertion = new DatabaseValueInsertion();
            databaseValueInsertion.InsertUsers("amngrx", "aman123", 0);
        }
    }

    public void InitializeTable()
    {
        string createUserTableQuery = @"
            CREATE TABLE users (
                user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL,
                password TEXT NOT NULL,
				balance INTEGER
            );";

        string createIncomeTableQuery = @"
            CREATE TABLE incomes (
                income_id INTEGER PRIMARY KEY AUTOINCREMENT,
				amount INTEGER NOT NULL,
				source TEXT NOT NULL,
				date DATE NOT NULL,
				tags TEXT NOT NULL,
                note TEXT NULL,
                type TEXT NOT NULL
            );";

        string createExpenseTableQuery = @"
            CREATE TABLE expenses (
                exp_id INTEGER PRIMARY KEY AUTOINCREMENT,
				exp_amount INTEGER NOT NULL,
				exp_source TEXT NOT NULL,
				exp_date DATE NOT NULL,
				tags TEXT NOT NULL,
                note TEXT NULL,
                exp_type TEXT NOT NULL
            );";

        string createDebtTableQuery = @"
            CREATE TABLE debt (
                debt_id INTEGER PRIMARY KEY AUTOINCREMENT,
				debt_amount INTEGER NOT NULL,
				debt_source TEXT NOT NULL,
				debt_date DATE NOT NULL,
                due_date DATE NOT NULL,
				tags TEXT NOT NULL,
                note TEXT NULL,
                debt_type TEXT NOT NULL
            );";

        using (SQLiteConnection conn = new SQLiteConnection(connectionDB))
        {
            conn.Open();

            using (SQLiteCommand cmd = new SQLiteCommand(createUserTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (SQLiteCommand cmd = new SQLiteCommand(createIncomeTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (SQLiteCommand cmd = new SQLiteCommand(createExpenseTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (SQLiteCommand cmd = new SQLiteCommand(createDebtTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

}
