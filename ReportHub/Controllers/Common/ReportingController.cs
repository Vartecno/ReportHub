//using DevExpress.XtraReports.UI;
//using ReportHub.DataAccess.DataHelpers.IDataHelpers;
//using ReportHub.DataAccess.IRepositories.CustomRepo;
//using ReportHub.DataAccess.IRepositories.ICustomRepo;
//using ReportHub.DataAccess.Repositories;
//using ReportHub.Objects.DTOs;
//using ReportHub.Objects.DTOs.Common;
//using ReportHub.Objects.Enum;
//using ReportHub.CustomAttributes;
//using ReportHub.DBCommon;
//using ReportHub.Reporting;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Swashbuckle.AspNetCore.Annotations;
//using System.ComponentModel.DataAnnotations;
//using static LogisticsSales.Objects.DTOs.HelpersViewModel.WebClientViewModel;

//namespace ReportHub.Controllers.Common;

//[Route("api/[controller]/[action]")]
//[ApiExplorerSettings(GroupName = "v2")]
//[ApiController]
//[CustomAuthorize([(int)UserType.Admin])]
//public class ReportingController : Base
//{
//    #region Repo
//    private readonly ILoggerHelper _loggerHelper;
//    private readonly ICustomRepository _customRepository;
//    public readonly IConfiguration _configuration;
//    public readonly IClient _IClient;
//    private readonly ILOG_LeadStatusRepository _leadStatus;
//    private string AccBaseUrl;
//    private readonly IReportRepository _reportRepository;
//    private string ERPBaseUrl;
//    private readonly ILOG_OrderLeadesRepository _log_OrderLeadesRepository;
//    private readonly IHttpContextAccessor _httpContextAccessor;
//    private ClinetAPIHelper clinetAPIHelper;

//    public ReportingController(ILoggerHelper loggerHelper,
//        ICustomRepository customRepository,
//        IConfiguration configuration,
//        IReportRepository reportRepository,
//        IClient iClient,
//        ILOG_OrderLeadesRepository log_OrderLeadesRepository,
//        IHttpContextAccessor httpContextAccessor,
//        ILOG_LeadStatusRepository leadStatus)
//    {
//        _loggerHelper = loggerHelper;
//        _customRepository = customRepository;
//        _configuration = configuration;
//        AccBaseUrl = _configuration.GetSection("APIs:Accounting:BaseURL").Value;
//        _reportRepository = reportRepository;
//        ERPBaseUrl = _configuration.GetSection("APIs:ERP:BaseURL").Value;
//        _IClient = iClient;
//        _log_OrderLeadesRepository = log_OrderLeadesRepository;
//        _httpContextAccessor = httpContextAccessor;
//        this.clinetAPIHelper = new ClinetAPIHelper(_configuration, _IClient, _httpContextAccessor);
//        _leadStatus = leadStatus;
//    }
//    #endregion


//    [HttpGet]
//    public async Task<IActionResult> ExportInvoice([Required] int OrderLeadId, [Required] string OrderLeadCode, int PrintedCurrencyID = 0)
//    {
//        try
//        {
//            var MissingDate = await _log_OrderLeadesRepository.allColumnsHaveData(OrderLeadId, OrderLeadCode);
//            if (MissingDate.Any()) return Ok(new InvoiceResponse
//            {
//                Link = "",
//                IsComplete = false,
//                MissingData = MissingDate
//            });
//            var url = _configuration.GetSection("APIs:Logistics:BaseURL").Value;
//            //string fileUrl = $"{url}/Reports/Invoice_{OrderLeadCode}.pdf";
//            string fileUrl = string.Empty;

//            var CompanyInfo = await this.GetCompanyInfo();
//            //var AccCustomerObj = await _reportRepository.getLeadewithAccCustomer(OrderLeadId);
//            //if (AccCustomerObj is null || AccCustomerObj.AccCustomarID == 0) return BadRequest("LblPleaseFillCustomer");
//            if (PrintedCurrencyID is 0)
//            {
//                PrintedCurrencyID = CompanyInfo.CurrencyIDH;
//            }
//            var country = await GetAllCountriesWithCities();
//            var currencies = await GetCurrencies();
//            var BankWithDetails = await this.GetBankWithDetails(CompanyInfo.Id, this.BranchID);
//            var units = await this.Get_ItemUnits();
//            var items = await this.Get_Items();
//            var data = await _customRepository.ExportInvoice(OrderLeadId, CompanyInfo, BankWithDetails, country, currencies, PrintedCurrencyID, units, items);
//            var obj = await _log_OrderLeadesRepository.FindRow(x => x.Id == OrderLeadId);
//            if (data is not null && data.InvoiceTo.AccCustomerID != 0)
//            {
//                var AccCustomer = await this.clinetAPIHelper.Customer_Find(data.InvoiceTo.AccCustomerID);
//                if (AccCustomer is not null)
//                {
//                    data.InvoiceTo.CustomerName = AccCustomer.CustomerPrimaryName;
//                }

//                data.BankWithDetails = BankWithDetails;
//            }

//            data.Currencies = currencies;
//            //XtraReport report = new InvoiceReport(data, PrintedCurrencyID);
//            bool isHaveDiscount = data.Charges.Any(x => x.Discount > 0);
//            XtraReport report = new();
//            if (isHaveDiscount)
//            {
//                report = new DiscountInvoiceReport(data, PrintedCurrencyID);
//            }
//            else
//            {
//                report = new InvoiceReport(data, PrintedCurrencyID);

//            }

//            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Reports");

//            string sanitizedCustomer = SanitizeFileName(data.InvoiceTo.CustomerName);
//            string sanitizedService = SanitizeFileName(data.ShipmentDetails.ServiceDetailCode);
//            string sanitizedOrigin = SanitizeFileName(data.ShipmentDetails.Origin);
//            string sanitizedDestination = SanitizeFileName(data.ShipmentDetails.Destination);

//            string fileName = $"{obj.InvoiceID}-{sanitizedCustomer}-{sanitizedService}-{sanitizedOrigin}-{sanitizedDestination}.pdf";
//            string filePath = Path.Combine(folderPath, fileName);



//            if (!Directory.Exists(folderPath))
//            {
//                Directory.CreateDirectory(folderPath);
//            }


//            //obj.LeadStatusId = 3;
//            var LeadStatusID = _leadStatus.FindRow(a => a.LeadStatusNameEn.Contains("Invoice Issued")).GetAwaiter().GetResult().Id;
//            obj.LeadStatusId = LeadStatusID;
//            await _log_OrderLeadesRepository.UpdateWithTransaction(obj);

//            fileUrl = $"{url}/Reports/{obj.InvoiceID}-{sanitizedCustomer}-{sanitizedService}-{sanitizedOrigin}-{sanitizedDestination}.pdf";
//            report.ExportToPdf(filePath);

//            return Ok(new InvoiceResponse
//            {
//                Link = fileUrl,
//                IsComplete = true,
//                MissingData = []
//            });
//        }
//        catch (Exception ex)
//        {
//            await _loggerHelper.AddLog(HttpContext);
//            return BadRequest("LblError400 : " + ex);
//        }
//    }

//    [HttpGet]
//    public async Task<IActionResult> ExportInvoiceV2([Required] int OrderLeadId, [Required] string OrderLeadCode, [Required] int OrderLeadMediatorID, int PrintedCurrencyID = 0)
//    {
//        try
//        {
//            var MissingDate = await _log_OrderLeadesRepository.allColumnsHaveData(OrderLeadId, OrderLeadCode);
//            if (MissingDate.Any()) return Ok(new InvoiceResponse
//            {
//                Link = "",
//                IsComplete = false,
//                MissingData = MissingDate
//            });
//            var url = _configuration.GetSection("APIs:Logistics:BaseURL").Value;
//            string fileUrl = string.Empty;

//            var CompanyInfo = await this.GetCompanyInfo();
//            if (PrintedCurrencyID is 0)
//            {
//                PrintedCurrencyID = CompanyInfo.CurrencyIDH;
//            }
//            var country = await GetAllCountriesWithCities();
//            var currencies = await GetCurrencies();
//            var BankWithDetails = await this.GetBankWithDetails(CompanyInfo.Id, this.BranchID);
//            var units = await this.Get_ItemUnits();
//            var items = await this.Get_Items();
//            var data = await _customRepository.ExportInvoiceV2(OrderLeadId, CompanyInfo, BankWithDetails, country, currencies, PrintedCurrencyID, units, items, OrderLeadMediatorID);
//            // call api qr
//            int _TransType = data.Charges.Any() && data.Charges.Select(q => q.TransType).FirstOrDefault() != 0 ? data.Charges.Select(q => q.TransType).FirstOrDefault() : 6;
//            string InvoiceID = data.Charges.Select(x => x.InvoiceID).FirstOrDefault();
//            var _QRObj = await this.clinetAPIHelper.GetQrCode(InvoiceID, _TransType);
//            var obj = await _log_OrderLeadesRepository.FindRow(x => x.Id == OrderLeadId);
//            if (data is not null && data.InvoiceTo.AccCustomerID != 0)
//            {
//                var AccCustomer = await this.clinetAPIHelper.Customer_Find(data.InvoiceTo.AccCustomerID);
//                if (AccCustomer is not null)
//                {
//                    data.InvoiceTo.CustomerName = AccCustomer.CustomerPrimaryName;
//                }
//                data.BankWithDetails = BankWithDetails;
//            }

//            data.Currencies = currencies;
//            data.ShipmentDetails.IENV_QR = _QRObj.QrCode;

//            bool isHaveDiscount = data.Charges.Any(x => x.Discount > 0);
//            XtraReport report = new();
//            if (isHaveDiscount)
//            {
//                report = new DiscountInvoiceReport(data, PrintedCurrencyID);
//            }
//            else
//            {
//                report = new InvoiceReport(data, PrintedCurrencyID);

//            }


//            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Reports");

//            string sanitizedCustomer = SanitizeFileName(data.InvoiceTo.CustomerName);
//            string sanitizedService = SanitizeFileName(data.ShipmentDetails.ServiceDetailCode);
//            string sanitizedOrigin = SanitizeFileName(data.ShipmentDetails.Origin);
//            string sanitizedDestination = SanitizeFileName(data.ShipmentDetails.Destination);

//            string fileName = $"{obj.InvoiceID}-{sanitizedCustomer}-{sanitizedService}-{sanitizedOrigin}-{sanitizedDestination}.pdf";
//            string filePath = Path.Combine(folderPath, fileName);



//            if (!Directory.Exists(folderPath))
//            {
//                Directory.CreateDirectory(folderPath);
//            }


//            //obj.LeadStatusId = 3;
//            var LeadStatusID = _leadStatus.FindRow(a => a.LeadStatusNameEn.Contains("Invoice Issued")).GetAwaiter().GetResult().Id;
//            obj.LeadStatusId = LeadStatusID;
//            await _log_OrderLeadesRepository.UpdateWithTransaction(obj);

//            fileUrl = $"{url}/Reports/{obj.InvoiceID}-{sanitizedCustomer}-{sanitizedService}-{sanitizedOrigin}-{sanitizedDestination}.pdf";
//            report.ExportToPdf(filePath);

//            return Ok(new InvoiceResponse
//            {
//                Link = fileUrl,
//                IsComplete = true,
//                MissingData = []
//            });
//        }
//        catch (Exception ex)
//        {
//            await _loggerHelper.AddLog(HttpContext);
//            return BadRequest("LblError400 : " + ex);
//        }
//    }


//    #region DevExpress - Reporting
//    [HttpPost]
//    [ProducesDefaultResponseType(typeof(List<OrderProfitResultDTO>))]
//    [SwaggerOperation(
//    Summary = "This API is for return the Profit Report",
//    Description = "<h1>This API is for return the Profit Report</h1>")]
//    public async Task<ActionResult> ProfitReport(OrderProfitDTO orderProfitDTO)
//    {
//        try
//        {
//            if (orderProfitDTO is null)
//                return NoContent();

//            orderProfitDTO.Currencies = await this.clinetAPIHelper.GetCurrencies();
//            List<OrderProfitResultDTO> leadDetails = await _reportRepository.GetOrderLeadProfitReports(orderProfitDTO);

//            if (leadDetails is null || !leadDetails.Any())
//                return BadRequest("No data found to export");
//            return Ok(leadDetails);
//        }
//        catch (Exception ex)
//        {
//            await _loggerHelper.AddLog(HttpContext, new { orderProfitDTO, ErrorMessage = ex.Message });
//            return BadRequest("Something went wrong while generating the Excel file." + ex);
//        }
//    }


//    #endregion








//    private async Task<List<BankWithDetailsDTO>> GetBankWithDetails(int CompanyId, int BranchID)
//    {
//        try
//        {
//            var Url = _configuration.GetSection("APIs:Accounting:AccAPI:GetBank_WithDetails").Value;
//            var CoreToken = HttpContext.Items["Token"] as string;
//            string Response = await _IClient.Get(new GetRequestViewModel
//            {
//                BaseUrl = AccBaseUrl,
//                Url = Url + "?CompanyId=" + CompanyId + "&BranchId=" + BranchID,
//            });

//            var res = JsonConvert.DeserializeObject<APIReponse<BankWithDetailsDTO>>(Response);
//            return res.ResponseDetails;
//        }
//        catch (Exception)
//        {

//            return new List<BankWithDetailsDTO>();
//        }
//    }
//    private async Task<CompanyInfo> GetCompanyInfo()
//    {
//        try
//        {
//            var Url = _configuration.GetSection("APIs:ERP:ERPAPI:GetCompanyInfo").Value;
//            string responseCountry = await _IClient.Get(new GetRequestViewModel
//            {
//                BaseUrl = ERPBaseUrl,
//                Url = Url + "?CompanyId=" + this.CompanyID,
//            });

//            var info = JsonConvert.DeserializeObject<APIReponseSingle<CompanyInfo>>(responseCountry);
//            if (info == null) return new CompanyInfo();

//            return info.ResponseDetails;
//        }
//        catch (Exception)
//        {
//            await _loggerHelper.AddLog(HttpContext);
//            return new CompanyInfo();
//        }
//    }
//    private async Task<List<CountryDTO>> GetAllCountriesWithCities()
//    {
//        try
//        {
//            var UrlCountry = _configuration.GetSection("APIs:ERP:ERPAPI:GetCountry").Value;
//            var UrlCity = _configuration.GetSection("APIs:ERP:ERPAPI:CountryArea").Value;
//            //var CoreToken = HttpContext.Items["Token"] as string;
//            //CoreToken = CoreToken.Replace("", "");

//            string responseCountry = await _IClient.Get(new GetRequestViewModel
//            {
//                BaseUrl = ERPBaseUrl,
//                Url = UrlCountry + "?lang=en",
//                //Headers = new Dictionary<string, string>
//                //    {
//                //        { "Authorization", CoreToken }
//                //    }
//            });

//            var countries = JsonConvert.DeserializeObject<APIReponse<CountryDTO>>(responseCountry);
//            if (countries == null || !countries.ResponseDetails.Any()) return new List<CountryDTO>();


//            string responseCity = await _IClient.Get(new GetRequestViewModel
//            {
//                BaseUrl = ERPBaseUrl,
//                Url = UrlCity + "?CountryID=",
//                //Headers = new Dictionary<string, string>
//                //    {
//                //        { "Authorization", CoreToken }
//                //    }
//            });

//            var cities = JsonConvert.DeserializeObject<APIReponse<AreaDTO>>(responseCity);
//            if (cities.ResponseDetails == null) cities.ResponseDetails = new List<AreaDTO>();

//            foreach (var country in countries.ResponseDetails)
//            {
//                country.Area = cities.ResponseDetails.Where(city => city.CountryId == country.Id).ToList();
//            }

//            return countries.ResponseDetails;
//        }
//        catch (Exception)
//        {
//            await _loggerHelper.AddLog(HttpContext);
//            return new List<CountryDTO>();
//        }
//    }
//    private async Task<List<CurrencyTable>> GetCurrencies()
//    {
//        try
//        {
//            var Url = _configuration.GetSection("APIs:ERP:ERPAPI:Currencies").Value;
//            string responseCountry = await _IClient.Get(new GetRequestViewModel
//            {
//                BaseUrl = ERPBaseUrl,
//                Url = Url + "?CompanyId=" + this.CompanyID + "&BranchId=" + this.BranchID,
//            });

//            var countries = JsonConvert.DeserializeObject<APIReponse<CurrencyTable>>(responseCountry);
//            if (countries == null || !countries.ResponseDetails.Any()) return new List<CurrencyTable>();

//            return countries.ResponseDetails;
//        }
//        catch (Exception)
//        {
//            await _loggerHelper.AddLog(HttpContext);
//            return new List<CurrencyTable>();
//        }
//    }
//    private async Task<List<ItemUnit>> Get_ItemUnits()
//    {
//        try
//        {
//            var Url = _configuration.GetSection("APIs:Accounting:AccAPI:Get_ItemUnits").Value;
//            string responseunit = await _IClient.Get(new GetRequestViewModel
//            {
//                BaseUrl = AccBaseUrl,
//                Url = Url + "?ItemId=" + null + "&CompanyId=" + this.CompanyID + "&BranchId=" + this.BranchID + "&Language=en",
//            });

//            var UnitS = JsonConvert.DeserializeObject<APIReponse<ItemUnit>>(responseunit);
//            if (UnitS == null || !UnitS.ResponseDetails.Any()) return new List<ItemUnit>();

//            return UnitS.ResponseDetails;
//        }
//        catch (Exception)
//        {
//            await _loggerHelper.AddLog(HttpContext);
//            return new List<ItemUnit>();
//        }
//    }
//    private async Task<List<Item>> Get_Items()
//    {
//        try
//        {
//            var Url = _configuration.GetSection("APIs:Accounting:AccAPI:Get_Items").Value;
//            string responseItems = await _IClient.Get(new GetRequestViewModel
//            {
//                BaseUrl = AccBaseUrl,
//                //Url = Url + "?ItemId=" + null + "&CompanyId=" + this.CompanyID + "&BranchId=" + this.BranchID + "&Language=en",
//                Url = Url + "?CompanyId=" + this.CompanyID + "&BranchId=" + this.BranchID + "&Language=en",
//            });

//            var UnitS = JsonConvert.DeserializeObject<APIReponse<Item>>(responseItems);
//            if (UnitS == null || !UnitS.ResponseDetails.Any()) return new List<Item>();

//            return UnitS.ResponseDetails;
//        }
//        catch (Exception)
//        {
//            await _loggerHelper.AddLog(HttpContext);
//            return new List<Item>();
//        }
//    }


//    public static string SanitizeFileName(string input)
//    {
//        if (string.IsNullOrWhiteSpace(input))
//            return string.Empty;

//        // Remove all whitespaces (space, tab, newline, etc.)
//        string noWhiteSpace = string.Concat(input.Where(c => !char.IsWhiteSpace(c)));

//        // Remove all slashes (both single / and double //)
//        string cleaned = noWhiteSpace.Replace("/", string.Empty);

//        return cleaned;
//    }
//}



