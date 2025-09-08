# Template Creation Guide

This guide explains how to create new report templates for the ReportHub centralized reporting system.

## Table of Contents
- [Template Architecture](#template-architecture)
- [Creating a New Template](#creating-a-new-template)
- [Template Registration](#template-registration)
- [Data Structure Design](#data-structure-design)
- [Validation Implementation](#validation-implementation)
- [Best Practices](#best-practices)

## Template Architecture

The template system is built on the following key components:

- **IReportTemplate**: Base interface for all templates
- **BaseReportTemplate**: Abstract base class with common functionality
- **TemplateEngine**: Manages and executes templates
- **Template-specific DTOs**: Data structures for each template type

## Creating a New Template

### Step 1: Define Data Structure

First, create DTOs for your template data in `ReportHub.Objects/DTOs/TemplateReportRequestDTO.cs`:

```csharp
/// <summary>
/// Data structure for Purchase Order template
/// </summary>
public class PurchaseOrderDataDTO
{
    public PurchaseOrderHeaderDTO Header { get; set; } = new();
    public List<PurchaseOrderItemDTO> Items { get; set; } = new();
    public PurchaseOrderTotalDTO Total { get; set; } = new();
    public VendorInfoDTO Vendor { get; set; } = new();
}

public class PurchaseOrderHeaderDTO
{
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime RequiredDate { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    // Add other fields as needed
}

// Define other DTOs...
```

### Step 2: Create Template Class

Create a new template class inheriting from `BaseReportTemplate`:

```csharp
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.Interfaces;
using ReportHub.Objects.Templates;
using Colors = QuestPDF.Helpers.Colors;

namespace ReportHub.Objects.Templates
{
    /// <summary>
    /// Template for Purchase Order reports
    /// </summary>
    public class PurchaseOrderTemplate : BaseReportTemplate
    {
        public override string TemplateId => "purchase_order";
        public override string DisplayName => "Purchase Order";
        public override string Description => "Professional purchase order template for procurement";
        public override Type[] SupportedDataTypes => new[] { typeof(PurchaseOrderDataDTO) };

        public override IContainer GenerateContent(TemplateReportRequestDTO request)
        {
            var orderData = ParseTemplateData<PurchaseOrderDataDTO>(request.Data);
            if (orderData == null)
                throw new ArgumentException("Invalid data format for PurchaseOrderTemplate");

            return container =>
            {
                container.Column(column =>
                {
                    // Header with company info and logo
                    column.Item().Element(headerContainer => 
                        RenderCompanyHeader(headerContainer, request.Branding));

                    // Purchase Order Title
                    column.Item().PaddingBottom(15).AlignCenter()
                        .Text("Purchase Order")
                        .FontSize(18).SemiBold().FontColor(request.Branding.Colors.Primary);

                    // Order details
                    column.Item().PaddingBottom(15).Element(detailsContainer => 
                        RenderOrderDetails(detailsContainer, orderData.Header, request.Branding));

                    // Vendor information
                    column.Item().PaddingBottom(15).Element(vendorContainer => 
                        RenderVendorInfo(vendorContainer, orderData.Vendor, request.Branding));

                    // Items table
                    if (orderData.Items?.Any() == true)
                    {
                        column.Item().PaddingBottom(15).Element(itemsContainer => 
                            RenderItemsTable(itemsContainer, orderData.Items, request.Branding));
                    }

                    // Total
                    if (orderData.Total != null)
                    {
                        column.Item().Element(totalContainer => 
                            RenderTotal(totalContainer, orderData.Total, request.Branding));
                    }
                });
            };
        }

        private void RenderOrderDetails(IContainer container, PurchaseOrderHeaderDTO header, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().Text($"Order Number: {header.OrderNumber}")
                    .FontSize(branding.Typography.BodySize);
                column.Item().Text($"Order Date: {FormatDate(header.OrderDate)}")
                    .FontSize(branding.Typography.BodySize);
                column.Item().Text($"Required Date: {FormatDate(header.RequiredDate)}")
                    .FontSize(branding.Typography.BodySize);
                column.Item().Text($"Buyer: {header.BuyerName}")
                    .FontSize(branding.Typography.BodySize);
            });
        }

        private void RenderVendorInfo(IContainer container, VendorInfoDTO vendor, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(5).Text("Vendor Information")
                    .FontSize(12).SemiBold();
                
                // Implement vendor details rendering
            });
        }

        private void RenderItemsTable(IContainer container, List<PurchaseOrderItemDTO> items, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(5).Text("Items")
                    .FontSize(12).SemiBold();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // Item No
                        columns.RelativeColumn(3);    // Description
                        columns.ConstantColumn(80);   // Quantity
                        columns.ConstantColumn(100);  // Unit Price
                        columns.ConstantColumn(100);  // Total
                    });

                    // Header
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("#").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Description").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Qty").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Unit Price").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Total").FontSize(10).SemiBold().FontColor(Colors.White);

                    // Data rows
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        var isEvenRow = i % 2 == 0;
                        var bgColor = isEvenRow ? Colors.White : Colors.Grey.Lighten4;

                        table.Cell().Background(bgColor).Padding(5)
                            .Text((i + 1).ToString()).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(item.Description).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(item.Quantity.ToString()).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(FormatCurrency(item.UnitPrice, item.Currency)).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(FormatCurrency(item.Total, item.Currency)).FontSize(10);
                    }
                });
            });
        }

        private void RenderTotal(IContainer container, PurchaseOrderTotalDTO total, ReportBrandingDTO branding)
        {
            container.AlignRight().Text($"Total Amount: {FormatCurrency(total.Total, total.Currency)}")
                .FontSize(12).SemiBold().FontColor(branding.Colors.Primary);
        }

        public override TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request)
        {
            var result = base.ValidateRequest(request);
            
            if (result.IsValid)
            {
                var orderData = ParseTemplateData<PurchaseOrderDataDTO>(request.Data);
                if (orderData == null)
                {
                    result.Errors.Add("Data must be of type PurchaseOrderDataDTO");
                }
                else
                {
                    if (orderData.Header == null)
                        result.Errors.Add("Order header is required");
                    if (string.IsNullOrWhiteSpace(orderData.Header?.OrderNumber))
                        result.Errors.Add("Order number is required");
                    if (orderData.Items == null || !orderData.Items.Any())
                        result.Warnings.Add("No items found in purchase order");
                }
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }

        public override object GetSampleData()
        {
            return new PurchaseOrderDataDTO
            {
                Header = new PurchaseOrderHeaderDTO
                {
                    OrderNumber = "PO-2025-001",
                    OrderDate = DateTime.Now,
                    RequiredDate = DateTime.Now.AddDays(30),
                    BuyerName = "John Smith"
                },
                Items = new List<PurchaseOrderItemDTO>
                {
                    new PurchaseOrderItemDTO
                    {
                        Description = "Office Supplies",
                        Quantity = 10,
                        UnitPrice = 25.00m,
                        Total = 250.00m,
                        Currency = "JOD"
                    }
                },
                Total = new PurchaseOrderTotalDTO
                {
                    Subtotal = 250.00m,
                    Tax = 25.00m,
                    Total = 275.00m,
                    Currency = "JOD"
                }
            };
        }

        public override TemplateConfigSchema GetConfigSchema()
        {
            return new TemplateConfigSchema
            {
                TemplateId = TemplateId,
                RequiredFields = new Dictionary<string, FieldSchema>
                {
                    { "Header", new FieldSchema { Name = "Header", Type = "PurchaseOrderHeaderDTO", Description = "Order header information", IsRequired = true } },
                    { "Items", new FieldSchema { Name = "Items", Type = "List<PurchaseOrderItemDTO>", Description = "List of order items", IsRequired = true } }
                },
                OptionalFields = new Dictionary<string, FieldSchema>
                {
                    { "Total", new FieldSchema { Name = "Total", Type = "PurchaseOrderTotalDTO", Description = "Order totals", IsRequired = false } },
                    { "Vendor", new FieldSchema { Name = "Vendor", Type = "VendorInfoDTO", Description = "Vendor information", IsRequired = false } }
                },
                SupportedCurrencies = new List<string> { "JOD", "USD", "EUR" },
                SupportedLanguages = new List<string> { "en-US", "ar-JO" }
            };
        }
    }
}
```

### Step 3: Register Template

Add your template to the dependency injection in `TemplateEngineServiceExtensions.cs`:

```csharp
public static IServiceCollection AddReportTemplates(this IServiceCollection services)
{
    services.AddSingleton<IReportTemplate, VenuesInvoiceTemplate>();
    services.AddSingleton<IReportTemplate, AccountStatementTemplate>();
    services.AddSingleton<IReportTemplate, PurchaseOrderTemplate>(); // Add your template
    // Add more templates here as needed

    return services;
}
```

## Data Structure Design

### Design Principles

1. **Separation of Concerns**: Separate header, items, totals, etc.
2. **Extensibility**: Use `Dictionary<string, object>` for custom fields
3. **Type Safety**: Use strongly-typed properties for common fields
4. **Consistency**: Follow naming conventions across templates

### Example Structure

```csharp
public class MyTemplateDataDTO
{
    // Core sections
    public MyTemplateHeaderDTO Header { get; set; } = new();
    public List<MyTemplateItemDTO> Items { get; set; } = new();
    public MyTemplateTotalDTO Total { get; set; } = new();
    
    // Extensibility
    public Dictionary<string, object> AdditionalFields { get; set; } = new();
}
```

## Validation Implementation

### Template-Specific Validation

```csharp
public override TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request)
{
    // Start with base validation
    var result = base.ValidateRequest(request);
    
    if (result.IsValid)
    {
        // Parse and validate template-specific data
        var data = ParseTemplateData<MyTemplateDataDTO>(request.Data);
        if (data == null)
        {
            result.Errors.Add("Invalid data format");
        }
        else
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(data.Header?.RequiredField))
                result.Errors.Add("Required field is missing");
            
            // Add warnings for optional but recommended fields
            if (data.Items?.Count == 0)
                result.Warnings.Add("No items provided");
        }
    }
    
    result.IsValid = !result.Errors.Any();
    return result;
}
```

## Best Practices

### Layout Design

1. **Consistent Spacing**: Use consistent padding and margins
2. **Brand Integration**: Always use brand colors and fonts
3. **Responsive Tables**: Handle varying data sizes gracefully
4. **Logo Placement**: Follow brand guidelines for logo positioning

### Code Organization

1. **Helper Methods**: Break rendering into logical helper methods
2. **Reusable Components**: Use base class methods for common elements
3. **Error Handling**: Provide graceful fallbacks for missing data
4. **Documentation**: Document template-specific requirements

### Performance

1. **Lazy Loading**: Only process data that will be rendered
2. **Efficient Parsing**: Cache parsed data where possible
3. **Memory Management**: Dispose of resources properly

### Testing

```csharp
[Test]
public void ValidateRequest_WithValidData_ReturnsSuccess()
{
    // Arrange
    var template = new PurchaseOrderTemplate();
    var request = CreateValidRequest();
    
    // Act
    var result = template.ValidateRequest(request);
    
    // Assert
    Assert.IsTrue(result.IsValid);
    Assert.IsEmpty(result.Errors);
}

[Test]
public void GenerateContent_WithSampleData_ProducesValidOutput()
{
    // Arrange
    var template = new PurchaseOrderTemplate();
    var request = CreateRequestWithSampleData();
    
    // Act & Assert
    Assert.DoesNotThrow(() => template.GenerateContent(request));
}
```

## Common Patterns

### Table Rendering

```csharp
protected void RenderDataTable<T>(IContainer container, List<T> data, 
    string title, Action<TableDescriptor, T, int> renderRow)
{
    container.Column(column =>
    {
        column.Item().PaddingBottom(5).Text(title).FontSize(12).SemiBold();
        
        column.Item().Table(table =>
        {
            // Configure columns
            table.ColumnsDefinition(ConfigureColumns);
            
            // Render header
            RenderTableHeader(table);
            
            // Render data rows
            for (int i = 0; i < data.Count; i++)
            {
                renderRow(table, data[i], i);
            }
        });
    });
}
```

### Conditional Rendering

```csharp
if (!string.IsNullOrWhiteSpace(data.OptionalField))
{
    column.Item().Text($"Optional: {data.OptionalField}");
}
```

### Multi-Currency Support

```csharp
protected string FormatCurrencyWithSymbol(decimal amount, string currencyCode)
{
    var symbols = new Dictionary<string, string>
    {
        { "JOD", "JD" },
        { "USD", "$" },
        { "EUR", "€" }
    };
    
    var symbol = symbols.GetValueOrDefault(currencyCode, currencyCode);
    return $"{amount:N3} {symbol}";
}
```

This guide provides the foundation for creating new templates. Each template should be self-contained, well-tested, and follow the established patterns for consistency and maintainability.