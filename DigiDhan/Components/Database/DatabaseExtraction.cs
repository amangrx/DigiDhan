using System.Data.SQLite;
using System.Diagnostics;

public class DatabaseExtraction
{
    private SQLiteConnection conn;
    string database = "Data Source=E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db;Version=3;";

    // Constructor that initializes the SQLite connection and opens it.
    public DatabaseExtraction()
    {
        conn = new SQLiteConnection(database);
        conn.Open();
    }

    // Fetches the balance for user based on their user ID.
    public int GetUserBalance(int userId)
    {
        //query to get details from users. 
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

    // Retrieves the total number of transactions across incomes, expenses, and debts.
    public int GetNumberTransaction()
    {
        int totalNumberOfTransaction = 0;

        string getNumberIncome = "SELECT COUNT(*) FROM incomes";
        string getNumberExpense = "SELECT COUNT(*) FROM expenses";
        string getNumberDebt = "SELECT COUNT(*) FROM debt";

        // For incomes
        using (var cmd = new SQLiteCommand(getNumberIncome, conn))
        {
            totalNumberOfTransaction += Convert.ToInt32(cmd.ExecuteScalar());
        }

        // For expenses
        using (var cmd = new SQLiteCommand(getNumberExpense, conn))
        {
            totalNumberOfTransaction += Convert.ToInt32(cmd.ExecuteScalar());
        }

        // For debts
        using (var cmd = new SQLiteCommand(getNumberDebt, conn))
        {
            totalNumberOfTransaction += Convert.ToInt32(cmd.ExecuteScalar());
        }

        return totalNumberOfTransaction;
    }

    // Fetches a list of all transactions (income, expenses, and debts) with their details.
    public List<Transaction> GetTransactions()
    {
        List<Transaction> transactions = new List<Transaction>();

        //Retrieve income transactions
        string incomeQuery = "SELECT amount, source, date, tags, note, type FROM incomes";
        using (var cmd = new SQLiteCommand(incomeQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var type = Enum.Parse<IncomeType>(reader.GetString(5));
                    transactions.Add(new Transaction("Income",
                        reader.GetInt32(0),
                        reader.GetString(1),
                        DateOnly.Parse(reader.GetString(2)),
                        reader.GetString(3),
                        reader.IsDBNull(4) ? "No Note" : reader.GetString(4),
                        type));
                }
            }
        }

        // Retrieve expense transactions
        string expenseQuery = "SELECT exp_amount, exp_source, exp_date, tags, note, exp_type FROM expenses";
        using (var cmd = new SQLiteCommand(expenseQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var type = Enum.Parse<ExpenseType>(reader.GetString(5));
                    transactions.Add(new Transaction("Expenses",
                        reader.GetInt32(0),
                        reader.GetString(1),
                        DateOnly.Parse(reader.GetString(2)),
                        reader.GetString(3),
                        reader.IsDBNull(4) ? "No Note" : reader.GetString(4),
                        type));
                }
            }
        }

        // Retrieve debt transactions
        string debtQuery = "SELECT debt_amount, debt_source, debt_date, tags, note, debt_type FROM debt";
        using (var cmd = new SQLiteCommand(debtQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var type = Enum.Parse<DebtType>(reader.GetString(5));
                    transactions.Add(new Transaction("Debts",
                        reader.GetInt32(0), reader.GetString(1),
                        DateOnly.Parse(reader.GetString(2)),
                        reader.GetString(3),
                        reader.IsDBNull(4) ? "No Note" : reader.GetString(4),
                        type));
                }
            }
        }

        return transactions;
    }

    // Retrieves all transactions sorted by date across incomes, expenses, and debts.
    public List<Transaction> SortByDate()
    {
        // Initializes an empty list to store transaction objects. 
        List<Transaction> transactions = new List<Transaction>();

        string sortQuery = @"
            SELECT 'Income' as category, amount, source, date, tags, note, type 
            FROM incomes 
            UNION ALL
            SELECT 'Expenses' as category, exp_amount, exp_source, exp_date, tags, note, exp_type
            FROM expenses
            UNION ALL
            SELECT 'Debts' as category, debt_amount, debt_source, debt_date, tags, note, debt_type
            FROM debt
            ORDER BY date;";

        using (var cmd = new SQLiteCommand(sortQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string category = reader.GetString(0);
                    string typeTransaction = reader.GetString(6);

                    // Declare the correct type variable based on the category
                    if (category == "Income")
                    {
                        // Parse as IncomeType
                        var incomeType = Enum.Parse<IncomeType>(typeTransaction);
                        transactions.Add(new Transaction(
                            category,
                            reader.GetInt32(1),
                            reader.GetString(2),
                            DateOnly.Parse(reader.GetString(3)),
                            reader.GetString(4),
                            reader.IsDBNull(5) ? "No Note" : reader.GetString(5),
                            incomeType // Pass the correct IncomeType
                        ));
                    }
                    else if (category == "Expenses")
                    {
                        // Parse as ExpenseType
                        var expenseType = Enum.Parse<ExpenseType>(typeTransaction);
                        transactions.Add(new Transaction(
                            category,
                            reader.GetInt32(1),
                            reader.GetString(2),
                            DateOnly.Parse(reader.GetString(3)),
                            reader.GetString(4),
                            reader.IsDBNull(5) ? "No Note" : reader.GetString(5),
                            expenseType // Pass the correct ExpenseType
                        ));
                    }
                    else if (category == "Debts")
                    {
                        // Parse as DebtType
                        var debtType = Enum.Parse<DebtType>(typeTransaction);
                        transactions.Add(new Transaction(
                            category,
                            reader.GetInt32(1),
                            reader.GetString(2),
                            DateOnly.Parse(reader.GetString(3)),
                            reader.GetString(4),
                            reader.IsDBNull(5) ? "No Note" : reader.GetString(5),
                            debtType // Pass the correct DebtType
                        ));
                    }
                }
            }
        }
        return transactions;
    }

    // Searches for transactions within a specific date range and matching the given filters.
    public List<Transaction> SearchByFilterAndSpecificDateRange(String title, IncomeType incType, ExpenseType expType, DebtType debType, DateOnly startDate, DateOnly endDate)
    {
        List<Transaction> transactions = new List<Transaction>();
        string specificDateQuery = @"
            SELECT 'Income' as category, amount, source, date, tags, note, type 
            FROM incomes 
            WHERE source LIKE CONCAT('%', @title, '%')
            AND type LIKE CONCAT('%', @incType, '%')
            AND date BETWEEN @startDate AND @endDate
            UNION ALL
            SELECT 'Expenses' as category, exp_amount, exp_source, exp_date, tags, note, exp_type
            FROM expenses
            WHERE exp_source LIKE CONCAT('%', @title, '%')
            AND exp_type LIKE CONCAT('%', @expType, '%')
            AND exp_date BETWEEN @startDate AND @endDate
            UNION ALL
            SELECT 'Debts' as category, debt_amount, debt_source, debt_date, tags, note, debt_type
            FROM debt
            WHERE debt_source LIKE CONCAT('%', @title, '%')
            AND debt_type LIKE CONCAT('%', @debType, '%')
            AND debt_date BETWEEN @startDate AND @endDate
            ORDER BY date;";

        using (var cmd = new SQLiteCommand(specificDateQuery, conn))
        {
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@incType", incType.ToString());
            cmd.Parameters.AddWithValue("@expType", expType.ToString());
            cmd.Parameters.AddWithValue("@debType", debType.ToString());
            cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string category = reader.GetString(0);
                    string typeTransaction = reader.GetString(6);

                    // Declare the correct type variable based on the category
                    if (category == "Income")
                    {
                        // Parse as IncomeType
                        var incomeType = Enum.Parse<IncomeType>(typeTransaction);
                        transactions.Add(new Transaction(
                            category,
                            reader.GetInt32(1),
                            reader.GetString(2),
                            DateOnly.Parse(reader.GetString(3)),
                            reader.GetString(4),
                            reader.IsDBNull(5) ? "No Note" : reader.GetString(5),
                            incomeType // Pass the correct IncomeType
                        ));
                    }
                    else if (category == "Expenses")
                    {
                        // Parse as ExpenseType
                        var expenseType = Enum.Parse<ExpenseType>(typeTransaction);
                        transactions.Add(new Transaction(
                            category,
                            reader.GetInt32(1),
                            reader.GetString(2),
                            DateOnly.Parse(reader.GetString(3)),
                            reader.GetString(4),
                            reader.IsDBNull(5) ? "No Note" : reader.GetString(5),
                            expenseType // Pass the correct ExpenseType
                        ));
                    }
                    else if (category == "Debts")
                    {
                        // Parse as DebtType
                        var debtType = Enum.Parse<DebtType>(typeTransaction);
                        transactions.Add(new Transaction(
                            category,
                            reader.GetInt32(1),
                            reader.GetString(2),
                            DateOnly.Parse(reader.GetString(3)),
                            reader.GetString(4),
                            reader.IsDBNull(5) ? "No Note" : reader.GetString(5),
                            debtType // Pass the correct DebtType
                        ));
                    }
                }
            }
        }
        return transactions;
    }

    // Gets the highest amount 
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

    // Gets the lowest amount 
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

    // Calculate the total amount 
    public int GetTotal(string tableName)
    {
        string query = $"SELECT SUM(amount) FROM {tableName}";

        if (tableName == "expenses") query = $"SELECT SUM(exp_amount) FROM {tableName}";
        if (tableName == "debt") query = $"SELECT SUM(debt_amount) FROM {tableName}";

        using (var cmd = new SQLiteCommand(query, conn))
        {
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
    }

    // Calculates the total amount of debts for a specific debt type 
    public int GetPendingAndClearedTotal(string debtType)
    {
        string query = $"SELECT SUM(debt_amount) FROM debt WHERE debt_type= '{debtType}';";
        if (debtType == "Cleared") query = $"SELECT SUM(debt_amount) FROM debt WHERE debt_type= '{debtType}';";

        using (var cmd = new SQLiteCommand(query, conn))
        {
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
    }
}