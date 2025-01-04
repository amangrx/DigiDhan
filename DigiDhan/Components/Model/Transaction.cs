﻿
public class Transaction
{
    public String category { get; set; }
    public int amount { get; set; }
    public String source { get; set; }
    public DateOnly dateOfTransaction { get; set; }
    public String tags { get; set; }
    public String? note { get; set; }

    public Transaction(string category, int amount, string source, DateOnly dateOfTransaction, string tags, string? note)
    {
        this.category = category;
        this.amount = amount;
        this.source = source;
        this.dateOfTransaction = dateOfTransaction;
        this.tags = tags;
        this.note = note;
    }

}