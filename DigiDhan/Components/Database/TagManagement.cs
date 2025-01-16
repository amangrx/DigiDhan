using System.Data.SQLite;

public class TagManagement
{
    private SQLiteConnection conn;
    string database = "Data Source=E:\\ICP\\Year 3\\Autumn\\Data and web development\\DigiDhan\\DigiDhan\\digidhan.db;Version=3;";

    public TagManagement()
    {
        conn = new SQLiteConnection(database);
        conn.Open();
    }

    public List<string> GetTagList()
    {
        List<string> tagList = new List<string>();
        string tagQuery = "SELECT tag_name FROM tags;"; 

        using (var cmd = new SQLiteCommand(tagQuery, conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tagList.Add(reader.GetString(reader.GetOrdinal("tag_name")));
                }
            }

        }
        return tagList;
    }


}