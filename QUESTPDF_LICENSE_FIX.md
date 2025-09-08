# QuestPDF License Configuration Fix

**Issue**: QuestPDF requires explicit license configuration before use
**Resolution**: Added Community license configuration to prevent licensing errors

## Files Modified

### 1. TemplateEngine.cs
**Location**: `/ReportHub.Common/Services/TemplateEngine.cs`

**Change**: Added QuestPDF license configuration in constructor
```csharp
public TemplateEngine(ILogger<TemplateEngine>? logger = null)
{
    // Configure QuestPDF license for Community use
    QuestPDF.Settings.License = LicenseType.Community;
    _logger = logger;
}
```

### 2. BaseReportTemplate.cs  
**Location**: `/ReportHub.Objects/Templates/BaseReportTemplate.cs`

**Change**: Added static constructor for license configuration
```csharp
static BaseReportTemplate()
{
    // Configure QuestPDF license for Community use
    QuestPDF.Settings.License = LicenseType.Community;
}
```

## Why This Fix Works

1. **TemplateEngine**: Main PDF generation service now sets license before any operations
2. **BaseReportTemplate**: Static constructor ensures license is set when any template is used
3. **Early Initialization**: License is configured before QuestPDF methods are called
4. **Community License**: Free license for organizations under $1M annual revenue

## Testing Recommendation

Try your invoice API request again:

```bash
curl -X POST "http://localhost:5000/api/templatereports/venues-invoice" \
  -H "Content-Type: application/json" \
  -d @your_invoice_request.json \
  --output invoice.pdf
```

The QuestPDF licensing error should be resolved and PDF generation should work properly.

## Notes

- **Community License**: Suitable for most users (organizations under $1M annual revenue)
- **Production Ready**: This configuration is appropriate for production use
- **No Performance Impact**: License setting is a one-time operation
- **Compliance**: Follows QuestPDF's fair use licensing model

---

**Fix Applied**: January 15, 2025  
**Status**: Ready for Testing