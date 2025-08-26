using System.ComponentModel.DataAnnotations;

namespace ReportHub.Objects.DTOs.Common;



public class TransFilter
{
    public string AccountNo { get; set; }
    public decimal CAmount { get; set; }
    public decimal DAmount { get; set; }
    public Int64 TranNo { get; set; }
    public int TranTypeNo { get; set; }
    public string Notes { get; set; }
    public string UserName { get; set; }
    public int CurrencyID { get; set; }
    public string TransTypeEn { get; set; }
    public string TranType { get; set; }
    public string AccountName { get; set; }
    public DateTime TranDate { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Int64 FromTranNo { get; set; }
    public Int64 ToTranNo { get; set; }
    public bool? IsPosted { get; set; }
    public int IsCompanyCenterialized { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public decimal CostsCentersNo { get; set; }

    public string CostsCentersName { get; set; }
    public string language { get; set; }
    public decimal? Total { get; set; }
    public int Page { get; set; }
    public int? AgreementId { get; set; }

}

public class Transactions
{

    public List<TransTypeUD> oLTransTypeUD { get; set; }
    public List<TransFileTable> oLTransFileTable { get; set; }
    public List<ReceiptChequeUD> oLReceiptChequeUD { get; set; }
    public int VoucherType { get; set; }
    public int UserID { get; set; }


    public int CompanyId { get; set; }


    public int BranchId { get; set; }


}
public class TransFileTable
{
    public Int64 ID { get; set; }
    public Int64 TranNo { get; set; }
    public Int64 TranTypeNo { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Path { get; set; }

}
public class TransTypeUD
{
    public string KeyId { get; set; }
    public Int64 TranNo { get; set; }
    public Int64 TranTypeNo { get; set; }
    public string AccountNo { get; set; }
    public DateTime TranDate { get; set; }
    public decimal DAmount { get; set; }
    public decimal CAmount { get; set; }
    public string Notes { get; set; }
    public DateTime CreateDate { get; set; }
    public string UserName { get; set; }
    public Int64 CostsCentersNo { get; set; }
    public string HeaderNotes { get; set; }
    public int CurrencyID { get; set; }
    public int AreaId { get; set; }
    public int CityId { get; set; }
    public int AgreementId { get; set; }
    public string ReservationNo { get; set; }
}
public class ReceiptChequeUD
{
    public Int64 TranNo { get; set; }
    public Int64 TranTypeNo { get; set; }
    public DateTime TranDate { get; set; }
    public int Branch { get; set; }
    public string ReceivedFrom { get; set; }
    public int DelegateID { get; set; }
    public DateTime Date { get; set; }
    public string Indicator { get; set; }
    public decimal Total { get; set; }
    public int IsChash { get; set; }

    public string ChequeNumber { get; set; }
    public DateTime ChequeDate { get; set; }
    public string ChequeBank { get; set; }

    public string DescriptionCode { get; set; }
}
public class PaymentMethod
{
    public int MethodId { get; set; }
    public int MethodIdMethodSafety { get; set; }

    public string Code { get; set; }

    public string PrimaryName { get; set; }
    public string SecondaryName { get; set; }
    public Int64 AccountNo { get; set; }
    public double BankPercentage { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ModifyBy { get; set; }
    public DateTime ModifyAt { get; set; }
    public bool IsActive { set; get; }
    public PaymentMethodDetails RefMethodDetails { get; set; }
    public List<PaymentMethod> ColpaymentMethods { get; set; }
    public List<AccountTable> AccountNumbers { get; set; }
    public string AccountName { set; get; }
    public bool IsDeleted { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public bool ChequesNumber { set; get; }
    public bool ChequeBank { set; get; }
    public bool RequestDate { set; get; }
    public string Name { get; set; }
    public int? TajeerPaymentMethodCode { get; set; }
    public int? TajeerOtherPaymentMethodCode { get; set; }
    public bool IsHolding { get; set; }
    public string ReferenceNumber { get; set; }

}
public class PaymentMethodDetails
{
    public int Last4CardDigits { get; set; }
    public int ChequeNumber { get; set; }
    public int FK_PaymentMethod { get; set; }
    public int FK_ChequeBank { get; set; }
    public DateTime ChequeDate { get; set; }

}
public class AccountTable
{

    public Int64 ID { get; set; }
    public string AccountNo { get; set; }
    public string AccountName { get; set; }
    public bool AcceptTrans { get; set; }
    public bool Debit { get; set; }
    public bool Credit { get; set; }
    public bool ProfitandLoss { get; set; }
    public bool Budget { get; set; }
    public decimal StandardValue { get; set; }
    public bool AcceptCostCenter { get; set; }
    public Int64 ParentId { get; set; }
    public string AccountSecondaryName { get; set; }

    public int AccountLevel { get; set; }
    public string AccountNameNo { get; set; }
    public string AccountSecondaryNameNo { get; set; }

    // public AccountDetailsTable RefAccountDetails { get; set; }
    // public TransTable RefTrans { get; set; }
}
public class CurrencyTable
{
    public int CurrencyID { get; set; }
    public string CurrencyCode { get; set; }
    public string CurrencyPrimaryName { get; set; }
    public string CurrencySecondlyName { get; set; }
    public decimal CurrencyExchengeRete { get; set; }
    public int DCurrencyID { get; set; }
    public int CompanyId { get; set; }
    public int BranchID { get; set; }
    public string Error { get; set; }
    public int CreatedBy { get; set; }
    public int ModifiedBy { get; set; }
    public string Name { get; set; }

}
public class AccountTableNew
{

    public int ID { get; set; }
    public string AccountNo { get; set; }
    public string AccountName { get; set; }
    public bool AcceptTrans { get; set; }
    public bool Debit { get; set; }
    public bool Credit { get; set; }
    public bool ProfitandLoss { get; set; }
    public bool Budget { get; set; }
    public decimal StandardValue { get; set; }
    public bool AcceptCostCenter { get; set; }
    public Int64 ParentId { get; set; }
    public string ParentAccountName { get; set; }
    public string AccountSecondaryName { get; set; }

    public decimal DAmount { get; set; }
    public decimal CAmount { get; set; }
    public int AccountOrder { get; set; }
    public decimal Total { get; set; }
    public int AccountLevel { get; set; }

    public TransTable RefTrans { get; set; }
    public string AccountNameNo { get; set; }
    public string AccountSecondaryNameNo { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
}

public class TransTable
{
    public bool? Post { get; set; }
    public bool? Confirmed { get; set; }
    public Int64 ID { get; set; }
    public Int64 TranNo { get; set; }
    public Int64 MaxTranNo { get; set; }
    public Int64 TranTypeNo { get; set; }
    public string AccountNo { get; set; }
    public string ReceivedFrom { get; set; }
    public bool? IsAutoCreated { get; set; }
    public DateTime ReceivedDate { get; set; }
    public DateTime Date { get; set; }
    public string Indicator { get; set; }
    public int IsChash { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    public Int64 ChequeNumber { get; set; }
    public string ChequeBank { get; set; }
    public string ChequeBankName { get; set; }
    public int? DelegateId { get; set; }
    public DateTime? ChequeDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime TranDate { get; set; }
    public decimal DAmount { get; set; }
    public decimal CAmount { get; set; }
    public decimal Total { get; set; }
    public int VoucherType { get; set; }
    public string DescriptionCode { get; set; }
    public string Notes { get; set; }
    public string Link { get; set; }
    public string TranType { get; set; }
    public string AccountName { get; set; }
    public DateTime PostedOn { get; set; }
    public string PostedBy { get; set; }
    public string CreatedBy { get; set; }
    //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
    public DateTime CreateDate { get; set; }
    public string UserName { get; set; }
    public Int64 CostsCentersNo { get; set; }
    public string HeaderNotes { get; set; }
    public int CurrencyID { get; set; }
    public AccountTable oAccountTable { get; set; }
    public CentersCostsTable oCentersCostsTable { get; set; }
    public TransTypeTable oTransTypeTable { get; set; }
    public CurrencyTable oCurrencyTable { get; set; }
    public List<AccountTable> oLAccountTable { get; set; }
    public List<CentersCostsTable> oLCentersCostsTable { get; set; }
    public List<TransTable> oLTransaction { get; set; }
    public List<TransTypeTable> oLTransTypeTable { get; set; }
    public List<CurrencyTable> oLCurrencyTable { get; set; }


    public List<TransTable> oLAccountTableNoParent { get; set; }
    public int BranchId { get; set; }

    public BankBranch oBankBranch { get; set; }

    public List<BankBranch> oLBankBranch { get; set; }
    public string GetTransactinLink { get; set; }
    public string GetTransactinFilterLink { get; set; }
    public string ImageName { get; set; }
}
public class BankBranch
{
    public BankBranch()
    {
        oLCurrencyTable = new List<CurrencyTable>();
        oLAccountTable = new List<AccountTable>();

    }
    public int BankBranchId { get; set; }
    public string BranchPrimaryName { get; set; }
    public string BranchSecondaryName { get; set; }
    public string Address { get; set; }
    public int BankId { get; set; }
    public bool IsActive { get; set; }
    public bool IsExchangeable { get; set; }
    public Bank oBank { get; set; }
    public string AccountBankNo { get; set; }
    public List<Bank> oLBank { get; set; }
    public List<CurrencyTable> oLCurrencyTable { get; set; }
    public string BankAccountNo { get; set; }
    public int CurrencyId { get; set; }
    public List<AccountTable> oLAccountTable { get; set; }

}
public class Bank
{
    public int BankId { get; set; }
    public int BankNumber { get; set; }
    public string BankPrimaryName { get; set; }
    public string BankSecondaryName { get; set; }
    public string BankGroup { get; set; }

    public bool IsActive { get; set; }
    public string Error { get; set; }
}
public class TransTypeTable
{
    public Int64 ID { get; set; }
    public string TransCode { get; set; }

    public string TransType { get; set; }
    public string TransTypeArabic { get; set; }
    public bool YearFlage { get; set; }
    public bool MonthFlage { get; set; }
    public int VoucherType { get; set; }
    public string VoucherTypeName { get; set; }
    public bool IsAutoCreated { get; set; }
    public bool IsAutoPosted { get; set; }
    public bool IsAdvance { get; set; }

    public int IsAbleToChange { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public int CreatedBy { get; set; }
    public int ModifiedBy { get; set; }


    public string CreatedOn { get; set; }
    public string ModifiedOn { get; set; }
    public string CreatedByName { get; set; }
    public string ModifiedByName { get; set; }

}

public class CentersCostsTable
{


    public Int64 ID { get; set; }
    public Int64 CentersCostsNo { get; set; }
    public string CentersCostsName { get; set; }
    public Int64 ParentId { get; set; }

    public string CentersCostsSecondaryName { get; set; }

    public int CompanyId { get; set; }

    public int BranchId { get; set; }

    /// <summary>
    /// Auditing Fields
    /// </summary>
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ModifyBy { get; set; }
    public DateTime? ModifyAt { get; set; }
    public bool IsDeleted { set; get; }
    public int? DepreciationAccountId { get; set; }
    public int? ExpensesDepreciationId { get; set; }

}
public class CustomerInformation
{
    public Int64 Id { get; set; }
    public int CompanyId { get; set; }
    public int BranchIDH { get; set; }
    public int oLDBPaymentType { get; set; }
    public int? Type { get; set; }
    public Int64 ClientbranchId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPrimaryName { get; set; }
    public string CustomerSecondaryname { get; set; }
    public bool BelongsToCompany { get; set; }
    public DateTime Gregorianbirthdate { get; set; }
    public Int64 NationalityId { get; set; }
    public string Job { get; set; }
    public string HomeAdress { get; set; }
    public string workAddress { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhoneNumber { get; set; }
    public string BusinessEmail { get; set; }
    public string BusinessPhone { get; set; }
    public string Notes { get; set; }
    public Int64 TaxNumber { get; set; }
    public bool BlackList { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ModifyBy { get; set; }
    public DateTime ModifyAt { get; set; }
    public bool Status { get; set; }
    public string AccountNoReceivable { get; set; }
    public Int64 AccountNo { get; set; }
    public string AccountNoPayable { get; set; }
    public int SalesPaymentTerms { get; set; }
    public int SalesPurchaseTerms { get; set; }
    public string PhotoPath { get; set; }
    public string PhotoPathOld { get; set; }
    public List<CustomerNotes> Notess { get; set; }
    public List<AccountTable> oLAccountNoReceivable { get; set; }
    public List<AccountTable> oLAccountNoPayable { get; set; }
    public List<CreditCards> Credits { get; set; }
    public int TotalPages { get; set; }
    public int? FromWhere { get; set; }
    public double PaymenEvaluation { get; set; }
    public int PaymenEvaluationPerCondition { get; set; }
    public SalesInvoiceSettings SalesInvoiceSettings { get; set; }
    public int WorkNature { get; set; }
    public int StopServiceId { get; set; }
    public decimal CreditLimit { get; set; }
    //public List<HttpPostedFileBase> Files { set; get; }
    public string FilePath { set; get; }
    public List<string> FilesPaths { get; set; }
    public int CustomerTypeId { get; set; }
    public string IdCopyNumber { set; get; }
    public string IdPlaceOfIssue { set; get; }
    public DateTime? IdIssueDate { set; get; }
    public string CustomerRepresentativePrimaryName { set; get; }
    public string CustomerRepresentativeCapacityPrimaryName { set; get; }
    public string CustomerRepresentativeSecondaryname { set; get; }
    public string CustomerRepresentativeCapacitySecondaryname { set; get; }
    public string CustomerRepresentativeIdNumber { set; get; }
    public string RegistrationNo { set; get; }
    public string RegistrationHoldingNo { set; get; }
    public string CommercialRegistration { set; get; }

    public bool IsSettlement { set; get; }
    public string TermsAndConditions { set; get; }
    public string StreetName { get; set; }
    public string PostalCode { get; set; }
    public string DistrictName { get; set; }
    public string IdentityType { get; set; }
    public string BuildingNumber { get; set; }
    public string Alpha2 { get; set; }
    public int CityId { get; set; }
    public int AreaId { get; set; }
    public List<Area> AreasList { set; get; }
    public List<City> CitiesList { set; get; }
    public string CityName { get; set; }
    public string RegionName { get; set; }
    public int GroupAccountsId { get; set; }
    public int? ClassificationGroupId { get; set; } // 1. Collection  2. H.O   3.  Legal  4. Operations
    public int CurrencyId { get; set; }
    public int SalesTaxGroupId { get; set; }
    public List<GroupOfAccounts> GroupOfAccountsList { set; get; }
    public List<CurrencyTable> CurrencyList { set; get; }
    public List<SupplierClassification> SupplierClassification { set; get; }

    public int TypeOfCustomer { get; set; } // 1. Person, 2. Organization
    public string Language { get; set; } // 1. Ar, 2. en-us
    public string DebtorNumber { get; set; } // 1. Ar, 2. en-us
    public List<PaymentMethod> PaymentMethods { set; get; }
    public int PaymentMethodsId { get; set; }
    public List<TaxClassificationTable> TaxClassificationTable { get; set; }

    public int? Contract_CC_DimensionsId { get; set; }
    public int? Department_DimensionsId { get; set; }
    public int? LOB_DimensionsId { get; set; }
    public int? Locations_DimensionsId { get; set; }
    public int? Regions_DimensionsId { get; set; }
    public int? Customer_DimensionsId { get; set; }
    public int? Vendor_DimensionsId { get; set; }
    public bool ShowFinancialDimensions { get; set; }
    public int? SupplierClassificationId { get; set; }
    public bool IsIntegrationDynamic365 { get; set; }

}
public class SalesInvoiceSettings
{
    public string FromAccountNo { get; set; }
    public string ToAccountNo { get; set; }
    public string TaxAccountNo { get; set; }
    public string TaxPurchasesAccountNo { get; set; }
    public string ExtraCharge { get; set; }
    public string CashAccount { get; set; }
    public string PurchaseAccount { get; set; }
    public string SalesAccount { get; set; }
    public string AccountNoReceivable { get; set; }
    public string AccountNoPayable { get; set; }
    public string ItemSalesAccount { get; set; }
    public string ItemPurchasAccount { get; set; }


    public string ItemReturnSalesAccount { get; set; }
    public string ItemReturnPurchasAccount { get; set; }

    public int FromAccountNoId { get; set; }
    public int ToAccountNoId { get; set; }
    public int TaxAccountNoId { get; set; }
    public int TaxPurchasesAccountNoId { get; set; }

    public int ExtraChargeId { get; set; }
    public int CashAccountId { get; set; }
    public int PurchaseAccountId { get; set; }
    public int SalesAccountId { get; set; }
    public int AccountNoReceivableId { get; set; }
    public int AccountNoPayableId { get; set; }
    public int ItemSalesAccountId { get; set; }
    public int ItemPurchasAccountId { get; set; }
    public int ItemReturnSalesAccountId { get; set; }
    public int ItemReturnPurchasAccountId { get; set; }
    public int CompanyId { get; set; }
    public int Branch { get; set; }

    public string FromAccountNoName { get; set; }
    public string ToAccountNoName { get; set; }
    public string TaxAccountNoName { get; set; }
    public string TaxPurchasesAccountNoName { get; set; }

    public string ExtraChargeName { get; set; }
    public string CashAccountName { get; set; }
    public string PurchaseAccountName { get; set; }
    public string SalesAccountName { get; set; }
    public string AccountNoReceivableName { get; set; }
    public string AccountNoPayableName { get; set; }
    public string ItemSalesAccountName { get; set; }
    public string ItemPurchasAccountName { get; set; }

    public string ItemReturnSalesAccountName { get; set; }
    public string ItemReturnPurchasAccountName { get; set; }

    public bool ShowCostCenter { get; set; }
    public bool ShowUnit { get; set; }
    public bool DiscountType { get; set; }
    public bool ShowCustomersAndVendorsOnChartOfAccounts { get; set; }

    public string POSCustomer { get; set; }

    public int EvaluationType { get; set; }
    public bool ShowFinancialDimensions { get; set; }
    public bool IsIntegrationDynamic365 { get; set; }

}
public class CreditCards
{

    public Int64 Id { get; set; }

    public string CreditType { get; set; }
    public string CreditName { get; set; }
    public string CardNo { get; set; }
    public Int64 ExpiryDYear { get; set; }
    public Int64 ExpiryDMonth { get; set; }
    public Int64 CVS { get; set; }
    public Int64 CustomerId { get; set; }
    // public AccountDetailsTable RefAccountDetails { get; set; }
    // public TransTable RefTrans { get; set; }
}
public class TaxClassificationTable
{
    public int TaxClassificationNo { get; set; }
    public string TaxClassificationName { get; set; }
    public string TaxClassificationArabicName { get; set; }
    public decimal TaxRate { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public string Name { get; set; }
    public string TaxClassificationCode { get; set; }


}
public class SupplierClassification
{
    public int Id { get; set; }
    public string PrimaryName { get; set; }
    public string Name { get; set; }

    public string SecondaryName { get; set; }
    public int CompanyId { get; set; }
    public int ModifiedBy { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }


}
public class Area
{
    public int Id { get; set; }
    public int CountryId { set; get; }
    public string Name { set; get; }
    public string PrimaryName { set; get; }
    public string SeconderyName { set; get; }
}
public class City
{
    public int Id { get; set; }
    public int AreaId { set; get; }
    public string Name { set; get; }
    public string PrimaryName { set; get; }
    public string SeconderyName { set; get; }
}
public class GroupOfAccounts
{
    public int Id { get; set; }
    public string GroupCode { get; set; }

    public string PrimaryName { get; set; }
    public string SecondaryName { get; set; }
    public int AccountId { get; set; }
    public int TaxGroup { get; set; } // 1 Taxable  2. Non Taxable
    public int CompanyId { get; set; }
    public int CreatedBy { get; set; }
    public int ModifiedBy { get; set; }
    public int TypeOfGroup { get; set; } // 1 Customer  2 Vendor
    public string Name { get; set; }
    public List<AccountTable> AccountTable { get; set; }
    public List<TaxClassificationTable> TaxClassificationTable { get; set; }


}
public class CustomerNotes
{

    public Int64 Id { get; set; }

    public string Note { get; set; }
    public Int64 CustomerId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public int? FromWhere { get; set; }
    public int? Type { get; set; }

    // public AccountDetailsTable RefAccountDetails { get; set; }
    // public TransTable RefTrans { get; set; }
}
public class PaymentMethodV2
{
    public int MethodId { get; set; }
    public string Code { get; set; }

    public string PrimaryName { get; set; }
    public string SecondaryName { get; set; }
    public Int64 AccountNo { get; set; }
    public double BankPercentage { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ModifyBy { get; set; }
    public DateTime ModifyAt { get; set; }
    public bool IsActive { set; get; }
    public List<PaymentMethod> ColpaymentMethods { get; set; }
    public List<AccountTable> AccountNumbers { get; set; }
    public string AccountName { set; get; }
    public bool IsDeleted { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }


    public bool ChequesNumber { set; get; }

    public bool ChequeBank { set; get; }

    //public DateTime Gregorianbirthdate { get; set; }
    //public DateTime? hijribirthdate { get; set; }
    //public bool ishijri { get; set; }

    public bool RequestDate { set; get; }
    public string Name { get; set; }

    public int? TajeerPaymentMethodCode { get; set; }
    public int? TajeerOtherPaymentMethodCode { get; set; }
    public bool IsHolding { get; set; }


}

public class ItemUnit
{
    public int Id { get; set; }
    public int? ItemId { get; set; }
    public int? UnitId { get; set; }
    public bool IsBasic { get; set; }
    public decimal Rate { get; set; }
    public string KeyId { get; set; }
    public string UnitPrimaryName { get; set; }
    public string UnitSecondaryName { get; set; }
    public string UnitName { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
}
public class Item
{

    public Int64 ItemId { get; set; }
    public int ItemNumber { get; set; }
    public string ItemBarcode { get; set; }
    public string ItemPrimaryName { get; set; }
    public string ItemSecondaryName { get; set; }
    public int CategoryId { get; set; }
    public string ItemPurchaseAccount { get; set; }
    public string ItemSalesAccount { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }

    public string ItemPurchaseAccountId { get; set; }
    public string ItemSalesAccountId { get; set; }
    public bool IsActive { get; set; }
    public decimal taxRate { get; set; }
    public decimal Price { get; set; }
    public bool Service { get; set; }
    public decimal PurchasingPrice { get; set; }
    public decimal ExtraCharge { get; set; }
    public string CategoryPrimaryName { get; set; }
    public string CategorySecondaryName { get; set; }

    public int TaxClassificationNo { get; set; }
    public int UnitId { get; set; }
    public string Image { get; set; }

    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public int CreatedBy { get; set; }

    public int ModifiedBy { get; set; }
    public string ItemUnitToSave { get; set; }
    public decimal MinQuantity { get; set; }
    public string BarcodeSymbolog { get; set; }
    public bool IsMaterial { get; set; }
    public int CriticalLimit { get; set; }
    public int? FromWhere { get; set; }
    public int ServiceTime { get; set; }
}
