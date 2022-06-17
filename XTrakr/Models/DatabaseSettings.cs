namespace XTrakr.Models;
public class DatabaseSettings
{
    public string Server { get; set; }
    public string Database { get; set; }
    public string Auth { get; set; }

    public DatabaseSettings()
    {
        Server = "localhost";
        Database = "XTrakr";
        Auth = "Trusted_Connection=true";
    }
}
