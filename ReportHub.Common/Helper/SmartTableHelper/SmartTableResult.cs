namespace ReportHub.Common.Helper.SmartTableHelper;

public class SmartTableResult<T>
{
    public IEnumerable<T> Items { get; set; }

    public int TotalRecord { get; set; }

    public int NumberOfPages { get; set; }
}