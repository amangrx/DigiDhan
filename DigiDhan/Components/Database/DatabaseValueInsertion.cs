using System.Data.Common;
using System.Data.SQLite;

public class DatabaseValueInsertion
{
    private SQLiteConnection conn;
    string database = "Data Source=E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db;Version=3;";

    public DatabaseValueInsertion()
    {
        conn = new SQLiteConnection(database);
        conn.Open();
    }

    //function to insert the values of user during the login to the database. 
    public void InsertUsers(string name, string password, int balance)
    {
        string insertUserQuery = "INSERT INTO users(username, password, balance) VALUES  (@name, @password, @balance)";
        using (var cmd = new SQLiteCommand(insertUserQuery, conn))
        {
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@balance", balance);
            cmd.ExecuteNonQuery();
        }
    }

    //function to insert the values of income to th edatabase. 
    public int InsertIncome(int amount, string source, DateOnly date, string tags, string note, IncomeType type)
    {
        string insertIncomeQuery = "INSERT INTO incomes (amount, source, date, tags, note, type) VALUES (@amount, @source, @date, @tags, @note, @type)";
        using (var cmd = new SQLiteCommand(insertIncomeQuery, conn))
        {
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@source", source);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@tags", tags);
            cmd.Parameters.AddWithValue("@note", note);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.ExecuteNonQuery();
        }
        long lastInsertId = conn.LastInsertRowId;

        return (int)lastInsertId;
    }
    public void UpdateUserBalance(int userId, int amount)
    {
        string updateUserBalanceQuery = "UPDATE users SET balance = balance + @amount WHERE user_id = @userId";

        using (var cmd = new SQLiteCommand(updateUserBalanceQuery, conn))
        {
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.ExecuteNonQuery();
        }
    }

}