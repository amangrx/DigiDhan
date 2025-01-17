using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Xml.Linq;

public class DatabaseConnection
{
    string connectionDB = "Data Source=E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db;Version=3;";
    
    //method that checks for existing database and if there is no database
    //then it calls the initialize table function that creates the table.
    public void CheckForExistingDatabase()
    {
        if (File.Exists("E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db"))
        {
            Debug.WriteLine("Database digidhan already exists");
        }
        else
        {
            Debug.WriteLine("Database file does not exist.");
            InitializeTable();//calling the InitializeTable function. 
            DatabaseValueInsertion databaseValueInsertion = new DatabaseValueInsertion();
            databaseValueInsertion.InsertUsers("amngrx", "aman123", 0);//inserting the users
        }
    }

    //method that contains create table queries.
    //it runs the queries and creates the corresponding table. 
    public void InitializeTable()
    {
        //User table
        string createUserTableQuery = @"
            CREATE TABLE users (
                user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL,
                password TEXT NOT NULL,
				balance INTEGER
            );";

        //Income table
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

        //Expense table
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

        //Debt table
        string createDebtTableQuery = @"
            CREATE TABLE debt (
                debt_id INTEGER PRIMARY KEY AUTOINCREMENT,
				debt_amount INTEGER NOT NULL,
                outstanding_amt INTEGER NOT NULL, 
				debt_source TEXT NOT NULL,
				debt_date DATE NOT NULL,
                due_date DATE NOT NULL,
				tags TEXT NOT NULL,
                note TEXT NULL,
                debt_type TEXT NOT NULL
            );";

        //Tags table
        string createTagTableQuery = @"
            CREATE TABLE tags(
                tag_id INTEGER PRIMARY KEY AUTOINCREMENT,
                tag_name TEXT NOT NULL
            );";

        //code to run the queries in command
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

            using (SQLiteCommand cmd = new SQLiteCommand(createTagTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

}
