public class User
{
    public string username { get; set; }
    public string password { get; set; }
    public int balance { get; set; }

    public User(string username, string password, int balance)
    {
        this.username = username;
        this.password = password;
        this.balance = balance;
    }
}