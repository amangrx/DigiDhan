public class Income
{
    public int _amount { get; set; }
    public String _source { get; set; }
    public DateOnly _dateOfTransaction { get; set; }
    public String _tags { get; set; }
    public String _note { get; set; }
    public IncomeType _type { get; set; }

    public Income() { }

    public Income(int amount, string source, DateOnly dateOfTransaction, string tags, string note, IncomeType type)
    {
        _amount = amount;
        _source = source;
        _dateOfTransaction = dateOfTransaction;
        _tags = tags;
        _note = note;
        _type = type;
    }

}
