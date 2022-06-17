namespace XTrakr.Common;
public class SearchContext
{
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ExpenseTypeId { get; set; }
    public int EntityId { get; set; }
    public decimal LowAmount { get; set; }
    public decimal HighAmount { get; set; }
    public string ReferenceContains { get; set; }
    public string DescriptionContains { get; set; }

    public SearchContext()
    {
        Year = 0;
        StartDate = default;
        EndDate = DateTime.MaxValue;
        ExpenseTypeId = 0;
        EntityId = 0;
        LowAmount = 0M;
        HighAmount = 0M;
        ReferenceContains = string.Empty;
        DescriptionContains = string.Empty;
    }

    public static SearchContext Default => new();
}
