namespace ReportHub.Objects.DTOs;

public class CountryDTO
{
    public int Id { get; set; }
    public string PrimaryName { get; set; }
    public string SeconderyName { get; set; }
    public string Name { get; set; }
    public string ISOCode { get; set; }
    public string CountryCode { get; set; }
    public string Alpha2 { get; set; }
    public List<AreaDTO> Area { get; set; }

}

public class AreaDTO
{
    public int Id { get; set; }
    public int CountryId { set; get; }
    public string Name { set; get; }
    public string PrimaryName { set; get; }
    public string SeconderyName { set; get; }
}
