
using XTrakr.Common;
using XTrakr.Infrastructure;
using XTrakr.Services.Interfaces;

namespace XTrakr.ViewModels;
public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(IExpenseService expenseService, IPayeeService payeeService, IExpenseTypeService expenseTypeService, AboutViewModel aboutViewModel,
        BackupViewModel backupViewModel, DashboardViewModel dashboardViewModel, ExpenseViewModel expenseViewModel, ExpenseTypeViewModel expenseTypeViewModel, 
        ManagePayeesViewModel managePayeesViewModel)
    {
        WindowTitle = $"{Constants.ProgramName} Version {Constants.ProgramVersion:n2}";
        Banner = "XTrakr - Track Expenses for your LLC";
        _expenseService = expenseService;
        _payeeService = payeeService;
        _expenseTypeService = expenseTypeService;
        _aboutViewModel = aboutViewModel;
        _backupViewModel = backupViewModel;
        _dashboardViewModel = dashboardViewModel;
        _expenseViewModel = expenseViewModel;
        _expenseTypeViewModel = expenseTypeViewModel;
        _managePayeesViewModel = managePayeesViewModel;
    }
}
