using XTrakr.Infrastructure;

namespace XTrakr.Models;
public class ChartItem : NotifyBase
{
    private string? _name;
    public string? Name
    {
        get => _name;
        private set => SetProperty(ref _name, value);
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    public ChartItem(string name, decimal amount)
    {
        Name = name;
        Amount = amount;
    }
}
