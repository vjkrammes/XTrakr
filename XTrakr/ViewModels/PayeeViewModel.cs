
using XTrakr.Infrastructure;
using XTrakr.Models;

namespace XTrakr.ViewModels;

public class PayeeViewModel : ViewModelBase
{
    #region Properties

    private PayeeModel? _payee;
    public PayeeModel? Payee
    {
        get => _payee;
        set
        {
            SetProperty(ref _payee, value);
            if (Payee is not null)
            {
                Name = Payee.Name;
                Address = Payee.Address;
                Description = Payee.Description;
            }
            else
            {
                Name = string.Empty;
                Address = string.Empty;
                Description = string.Empty;
            }
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _address;
    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    #endregion

    #region Command Methods

    public override bool OkCanExecute() => !string.IsNullOrWhiteSpace(Name);

    #endregion

    public PayeeViewModel()
    {
        Payee = new();
        _name = string.Empty;
        _address = string.Empty;
        _description = string.Empty;
    }
}
