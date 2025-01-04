
using System.Data.Common;
using System.Data.SQLite;
using System.Transactions;

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

    public List<Transaction> GetTransactions()
    {
        List<Transaction> transactions = new List<Transaction>();

        string incomeQuery = "SELECT amount, source, date, tags, note FROM incomes";
        using (var cmd = new SQLiteCommand(incomeQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    transactions.Add(new Transaction("Income", 
                        reader.GetInt32(0),
                        reader.GetString(1), 
                        DateOnly.Parse(reader.GetString(2)), 
                        reader.GetString(3),
                        reader.IsDBNull(4) ? "No Note" : reader.GetString(4)));
                }
            }
        }

        // Retrieve Expense Transactions
        string expenseQuery = "SELECT exp_amount, exp_source, exp_date, tags, note FROM expenses";
        using (var cmd = new SQLiteCommand(expenseQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    transactions.Add(new Transaction("Expense", 
                        reader.GetInt32(0), 
                        reader.GetString(1), 
                        DateOnly.Parse(reader.GetString(2)), 
                        reader.GetString(3),
                        reader.IsDBNull(4) ? "No Note" : reader.GetString(4)));
                }
            }
        }

        // Retrieve Debt Transactions
        string debtQuery = "SELECT debt_amount, debt_source, debt_date, tags, note FROM debt";
        using (var cmd = new SQLiteCommand(debtQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    transactions.Add(new Transaction("Debt", 
                        reader.GetInt32(0), reader.GetString(1), 
                        DateOnly.Parse(reader.GetString(2)), 
                        reader.GetString(3), 
                        reader.IsDBNull(4) ? "No Note" : reader.GetString(4)));
                }
            }
        }
        return transactions;
    }


    public List<Transaction> SortByDate()
    {
        List<Transaction> transactions = new List<Transaction>();

        string sortQuery = @"
			SELECT 'Income' as category, amount, source, date, tags, note
			FROM incomes 
			UNION ALL
			SELECT 'Expenses' as category, exp_amount, exp_source, exp_date, tags, note
			FROM expenses
			UNION ALL
			SELECT 'Debts' as category, debt_amount, debt_source, debt_date, tags, note
			FROM debt
			ORDER BY date;";

        using (var cmd = new SQLiteCommand(sortQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    transactions.Add(new Transaction(reader.GetString(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        DateOnly.Parse(reader.GetString(3)),
                        reader.GetString(4),
                        reader.IsDBNull(5) ? "No Note" : reader.GetString(5)));
                }
            }
        }
        return transactions ;
    }

    public int GetHighest(string tableName)
    {
        string query = $"SELECT MAX(amount) FROM {tableName}";

        if (tableName == "expenses") query = $"SELECT MAX(exp_amount) FROM {tableName}";
        if (tableName == "debt") query = $"SELECT MAX(debt_amount) FROM {tableName}";

        using (var cmd = new SQLiteCommand(query, conn))
        {
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
    }

    public int GetLowest(string tableName)
    {
        string query = $"SELECT MIN(amount) FROM {tableName}";

        if (tableName == "expenses") query = $"SELECT MIN(exp_amount) FROM {tableName}";
        if (tableName == "debt") query = $"SELECT MIN(debt_amount) FROM {tableName}";

        using (var cmd = new SQLiteCommand(query, conn))
        {
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
    }



}