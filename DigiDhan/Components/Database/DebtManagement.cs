using Microsoft.Maui.Controls;
using System.Data.SqlClient;
using System.Data.SQLite;

public class DebtManagement
{
    private SQLiteConnection _connection;
    string database = "Data Source=E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db;Version=3;";

    public DebtManagement()
    {
        _connection = new SQLiteConnection(database);
        _connection.Open();
    }

    //For checking the number of pending debt at the time of income insertion
    public int CheckNumberOfPendingDebt()
    {
        int numberOfPendingDebt = 0;
        string checkDebtQuery = "SELECT COUNT(*) FROM debt WHERE debt_type = 'Pending';";
        using (var cmd = new SQLiteCommand(checkDebtQuery, _connection))
        {
            numberOfPendingDebt += Convert.ToInt32(cmd.ExecuteScalar());
        }
        return numberOfPendingDebt;
    }

    //function to clear the pending debt 
    public int ClearPendingDebt(int amount)
    {
        //query for getting outstanding amount
        string getOutstanding = "SELECT debt_id, outstanding_amt FROM debt WHERE debt_type ='Pending';";
        //query for full clearance of outstanding amount 
        string fullyPaid = "UPDATE debt SET outstanding_amt = 0, debt_type = 'Cleared' WHERE debt_id = @id";
        //query for partial payment of outstanding amount 
        string partialPayment = "UPDATE debt SET outstanding_amt = outstanding_amt - @amount WHERE debt_id = @id";

        using (var cmd = new SQLiteCommand(getOutstanding, _connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read() && amount > 0)
                {
                    int debtId = reader.GetInt32(reader.GetOrdinal("debt_id"));
                    int currentOutstandingDebt = reader.GetInt32(reader.GetOrdinal("outstanding_amt"));
                    //For fully paid cases 
                    if (amount >= currentOutstandingDebt)
                    {
                        using (var updateCMD = new SQLiteCommand(fullyPaid, _connection))
                        {
                            updateCMD.Parameters.AddWithValue("@id", debtId);
                            int row = updateCMD.ExecuteNonQuery();
                        }
                        amount -= currentOutstandingDebt;
                    }
                    else
                    {
                        //For partial payment of pending debt
                        using (var updateCmd = new SQLiteCommand(partialPayment, _connection))
                        {
                            updateCmd.Parameters.AddWithValue("@id", debtId);
                            updateCmd.Parameters.AddWithValue("@amount", amount);
                            updateCmd.ExecuteNonQuery();
                        }
                        amount = 0;
                    }
                }

            }

        }
        return amount;

    }

}