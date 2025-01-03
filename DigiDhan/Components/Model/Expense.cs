public class Expense
{
    public int amount { get; set; }
    public String source { get; set; }
    public DateOnly dateOfTransaction { get; set; }
    public String tags { get; set; }
    public String note { get; set; }
    public ExpenseType type { get; set; }

    public Expense() { }

    public Expense(int amount, string source, DateOnly dateOfTransaction, string tags, string note, ExpenseType type)
    {
        this.amount = amount; 
        this.source = source;
        this.dateOfTransaction = dateOfTransaction;
        this.tags = tags;
        this.note = note;
        this.type = type;
    }

}
