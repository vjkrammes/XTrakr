using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using XTrakr.Common;
using XTrakr.Enumerations;
using XTrakr.Infrastructure;
using XTrakr.Models;
using XTrakr.Services.Interfaces;
using XTrakr.Views;

namespace XTrakr.ViewModels;

public class ManagePayeesViewModel : ViewModelBase
{
    #region Properties

    private readonly IPayeeService _payeeService;
    private readonly PayeeViewModel _payeeViewModel;

    private ObservableCollection<PayeeModel>? _payees;
    public ObservableCollection<PayeeModel>? Payees
    {
        get => _payees;
        set => SetProperty(ref _payees, value);
    }

    private PayeeModel? _selectedPayee;
    public PayeeModel? SelectedPayee
    {
        get => _selectedPayee;
        set => SetProperty(ref _selectedPayee, value);
    }

    #endregion

    #region Commands

    private AsyncCommand? _addCommand;
    public ICommand AddCommand
    {
        get
        {
            if (_addCommand is null)
            {
                _addCommand = new(AddClick, AlwaysCanExecute);
            }
            return _addCommand;
        }
    }

    private AsyncCommand? _editCommand;
    public ICommand EditCommand
    {
        get
        {
            if (_editCommand is null)
            {
                _editCommand = new(EditClick, PayeeSelected);
            }
            return _editCommand;
        }
    }

    private AsyncCommand? _deleteCommand;
    public ICommand DeleteCommand
    {
        get
        {
            if (_deleteCommand is null)
            {
                _deleteCommand = new(DeleteClick, DeleteCanClick);
            }
            return _deleteCommand;
        }
    }

    private AsyncCommand? _windowLoadedCommand;

    public ICommand WindowLoadedCommand
    {
        get
        {
            if (_windowLoadedCommand is null)
            {
                _windowLoadedCommand = new(WindowLoaded, AlwaysCanExecute);
            }
            return _windowLoadedCommand;
        }
    }

    #endregion

    #region Command Methods

    private async Task AddClick()
    {
        if (DialogSupport.ShowDialog<PayeeWindow>(_payeeViewModel, Application.Current.MainWindow) == false)
        {
            return;
        }
        var payee = new PayeeModel
        {
            Id = IdEncoder.EncodeId(0),
            Name = _payeeViewModel.Name.Capitalize(),
            Address = _payeeViewModel.Address,
            Description = _payeeViewModel.Description,
            CanDelete = true
        };
        var response = await _payeeService.InsertAsync(payee);
        if (response.Successful)
        {
            var ix = 0;
            while (ix < Payees!.Count && payee > Payees[ix])
            {
                ix++;
            }
            Payees.Insert(ix, payee);
            return;
        }
        PopupManager.Popup(response.Message!, "Database Error Adding Payee", PopupButtons.Ok, PopupImage.Error);
    }

    private bool PayeeSelected() => SelectedPayee is not null;

    private async Task EditClick()
    {
        if (SelectedPayee is not null)
        {
            _payeeViewModel!.Payee = SelectedPayee.Clone();
            if (DialogSupport.ShowDialog<PayeeWindow>(_payeeViewModel, Application.Current.MainWindow) == false)
            {
                return;
            }
            var payee = new PayeeModel
            {
                Id = SelectedPayee.Id,
                Name = _payeeViewModel.Name.Capitalize(),
                Address = _payeeViewModel.Address,
                Description = _payeeViewModel.Description,
                CanDelete = true
            };
            var response = await _payeeService.UpdateAsync(payee);
            if (response.Successful)
            {
                Payees!.Remove(SelectedPayee);
                var ix = 0;
                while (ix < Payees!.Count && payee > Payees[ix])
                {
                    ix++;
                }
                Payees.Insert(ix, payee);
                return;
            }
            PopupManager.Popup(response.Message!, "Database Error Updating Payee", PopupButtons.Ok, PopupImage.Error);
        }
    }

    private bool DeleteCanClick() => SelectedPayee is not null && SelectedPayee.CanDelete;

    private async Task DeleteClick()
    {
        if (SelectedPayee is not null)
        {
            var response = await _payeeService.DeleteAsync(SelectedPayee);
            if (response.Successful)
            {
                Payees!.Remove(SelectedPayee);
                return;
            }
            PopupManager.Popup(response.Message!, "Database Error Deleting Payee", PopupButtons.Ok, PopupImage.Error);
        }
    }

    private async Task WindowLoaded()
    {
        Payees = new((await _payeeService.GetAsync()).OrderBy(x => x.Name));
    }

    #endregion

    public ManagePayeesViewModel(IPayeeService payeeService, PayeeViewModel payeeViewModel)
    {
        _payeeService = payeeService;
        _payeeViewModel = payeeViewModel;
    }
}
