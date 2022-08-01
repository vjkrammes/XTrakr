using System;

using XTrakr.Infrastructure;
using XTrakr.Models;

namespace XTrakr.ViewModels;

public class IncomeItemViewModel : ViewModelBase
{
    #region Properties

    private DateTime? _date;
    public DateTime? Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    private decimal _owed;
    public decimal Owed
    {
        get => _owed;
        set => SetProperty(ref _owed, value);
    }

    private decimal _paid;
    public decimal Paid
    {
        get => _paid;
        set => SetProperty(ref _paid, value);
    }

    private string? _reference;
    public string? Reference
    {
        get => _reference;
        set => SetProperty(ref _reference, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private IncomeModel? _model;
    public IncomeModel? Model
    {
        get => _model;
        set
        {
            SetProperty(ref _model, value);
            if (Model is null)
            {
                Date = DateTime.UtcNow;
                Owed = 0M;
                Paid = 0M;
                Reference = string.Empty;
                Description = string.Empty;
            }
            else
            {
                Date = Model.IncomeDate;
                Owed = Model.AmountOwed;
                Paid = Model.AmountPaid;
                Reference = Model.Reference;
                Description = Model.Description;
            }
        }
    }

    #endregion

    #region Command Methods

    public override bool OkCanExecute()
    {
        if (Date == default)
        {
            return false;
        }
        if (Owed < 0M || Paid < 0M)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(Reference) && string.IsNullOrWhiteSpace(Description))
        {
            return false;
        }
        return true;
    }

    #endregion

    public override void Reset()
    {
        base.Reset();
    }

    public IncomeItemViewModel()
    {
        Date = DateTime.UtcNow;
        Owed = 0M;
        Paid = 0M;
        Reference = string.Empty;
        Description = string.Empty;
        Model = new();
    }
}
