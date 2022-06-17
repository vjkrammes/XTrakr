namespace XTrakr.Models;
public class ThemeModel
{
    public string Alt0 { get; set; }
    public string Alt1 { get; set; }
    public string Background { get; set; }
    public string Border { get; set; }
    public string Foreground { get; set; }

    public ThemeModel()
    {
        Alt0 = "White";
        Alt1 = "White";
        Background = "Black";
        Border = "Black";
        Foreground = "White";
    }
}
