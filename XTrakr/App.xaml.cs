using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Windows;

using XTrakr.Infrastructure;
using XTrakr.Interfaces;
using XTrakr.Models;
using XTrakr.Repositories;
using XTrakr.Repositories.Interfaces;
using XTrakr.Services;
using XTrakr.Services.Interfaces;
using XTrakr.ViewModels;

namespace XTrakr;

public partial class App : Application
{
    public IServiceProvider? ServiceProvider { get; }
    public IConfiguration Configuration { get; }

    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjU3ODk2QDMyMzAyZTMxMmUzMEJOa1RFY3Vxc2lGTFRFVktOWXkzcDBGSFhmbWp6d1ZIUjgyM05rRDFrYWs9");
        var services = new ServiceCollection();
        Configuration = new ConfigurationFactory().Create("appsettings.json", isOptional: false);
        ConfigureServices(services, Configuration);
        services.AddSingleton(x => x);
        ServiceProvider = services.BuildServiceProvider();
        UpdateDatabase();
    }

    private void UpdateDatabase()
    {
        var builder = ServiceProvider!.GetRequiredService<IDatabaseBuilder>();
        builder.BuildDatabase(false);
    }

    private void ApplicationStartup(object sender, StartupEventArgs e)
    {
        var mainViewModel = ServiceProvider?.GetRequiredService<MainViewModel>();
        var mainWindow = new MainWindow
        {
            DataContext = mainViewModel
        };
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Miscellaneous services

        services.AddTransient<IConfigurationFactory, ConfigurationFactory>();

        // Database and DatabaseBuilder

        var dbsettings = configuration.GetSection("Database").Get<DatabaseSettings>();
        if (dbsettings is null)
        {
            dbsettings = new();
        }
        services.AddTransient<IDatabase>(x => new Database(dbsettings.Server, dbsettings.Database, dbsettings.Auth));
        services.AddTransient<IDatabaseBuilder, DatabaseBuilder>();

        // Repositories

        services.AddTransient<IExpenseRepository, ExpenseRepository>();
        services.AddTransient<IExpenseTypeRepository, ExpenseTypeRepository>();
        services.AddTransient<IPayeeRepository, PayeeRepository>();

        // Data services

        services.AddTransient<IExpenseService, ExpenseService>();
        services.AddTransient<IExpenseTypeService, ExpenseTypeService>();
        services.AddTransient<IPayeeService, PayeeService>();

        // View Models

        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ExpenseViewModel>();
        services.AddTransient<ExpenseTypeViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ManagePayeesViewModel>();
        services.AddTransient<PayeeViewModel>();
        services.AddTransient<PopupViewModel>();
    }
}
