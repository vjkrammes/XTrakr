using CsvHelper.Configuration;

using XTrakr.Models;

namespace XTrakr.Infrastructure;

public class ExpenseMap : ClassMap<ExpenseModel>
{
    public ExpenseMap()
    {
        Map(x => x.Id).Index(0).Name("Id");
        Map(x => x.PayeeId).Index(1).Name("PayeeId");
        Map(x => x.ExpenseTypeId).Index(2).Name("ExpenseTypeId");
        Map(x => x.ExpenseDate).Index(3).Name("ExpenseDate");
        Map(x => x.Amount).Index(4).Name("Amount");
        Map(x => x.Reference).Index(5).Name("Reference");
        Map(x => x.Description).Index(6).Name("Description");
    }
}
