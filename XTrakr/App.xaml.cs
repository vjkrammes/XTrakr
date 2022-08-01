using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        var services = new ServiceCollection();
        Configuration = new ConfigurationFactory().Create("appsettings.json", isOptional: false);
        var sflicense = Configuration["Licenses:Syncfusion"];
        if (!string.IsNullOrWhiteSpace(sflicense))
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(sflicense);
        }
        ConfigureServices(services, Configuration);
        services.AddSingleton(x => x);
        ServiceProvider = services.BuildServiceProvider();
        UpdateDatabase();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // make text boxes auto-select text when they get focus
        EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(MouseHandler<TextBox>));
        EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
            new RoutedEventHandler(TextBoxSelectText));
        EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent,
            new RoutedEventHandler(TextBoxSelectText));
        EventManager.RegisterClassHandler(typeof(TextBox), Control.MouseDoubleClickEvent,
            new RoutedEventHandler(TextBoxSelectText));
        EventManager.RegisterClassHandler(typeof(PasswordBox), UIElement.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(MouseHandler<PasswordBox>));
    }

    private void MouseHandler<T>(object sender, MouseButtonEventArgs e) where T : UIElement
    {
        DependencyObject parent = (e.OriginalSource as UIElement)!;
        while (parent is not null and not T)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        if (parent is not null)
        {
            if (parent is T control)
            {
                if (!control.IsKeyboardFocusWithin)
                {
                    control.Focus();
                    e.Handled = true;
                }
            }
        }
    }

    private void TextBoxSelectText(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is TextBox tb)
        {
            tb.SelectAll();
        }
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
        services.AddTransient<IExplorerService, ExplorerService>();

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
        services.AddTransient<IIncomeRepository, IncomeRepository>();
        services.AddTransient<IPayeeRepository, PayeeRepository>();

        // Data services

        services.AddTransient<IExpenseService, ExpenseService>();
        services.AddTransient<IExpenseTypeService, ExpenseTypeService>();
        services.AddTransient<IIncomeService, IncomeService>();
        services.AddTransient<IPayeeService, PayeeService>();

        // View Models

        services.AddTransient<AboutViewModel>();
        services.AddTransient<BackupViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ExpenseViewModel>();
        services.AddTransient<ExpenseTypeViewModel>();
        services.AddTransient<ExplorerViewModel>();
        services.AddTransient<IncomeItemViewModel>();
        services.AddTransient<IncomeViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ManagePayeesViewModel>();
        services.AddTransient<PayeeViewModel>();
        services.AddTransient<PopupViewModel>();
    }
}
