using System.Data.Common;
using System.Data.SQLite;

public class DatabaseExtraction
{
    private SQLiteConnection conn;
    string database = "Data Source=E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db;Version=3;";

    public DatabaseExtraction()
    {
        conn = new SQLiteConnection(database);
        conn.Open();
    }

    public int GetUserBalance(int userId)
    {
        string userBalanceQuery = "SELECT * FROM users WHERE user_id = @userId;";

        using (var cmd = new SQLiteCommand(userBalanceQuery, conn))
        {
            cmd.Parameters.AddWithValue("@userId", userId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetInt32(3);
                }
                return 0;
            }
        }
    }



}