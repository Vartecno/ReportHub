namespace ReportHub.Objects.DTOs;

class InvoiceDTO
{
}


public class BankWithDetailsDTO
{
    public int BankId { get; set; }
    public object BankNumber { get; set; }
    public string BankPrimaryName { get; set; }
    public string BankSecondaryName { get; set; }
    public bool IsActive { get; set; }
    public string BranchPrimaryName { get; set; }
    public string BranchSecondaryName { get; set; }
    public string Address { get; set; }
    public string AccountBankNo { get; set; }
    public string SWIFT { get; set; }
    public string IBAN { get; set; }
    public int CurrencyId { get; set; }
}

public class APIReponse<T>
{
    public int ErrorCode { get; set; }
    public object ErrorMessage { get; set; }
    public bool IsScusses { get; set; }
    public List<T> ResponseDetails { get; set; }
}

public class APIReponseSingle<T>
{
    public int ErrorCode { get; set; }
    public object ErrorMessage { get; set; }
    public bool IsScusses { get; set; }
    public T ResponseDetails { get; set; }
}



public class InvoiceCompanyInfoDTO
{
    public int Id { get; set; }
    public int CompanyNumber { get; set; }
    public string CompanySecondaryName { get; set; }
    public string CompanyPrimaryName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Fax { get; set; }
    public string FooterLine1 { get; set; }
    public string FooterLine2 { get; set; }
    public string Image { get; set; }
    public string Host { get; set; }
    public string Email { get; set; }
    public string SMTPEmail { get; set; }
    public string TaxNumber { get; set; }
    public string RegistrationNo { get; set; }
    public string Password { get; set; }
    public string Port { get; set; }
    public bool Ssl { get; set; }
    public bool IsNumberingAutomatic { get; set; }
    public string CurrencyPrimaryName { get; set; }
    public string CurrencySecondlyName { get; set; }
    public string CurrencyCode { get; set; }
    public int CountryId { get; set; }
    public int CurrencyIDH { get; set; }
    public int CompanyType { get; set; }
}


public class CompanyInfo
{
    public int Id { get; set; }
    public int CompanyNumber { get; set; }
    public string CompanySecondaryName { get; set; }
    public string CompanyPrimaryName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Fax { get; set; }
    public string FooterLine1 { get; set; }
    public string FooterLine2 { get; set; }
    public string Image { get; set; }
    public object ColCompanyBranches { get; set; }
    public string Host { get; set; }
    public string Email { get; set; }
    public string SMTPEmail { get; set; }
    public string TaxNumber { get; set; }
    public string RegistrationNo { get; set; }
    public string Password { get; set; }
    public string Port { get; set; }
    public bool Ssl { get; set; }
    public int DefaultForOnline { get; set; }
    public bool IsNumberingAutomatic { get; set; }
    public string CurrencyPrimaryName { get; set; }
    public string CurrencySecondlyName { get; set; }
    public string CurrencyCode { get; set; }
    public bool isSelected { get; set; }
    public int CountryId { get; set; }
    public int CurrencyIDH { get; set; }
    public int Lkp_CurrencyId { get; set; }
    public int CompanyType { get; set; }
    public object ActivationDate { get; set; }
    public object CreatedBy { get; set; }
    public object CreatedAt { get; set; }
    public object ModifyBy { get; set; }
    public object ModifyAt { get; set; }
    public List<object> status { get; set; }
    public List<object> Workflows { get; set; }
    public object UploadImage { get; set; }
    public object ISOCode { get; set; }
    public object CountryCode { get; set; }
    public object SignatureImage { get; set; }
    public object PrimaryImage { get; set; }
    public object SecondaryImage { get; set; }
}



public class CompanyBranch
{
    public int BranchID { get; set; }
    public int BranchNumber { get; set; }
    public string BranchPrimaryName { get; set; }
    public string BranchSecondaryName { get; set; }
    public string SmallCurPrimaryName { get; set; }
    public string SmallCurSecondaryName { get; set; }
    public int CompanyId { get; set; }
    public int CreatedBy { get; set; }
    public int CityId { get; set; }
    public int AreaId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ModifyBy { get; set; }
    public DateTime ModifyAt { get; set; }
    public bool IsActive { get; set; }
    public string Host { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Port { get; set; }
    public bool Ssl { get; set; }
    public CompanyInfo oCompanyInfo { get; set; }
    //public List<CompanyInfo> oLCompany { get; set; }
    public bool IsNumberingAutomatic { get; set; }
    public bool IsAirportBranch { get; set; }
    public string CurrencyPrimaryName { get; set; }
    public string CurrencySecondlyName { get; set; }
    public string CurrencyCode { get; set; }
    public int CurrencyIDH { get; set; }
    public int CountryId { get; set; }
    public int BranchType { get; set; }
    public string ISOCode { get; set; }
    public string CountryCode { get; set; }
    public string TaxNumber { get; set; }
    public string RegistrationNo { get; set; }
    public string SMTPEmail { get; set; }
    public int CompanyCountryId { get; set; }
    public string Name { get; set; }
    public List<Groups> Groups { get; set; }
    public Byte[] Img { get; set; }
    public int? TajeerBranchId { get; set; }
    public bool IsWorkshop { get; set; }
    public string OTP { get; set; }
    public string PostalCode { get; set; }
    public string AdditionalStreetAddress { get; set; }
    public string BuildingNumber { get; set; }
    public string IdentityType { get; set; }
    public string DistrictName { get; set; }
    public string StreetName { get; set; }
    public string Alpha2 { get; set; }
}

public class Groups
{
    public int GroupID { get; set; }
    public int BranchId { get; set; }
    public int CompanyId { get; set; }
    public string PrimaryGroupName { get; set; }
    public string SecondaryGroupName { get; set; }
    public string Name { get; set; }

}

public class BranchInfoDTO
{
    public int BranchID { get; set; }
    public int BranchNumber { get; set; }
    public string BranchPrimaryName { get; set; }
    public string BranchSecondaryName { get; set; }
    public string SmallCurPrimaryName { get; set; }
    public string SmallCurSecondaryName { get; set; }
    public int CreatedBy { get; set; }
    public int CityId { get; set; }
    public int AreaId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ModifyBy { get; set; }
    public DateTime ModifyAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsNumberingAutomatic { get; set; }
    public string CurrencyPrimaryName { get; set; }
    public string CurrencySecondlyName { get; set; }
    public string CurrencyCode { get; set; }
    public int CurrencyIDH { get; set; }
    public string ISOCode { get; set; }
    public int BranchType { get; set; }
    public string CountryCode { get; set; }
}
public class InvoiceResponse
{
    public string Link { get; set; }
    public bool IsComplete { get; set; }
    public List<string> MissingData { get; set; }
}