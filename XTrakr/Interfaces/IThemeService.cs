using Microsoft.Extensions.Configuration;

namespace XTrakr.Interfaces;
public interface IThemeService
{
    void LoadDefaultTheme();
    void LoadTheme(IConfiguration configuration);
    void SetColor(string key, string colorName);
    string GetColor(string key);
}
