namespace ReportHub.Objects.DTOs;

public class LookupsDTO
{
    public int HeaderLookupId { get; set; }
    public string Name { get; set; }
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    public short Value { get; set; }
    public int? Reference_LookupID { get; set; }
    public string Reference_LookupName { get; set; }
    public int LookupId { get; set; }
    public string LookUpHeaderName { get; set; }
}
public class ChildLookups
{
    public int HeaderLookupId { get; set; }
    public string Name { get; set; }
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    public string LookUpHeaderName { get; set; }
    public int LookupId { get; set; }


}