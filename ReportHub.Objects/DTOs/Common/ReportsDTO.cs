namespace ReportHub.Objects.DTOs.Common
{
    class ReportsDTO
    {
    }

    public class InvoiceReport_CompanyInfoDTO
    {
        //public int Id { get; set; }
        //public int CompanyNumber { get; set; }
        //public string CompanySecondaryName { get; set; }
        //public string CompanyPrimaryName { get; set; }
        //public string Phone { get; set; }
        //public string Address { get; set; }
        //public string Email { get; set; }

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
    public class InvoiceReport_LeadOrderInfoDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? AccCustomerId { get; set; }
        public int SalesCustomerId { get; set; }
        public string HBL { get; set; }
        public string MBL { get; set; }
        public string Awe { get; set; }
        public int? Shipper { get; set; }
        public int? Consigne { get; set; }
        public int FromSourceID { get; set; }
        public int ToDestinationID { get; set; }
        public int LeadStatusId { get; set; }
        public int ServiceTypeDetailsId { get; set; }
        public int LogisticsType_Id { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public string TruckNumber { get; set; }
        public string ConsigneeName { get; set; }
        public string ShipperName { get; set; }
    }
    public class InvoiceReport_LeadOrderDetailsDTO
    {
        public int OrderLeadesDetailsID { get; set; }
        public decimal Quantity { get; set; }
        public int UnitID { get; set; }
        public int DescriptionID { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal Cost { get; set; }
        public string DescriptionStr { get; set; }
        public int CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public string UnitName { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class InvoiceReport_CompanyBankInfoDTO
    {
        public int BankId { get; set; }
        public int BankNumber { get; set; }
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
    public class InvoiceReportDTO
    {
        public InvoiceReport_CompanyInfoDTO InvoiceReport_CompanyInfo { get; set; }
        public InvoiceReport_LeadOrderInfoDTO InvoiceReport_LeadOrderInfo { get; set; }
        public List<InvoiceReport_CompanyBankInfoDTO> InvoiceReport_CompanyBankInfo { get; set; }
        public List<InvoiceReport_LeadOrderDetailsDTO> InvoiceReport_LeadOrderDetails { get; set; }
    }
    public class InvoiceResponseDTO
    {
        public InvoiceResponseDTO()
        {
            Currencies = new List<CurrencyTable>();
        }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public InvoiceTo InvoiceTo { get; set; }
        public ShipmentDetails ShipmentDetails { get; set; }
        public List<ChargesDTO> Charges { get; set; }
        public ChargesTotalDTO ChargesTotal { get; set; }
        public BankWithDetailsDTO BankWithDetail { get; set; }
        public List<BankWithDetailsDTO> BankWithDetails { get; set; }
        public List<CurrencyTable> Currencies { get; set; }
    }

    public class InvoiceTo
    {
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string toCityName { get; set; }
        public int AccCustomerID { get; set; }
        public string toCountryName { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string ClientReference { get; set; }
        public string NubaReference { get; set; }
    }
    public class ShipmentDetails
    {
        public string Service { get; set; }
        public int ServiceTypeId { get; set; }
        public int ServiceTypeDetailId { get; set; }
        public string ServiceDetail { get; set; }
        public string ServiceDetailCode { get; set; }
        public string Shipper { get; set; }
        public string AWB { get; set; }
        public string Consignee { get; set; }
        public int DestinationID { get; set; }
        public string NOP { get; set; }
        public decimal GrossWeight { get; set; }
        public string Volume { get; set; }
        public string MBL { get; set; }
        public string HBL { get; set; }
        public decimal Weight { get; set; }
        public int FromSourceID { get; set; }
        public int ToDestinationID { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string IENV_QR { get; set; }
    }
    public class ChargesTotalDTO
    {
        public decimal TotalDue { get; set; }
        public decimal USDTotalDue { get; set; }
    }
}
