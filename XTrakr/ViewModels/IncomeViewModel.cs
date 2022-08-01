using System;
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

public class IncomeViewModel : ViewModelBase
{
    #region Properties

    private readonly IIncomeService _incomeService;
    private readonly IncomeItemViewModel _incomeItemViewModel;

    private ObservableCollection<IncomeModel>? _income;
    public ObservableCollection<IncomeModel>? Income
    {
        get => _income;
        set => SetProperty(ref _income, value);
    }

    private IncomeModel? _selectedIncome;
    public IncomeModel? SelectedIncome
    {
        get => _selectedIncome;
        set => SetProperty(ref _selectedIncome, value);
    }

    #endregion

    #region Commands

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

    private AsyncCommand? _newCommand;
    public ICommand NewCommand
    {
        get
        {
            if (_newCommand is null)
            {
                _newCommand = new(NewClick, AlwaysCanExecute);
            }
            return _newCommand;
        }
    }

    private AsyncCommand? _editCommand;
    public ICommand EditCommand
    {
        get
        {
            if (_editCommand is null)
            {
                _editCommand = new AsyncCommand(EditClick, IncomeSelected);
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
                _deleteCommand = new(DeleteClick, IncomeSelected);
            }
            return _deleteCommand;
        }
    }

    #endregion

    #region

    private async Task NewClick()
    {
        _incomeItemViewModel.Model = null;
        if (DialogSupport.ShowDialog<IncomeItemWindow>(_incomeItemViewModel, Application.Current.MainWindow) != true)
        {
            return;
        }
        var model = new IncomeModel
        {
            Id = IdEncoder.EncodeId(0),
            ContractId = IdEncoder.EncodeId(0),
            IncomeDate = _incomeItemViewModel.Date ?? DateTime.UtcNow,
            AmountOwed = _incomeItemViewModel.Owed,
            AmountPaid = _incomeItemViewModel.Paid,
            Reference = _incomeItemViewModel.Reference ?? string.Empty,
            Description = _incomeItemViewModel.Description ?? string.Empty,
            CanDelete = true
        };
        var result = await _incomeService.InsertAsync(model);
        if (!result.Successful)
        {
            PopupManager.Popup(result.Message!, "Add Item Failed", PopupButtons.Ok, PopupImage.Error);
            return;
        }
        var ix = 0;
        while (ix < Income!.Count && Income[ix].IncomeDate > model.IncomeDate)
        {
            ix++;
        }
        Income.Insert(ix, model);
        SelectedIncome = model;
        SelectedIncome = null;
    }

    private bool IncomeSelected() => SelectedIncome is not null;

    private async Task EditClick()
    {
        if (SelectedIncome is null)
        {
            return;
        }
        _incomeItemViewModel.Model = SelectedIncome;
        if (DialogSupport.ShowDialog<IncomeItemWindow>(_incomeItemViewModel, Application.Current.MainWindow) != true)
        {
            SelectedIncome = null;
            return;
        }
        var item = SelectedIncome.Clone();
        if (_incomeItemViewModel.Date.HasValue)
        {
            item.IncomeDate = _incomeItemViewModel.Date.Value;
        }
        item.AmountOwed = _incomeItemViewModel.Owed;
        item.AmountPaid = _incomeItemViewModel.Paid;
        item.Reference = _incomeItemViewModel.Reference ?? string.Empty;
        item.Description = _incomeItemViewModel.Description ?? string.Empty;
        item.CanDelete = true;
        var result = await _incomeService.UpdateAsync(item);
        if (!result.Successful)
        {
            PopupManager.Popup(result.Message!, "Add Item Failed", PopupButtons.Ok, PopupImage.Error);
            return;
        }
        Income!.Remove(SelectedIncome);
        SelectedIncome = null;
        var ix = 0;
        while (ix < Income.Count && Income[ix].IncomeDate > item.IncomeDate)
        {
            ix++;
        }
        Income.Insert(ix, item);
        SelectedIncome = item;
        SelectedIncome = null;
    }

    private async Task DeleteClick()
    {
        if (SelectedIncome is null)
        {
            return;
        }
        var result = await _incomeService.DeleteAsync(SelectedIncome);
        if (result.Successful)
        {
            Income!.Remove(SelectedIncome);
            SelectedIncome = null;
            return;
        }
        PopupManager.Popup(result.Message!, "Delete Failed", PopupButtons.Ok, PopupImage.Error);
        return;
    }

    private async Task WindowLoaded()
    {
        Income = new((await _incomeService.GetAsync()).OrderByDescending(x => x.IncomeDate));
    }

    #endregion

    public override void Reset()
    {
        base.Reset();
    }

    public IncomeViewModel(IIncomeService incomeService, IncomeItemViewModel incomeItemViewModel)
    {
        _incomeService = incomeService;
        _incomeItemViewModel = incomeItemViewModel;
    }
}
