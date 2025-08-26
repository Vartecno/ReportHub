using ReportHub.Objects.DTOs;
using ReportHub.Objects.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;

namespace ReportHub.DBCommon;

public class Base : ControllerBase
{

    public Base()
    {

        HttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        if (httpContextAccessor.HttpContext is not null)
        {
            ClaimsPrincipal principal = httpContextAccessor.HttpContext.User;

            if (principal is not null)
            {
                if (!string.IsNullOrEmpty(principal.FindFirst("UserID")?.Value))
                {
                    //za
                }
            }
        }
    }

    public int UserID
    {
        get
        {
            int UserID = (int)HttpContext.Items["UserID"];
            if (UserID == 0)
                return 0;
            return UserID;

        }
    }
    public int CompanyID
    {
        get
        {
            int CompanyID = (int)HttpContext.Items["CompanyID"];
            if (CompanyID == 0)
                return 0;
            return CompanyID;

        }
    }
    public int BranchID
    {
        get
        {
            int BranchID = (int)HttpContext.Items["BranchID"];
            if (BranchID == 0)
                return 0;
            return BranchID;

        }
    }





    #region Invoice
    [NonAction]
    public decimal GetTotalInvoice(decimal UnitPrice, decimal QTY, decimal ExchangeRate)
    {
        //return (UnitPrice * QTY) * ExchangeRate;
        return (UnitPrice * ExchangeRate) * QTY;
    }
    [NonAction]
    public decimal GetTotalInvoice(decimal UnitPrice, decimal ExchangeRate)
    {
        return (UnitPrice * ExchangeRate);
    }
    #endregion


}
