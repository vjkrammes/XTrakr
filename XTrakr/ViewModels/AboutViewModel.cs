using System.Linq;
using System.Reflection;

using XTrakr.Common;
using XTrakr.Controls;
using XTrakr.Infrastructure;

namespace XTrakr.ViewModels;

public class AboutViewModel : ViewModelBase
{
    private ObservableDictionary<string, string>? _credits;
    public ObservableDictionary<string, string>? Credits
    {
        get => _credits;
        set => SetProperty(ref _credits, value);
    }

    private string GetCopyrightFromAssembly()
    {
        var assembly = GetType().Assembly;
        var attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
        if (attributes is not null && attributes.Any())
        {
            return ((AssemblyCopyrightAttribute)attributes.First()).Copyright;
        }
        return "Copyright information unavailable";
    }

    private string GetCompanyFromAssembly()
    {
        var assembly = GetType().Assembly;
        var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
        if (attributes is not null && attributes.Any())
        {
            return ((AssemblyCompanyAttribute)attributes.First()).Company;
        }
        return "Company information unavailable";
    }

    public AboutViewModel()
    {
        Credits = new()
        {
            ["Product"] = Constants.ProgramName,
            ["Version"] = Constants.ProgramVersion.ToString("n2"),
            ["Author"] = "V. James Krammes",
            ["Company"] = GetCompanyFromAssembly(),
            ["Copyright"] = GetCopyrightFromAssembly(),
            ["Platform"] = "Windows Desktop",
            ["Architecture"] = "Model - View - ViewModel (MVVM)",
            [".Net Version"] = ".Net 6",
            ["Presentation"] = "Windows Presentation Foundation (WPF)",
            ["Database"] = "Microsoft SQL (T-SQL)",
            ["Database Access"] = "Dapper (https://github.com/DapperLib/Dapper)",
            ["Charts"] = "Syncfusion (https://syncfusion.com)",
            ["CSV Support"] = "CSVHelper (https://joshclose.github.io/CsvHelper/)",
            ["Repository"] = "https://github.com/vjkrammes/XTrakr"
        };
    }
}
