namespace ReportHub.Objects.DTOs.Common;

public class InvoiceSalesDTO
{
}
public class AccountSalesMaster
{
    public AccountSalesMaster()
    {
        oBranchTable = new CompanyBranch();
        oLAccountTable = new List<AccountTable>();
        oLCentersCostsTable = new List<CentersCostsTable>();
        oLTransTypeTable = new List<TransTypeTable>();
        oLCurrencyTable = new List<CurrencyTable>();
        oLBranchTable = new List<CompanyBranch>();
        oLInvoiceEdit = new List<InvoiceEdit>();
        oCustomerInformation = new CustomerInformation();

    }
    public decimal ID { get; set; }
    public decimal AccSalesNo { get; set; }
    public int AccSalesTypeNo { get; set; }
    public int AccSalesBranch { get; set; }
    public DateTime AccSalesDate { get; set; }
    public string Notes { get; set; }
    public decimal Total { get; set; }
    public string CAccountNo { get; set; }
    public string DAccountNo { get; set; }
    public decimal Discount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int InventoryAccountId { get; set; }
    public string PoNo { get; set; }
    public decimal Net { get; set; }
    public decimal Tax { get; set; }
    public decimal Final { get; set; }
    public string Link { get; set; }
    public string Reference { get; set; }

    public int CurrencyID { get; set; }
    public decimal TotalExtraCharge { get; set; }
    public decimal PolicyNo { get; set; }
    public decimal PackageNo { get; set; }
    public decimal Weight { get; set; }
    public decimal NoticeNum { get; set; }
    public string Shipper { get; set; }
    public string ShipperLine { get; set; }
    public string Imo { get; set; }
    public DateTime SupplyDate { get; set; }
    public decimal Currency { get; set; }
    public decimal Cash { get; set; }
    public int InvoiceTypeNo { get; set; }
    public int InvoiceType { get; set; }
    public int PaymentType { get; set; }
    public string UserId { get; set; }
    public int PaymentTerms { get; set; }
    public bool Paid { get; set; }
    public int DelegateID { get; set; }
    public int TypeSalesPurchasesID { get; set; }
    public bool IsFiexdAsset { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreateDate { get; set; }
    public DBPaymentType oDBPaymentType { get; set; }
    public AccountTable oAccountTable { get; set; }
    public CentersCostsTable oCentersCostsTable { get; set; }
    public TransTypeTable oTransTypeTable { get; set; }
    public CurrencyTable oCurrencyTable { get; set; }

    public CompanyBranch oBranchTable { get; set; }
    public List<AccountTable> oLAccountTable { get; set; }
    public List<CentersCostsTable> oLCentersCostsTable { get; set; }
    public List<TransTypeTable> oLTransTypeTable { get; set; }
    public List<CurrencyTable> oLCurrencyTable { get; set; }
    public List<CompanyBranch> oLBranchTable { get; set; }
    public List<InvoiceEdit> oLInvoiceEdit { get; set; }
    public CustomerInformation oCustomerInformation { get; set; }
    public string InvoiceToSave { get; set; }
    public DateTime ReferenceInvoiceDate { get; set; }
    public int ZatcaOrder { get; set; }
    public string InvoiceHash { get; set; }
    public bool ReportedToZatca { get; set; }
    public string ReportingResult { get; set; }
    public string ReportingStatus { get; set; }
    public string QrCode { get; set; }
    public string SignedXml { get; set; }
    public DateTime ZatcaSubmissionDate { get; set; }
    public int ZatcaInvoiceType { get; set; }
    public int ZatcaInvoiceTypeCode { get; set; }
    public int BranchId { get; set; }
    public int CompanyId { get; set; }
    public bool? IsSpot { get; set; }
    public int CustomerId { get; set; }
    public int TotalReportedInvoice { get; set; }
    public int Fk_AgreementId { get; set; }
    public string ReservationNo { get; set; }
    public string SupplierInvoiceNo { get; set; }

}


public class InvoiceEdit
{
    //        { ID: 1, ItemNumber: 1, ItemName: '', ItemCategory: '', 
    //            ItemAccount: 1, CostsCentersNo: 1, CostsCentersName: [],
    // Description: [], Reference: [], ClientTaxNo: [], ClientName: [], 
    //Quantity: 0, Price: 0, Total: 0, TaxValue: 0, Final: 0, ExtraCharge:0}
    public int ID { get; set; }
    public Int64 ItemNumber { get; set; }
    public string ItemName { get; set; }
    public string ItemCategory { get; set; }
    public string ItemAccount { get; set; }
    public decimal CostsCentersNo { get; set; }
    public string CostsCentersName { get; set; }
    public string Description { get; set; }
    public string Reference { get; set; }
    public string ClientTaxNo { get; set; }
    public string ClientName { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxValue { get; set; }
    public decimal ExtraCharge { get; set; }
    public int TaxClassificationId { get; set; }
    public decimal Final { get; set; }
}

public class DBPaymentType
{
    public int Id { get; set; }
    public string PrimaryName { get; set; }
    public string SecondaryName { get; set; }
    public int NumberOfDay { get; set; }
    public bool Ajax { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public int ModifiedBy { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedOn { get; set; }
    public string ModifiedOn { get; set; }
    public string CreatedByName { get; set; }
    public string ModifiedByName { get; set; }
}


public class TypeSalesPurchases
{
    public int InvoiceType { get; set; } // 1 Seles 2. Procurment
    public int Id { get; set; }
    public string PrimaryName { get; set; }
    public string SecondaryName { get; set; }
    public int AccountId { get; set; }
    public bool IsFiexdAsset { get; set; }
    public int BranchId { get; set; }
    public int CompanyId { get; set; }
    public List<AccountTable> AccountTable { get; set; }
}


public class AccountSalesDetails
{
    public string KeyId { get; set; }
    public decimal id { get; set; }
    public decimal AccSalesNo { get; set; }
    public int AccSalesTypeNo { get; set; }
    public Int64? ItemNumber { get; set; }
    public int? UnitId { get; set; }
    public decimal? CostsCentersNo { get; set; }
    public string Description { get; set; }
    public string ClientTaxNo { get; set; }
    public string ClientName { get; set; }
    public string Reference { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitQuantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int? TaxClassificationId { get; set; }
    public decimal Tax { get; set; }
    public decimal Final { get; set; }
    public decimal ExtraCharge { get; set; }
    public decimal AccSalesMasterNo { get; set; }
    public int? WarehouseId { get; set; }
    public decimal ItemCost { get; set; }
}
public class AccountSales
{
    public AccountSalesMaster AccountSalesMaster { get; set; }
    public List<AccountSalesDetails> AccountSalesDetails { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public int CompanyType { get; set; }
    public int AreaId { get; set; }
    public int CityId { get; set; }
}