
using ReportHub.Objects.DTOs;
using ReportHub.Objects.DTOs.Common;
using Newtonsoft.Json;
using static ReportHub.Objects.DTOs.HelpersViewModel.WebClientViewModel;
using ReportHub.Common.DataHelpers.IDataHelpers;

namespace ReportHub.DBCommon;

public sealed class ClinetAPIHelper
{
    #region Repo
    public readonly IConfiguration _configuration;
    public readonly IClient _IClient;
    private string AccBaseUrl;
    private string ERPBaseUrl;
    private int CompanyID;
    private int BranchID;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public ClinetAPIHelper(
        IConfiguration configuration,
        IClient iClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        AccBaseUrl = _configuration.GetSection("APIs:Accounting:BaseURL").Value;
        ERPBaseUrl = _configuration.GetSection("APIs:ERP:BaseURL").Value;
        _IClient = iClient;
        _httpContextAccessor = httpContextAccessor;
        CompanyID = (int)(_httpContextAccessor.HttpContext?.Items["CompanyID"] ?? 0);
        BranchID = (int)(_httpContextAccessor.HttpContext?.Items["BranchID"] ?? 0);
    }
    #endregion

    #region General APIs

    public async Task<CompanyInfo> GetCompanyInfo()
    {
        try
        {
            var Url = _configuration.GetSection("APIs:ERP:ERPAPI:GetCompanyInfo").Value;
            string responseCountry = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = ERPBaseUrl,
                Url = Url + "?CompanyId=" + this.CompanyID,
            });

            var info = JsonConvert.DeserializeObject<APIReponseSingle<CompanyInfo>>(responseCountry);
            if (info == null) return new CompanyInfo();

            return info.ResponseDetails;
        }
        catch (Exception)
        {
            return new CompanyInfo();
        }
    }

    public async Task<CompanyBranch> GetBranchInfo()
    {
        try
        {
            var Url = _configuration.GetSection("APIs:ERP:ERPAPI:GetBranchInfo").Value;
            string responseCountry = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = ERPBaseUrl,
                Url = Url + "?Id=" + this.BranchID,
            });

            var countries = JsonConvert.DeserializeObject<APIReponseSingle<CompanyBranch>>(responseCountry);
            if (countries == null) return new CompanyBranch();

            return countries.ResponseDetails;
        }
        catch (Exception)
        {
            return new CompanyBranch();
        }
    }

    public async Task<List<CountryDTO>> GetAllCountriesWithCities()
    {
        try
        {
            var UrlCountry = _configuration.GetSection("APIs:ERP:ERPAPI:GetCountry").Value;
            var UrlCity = _configuration.GetSection("APIs:ERP:ERPAPI:CountryArea").Value;

            string responseCountry = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = ERPBaseUrl,
                Url = UrlCountry + "?lang=en",
            });

            var countries = JsonConvert.DeserializeObject<APIReponse<CountryDTO>>(responseCountry);
            if (countries == null || !countries.ResponseDetails.Any()) return new List<CountryDTO>();


            string responseCity = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = ERPBaseUrl,
                Url = UrlCity + "?CountryID=",
            });

            var cities = JsonConvert.DeserializeObject<APIReponse<AreaDTO>>(responseCity);
            if (cities.ResponseDetails == null) cities.ResponseDetails = new List<AreaDTO>();

            foreach (var country in countries.ResponseDetails)
            {
                country.Area = cities.ResponseDetails.Where(city => city.CountryId == country.Id).ToList();
            }

            return countries.ResponseDetails;
        }
        catch (Exception)
        {
            return new List<CountryDTO>();
        }
    }

    public async Task<List<CurrencyTable>> GetCurrencies()
    {
        try
        {
            var Url = _configuration.GetSection("APIs:ERP:ERPAPI:Currencies").Value;
            string responseCountry = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = ERPBaseUrl,
                Url = Url + "?CompanyId=" + this.CompanyID + "&BranchId=" + this.BranchID,
            });

            var countries = JsonConvert.DeserializeObject<APIReponse<CurrencyTable>>(responseCountry);
            if (countries == null || !countries.ResponseDetails.Any()) return new List<CurrencyTable>();

            return countries.ResponseDetails;
        }
        catch (Exception)
        {
            return new List<CurrencyTable>();
        }
    }

    public async Task<List<BankWithDetailsDTO>> GetBankWithDetails(int CompanyId, int BranchID)
    {
        try
        {
            var Url = _configuration.GetSection("APIs:Accounting:AccAPI:GetBank_WithDetails").Value;
            string Response = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = AccBaseUrl,
                Url = Url + "?CompanyId=" + CompanyId + "&BranchId=" + BranchID,
            });

            var res = JsonConvert.DeserializeObject<APIReponse<BankWithDetailsDTO>>(Response);
            return res.ResponseDetails;
        }
        catch (Exception)
        {

            return new List<BankWithDetailsDTO>();
        }
    }

    public async Task<List<CustomerInformation>> Customers_GetAll(int CompanyId, int BranchId, int IsCompanyCenterialized, string Language = "en")
    {
        var UrlCustomer_Find = _configuration.GetSection("APIs:Accounting:AccAPI:Customers_GetAll").Value;

        string responseChartOfAccountAcceptTrans = await _IClient.Get(new GetRequestViewModel
        {
            BaseUrl = AccBaseUrl,
            Url = UrlCustomer_Find + "?CompanyId=" + CompanyId + "&BranchId=" + BranchId + "&IsCompanyCenterialized=" + IsCompanyCenterialized + "&Language=" + Language,
        });

        var AccountAcceptTrans = JsonConvert.DeserializeObject<APIReponse<CustomerInformation>>(responseChartOfAccountAcceptTrans);
        if (AccountAcceptTrans == null) return new List<CustomerInformation>();
        return AccountAcceptTrans.ResponseDetails;
    }

    public async Task<List<Item>> Get_Items(int CompanyID, int BranchID)
    {
        try
        {
            var Url = _configuration.GetSection("APIs:Accounting:AccAPI:Get_Items").Value;
            string responseItems = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = AccBaseUrl,
                //Url = Url + "?ItemId=" + null + "&CompanyId=" + this.CompanyID + "&BranchId=" + this.BranchID + "&Language=en",
                Url = Url + "?CompanyId=" + CompanyID + "&BranchId=" + BranchID + "&Language=en",
            });

            var UnitS = JsonConvert.DeserializeObject<APIReponse<Item>>(responseItems);
            if (UnitS == null || !UnitS.ResponseDetails.Any()) return new List<Item>();

            return UnitS.ResponseDetails;
        }
        catch (Exception)
        {
            return new List<Item>();
        }
    }

    public async Task<List<ItemUnit>> Get_ItemUnits(int CompanyID, int BranchID)
    {
        try
        {
            var Url = _configuration.GetSection("APIs:Accounting:AccAPI:Get_ItemUnits").Value;
            string responseunit = await _IClient.Get(new GetRequestViewModel
            {
                BaseUrl = AccBaseUrl,
                Url = Url + "?ItemId=" + null + "&CompanyId=" + CompanyID + "&BranchId=" + BranchID + "&Language=en",
            });

            var UnitS = JsonConvert.DeserializeObject<APIReponse<ItemUnit>>(responseunit);
            if (UnitS == null || !UnitS.ResponseDetails.Any()) return new List<ItemUnit>();

            return UnitS.ResponseDetails;
        }
        catch (Exception)
        {
            return new List<ItemUnit>();
        }
    }
    #endregion

 


    #region Sales Invoice

    public async Task<TypeSalesPurchases> TypeSalesPurchases_GetById(int Id)
    {
        var UrlCustomer_Find = _configuration.GetSection("APIs:Accounting:AccAPI:TypeSalesPurchases_GetById").Value;

        string responseChartOfAccountAcceptTrans = await _IClient.Get(new GetRequestViewModel
        {
            BaseUrl = AccBaseUrl,
            Url = UrlCustomer_Find + "?Id=" + Id,
        });

        var AccountAcceptTrans = JsonConvert.DeserializeObject<APIReponseSingle<TypeSalesPurchases>>(responseChartOfAccountAcceptTrans);
        if (AccountAcceptTrans == null) return new TypeSalesPurchases();
        return AccountAcceptTrans.ResponseDetails;
    }


    public async Task<List<TaxClassificationTable>> TaxClassificationGet(int CompanyId, int BranchId, string lang = "en")
    {
        var UrlCustomer_Find = _configuration.GetSection("APIs:Accounting:AccAPI:TaxClassificationGet").Value;

        string responseChartOfAccountAcceptTrans = await _IClient.Get(new GetRequestViewModel
        {
            BaseUrl = AccBaseUrl,
            Url = UrlCustomer_Find + "?CompanyId=" + CompanyId + "&BranchId=" + BranchId + "&lang=" + lang,
        });

        var AccountAcceptTrans = JsonConvert.DeserializeObject<APIReponse<TaxClassificationTable>>(responseChartOfAccountAcceptTrans);
        if (AccountAcceptTrans == null) return new List<TaxClassificationTable>();
        return AccountAcceptTrans.ResponseDetails;
    }

    public async Task<AccountSalesMaster> AccountSalesMaster_Insert(AccountSales accountSales, bool isSpot = false)
    {
        var UrlCustomer_Find = _configuration.GetSection("APIs:Accounting:AccAPI:AccountSalesMaster_Insert").Value;

        string responseChartOfAccountAcceptTrans = await _IClient.Post(new PostRequestViewModel
        {
            BaseUrl = AccBaseUrl,
            SerializedContent = JsonConvert.SerializeObject(accountSales),
            Url = UrlCustomer_Find
        });

        var AccountAcceptTrans = JsonConvert.DeserializeObject<APIReponseSingle<AccountSalesMaster>>(responseChartOfAccountAcceptTrans);
        if (AccountAcceptTrans == null) return null;
        return AccountAcceptTrans.ResponseDetails;
    }


    #endregion


 

}
