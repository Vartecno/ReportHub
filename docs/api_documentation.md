# ReportHub API Documentation

## Overview

ReportHub is a centralized reporting system that supports 50+ report types with dynamic branding, logos, and icons. The system provides both modern template-based APIs and backward-compatible legacy endpoints.

## Base URL

```
http://localhost:5000/api
https://your-domain.com/api
```

## Authentication

Currently, the API does not require authentication. In production, you should implement appropriate authentication mechanisms.

## Template-Based Reporting API

### Generate Template Report

Generate a report using the template engine with dynamic branding.

**Endpoint:** `POST /templatereports/generate`

**Content-Type:** `application/json`

**Request Body:**
```json
{
  "reportType": "venues_invoice",
  "branding": {
    "company": {
      "name": "Company Name",
      "address": "Company Address",
      "phone": "+1234567890",
      "email": "contact@company.com"
    },
    "logo": "data:image/png;base64,...",
    "colors": {
      "primary": "#0066CC",
      "secondary": "#F0F0F0"
    }
  },
  "data": {
    // Template-specific data structure
  },
  "configuration": {
    "title": "Report Title",
    "includePageNumbers": true
  }
}
```

**Response:**
- **200 OK**: Returns PDF file
- **400 Bad Request**: Validation errors
- **500 Internal Server Error**: Server error

**Response Headers:**
```
Content-Type: application/pdf
Content-Disposition: attachment; filename="Report_20250115_103000.pdf"
```

### Get Available Templates

Retrieve all available report templates.

**Endpoint:** `GET /templatereports/templates`

**Response:**
```json
{
  "templates": [
    {
      "templateId": "venues_invoice",
      "displayName": "Venues Invoice",
      "description": "Professional invoice template for logistics",
      "version": "1.0.0",
      "category": "Invoices",
      "tags": ["billing", "invoice", "logistics"],
      "isActive": true
    }
  ],
  "count": 2
}
```

### Get Template Sample Data

Get sample data structure for a specific template.

**Endpoint:** `GET /templatereports/templates/{templateId}/sample`

**Parameters:**
- `templateId` (path): Template identifier (e.g., "venues_invoice")

**Response:**
```json
{
  "templateId": "venues_invoice",
  "sampleData": {
    "header": {
      "invoiceNumber": "INV-2025-001",
      "date": "2025-01-15T00:00:00Z"
    }
    // ... rest of sample data
  }
}
```

### Get Template Configuration Schema

Get configuration schema for a specific template.

**Endpoint:** `GET /templatereports/templates/{templateId}/schema`

**Response:**
```json
{
  "templateId": "venues_invoice",
  "requiredFields": {
    "header": {
      "name": "Header",
      "type": "InvoiceHeaderDTO",
      "description": "Invoice header information",
      "isRequired": true
    }
  },
  "optionalFields": {
    "shipment": {
      "name": "Shipment",
      "type": "ShipmentDetailsDTO",
      "isRequired": false
    }
  },
  "supportedCurrencies": ["JOD", "USD", "EUR"],
  "supportedLanguages": ["en-US", "ar-JO"]
}
```

### Validate Template Request

Validate a template request without generating the report.

**Endpoint:** `POST /templatereports/validate`

**Request Body:** Same as generate request

**Response:**
```json
{
  "isValid": true,
  "errors": [],
  "warnings": ["No charges found in invoice"]
}
```

### Template-Specific Endpoints

#### Generate Venues Invoice

**Endpoint:** `POST /templatereports/venues-invoice`

#### Generate Account Statement

**Endpoint:** `POST /templatereports/account-statement`

## Legacy Reporting API (Backward Compatibility)

### Generate Report

**Endpoint:** `POST /reports/generate`

**Request Body:**
```json
{
  "format": "PDF",
  "settings": {
    "title": "Report Title",
    "author": "System",
    "formatting": {
      "fontSize": 12,
      "fontFamily": "Arial"
    }
  },
  "data": {
    "sections": [],
    "tables": [],
    "variables": {}
  }
}
```

### Generate PDF Report

**Endpoint:** `POST /reports/generate-pdf`

### Get Supported Formats

**Endpoint:** `GET /reports/supported-formats`

**Response:**
```json
[
  {
    "format": "PDF",
    "contentType": "application/pdf"
  },
  {
    "format": "Word",
    "contentType": "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
  }
]
```

## Data Structures

### ReportBrandingDTO

```json
{
  "company": {
    "name": "string (required)",
    "address": "string",
    "phone": "string",
    "email": "string",
    "city": "string",
    "country": "string"
  },
  "logo": "string (base64 or URL)",
  "icons": {
    "header_icon": "string",
    "footer_icon": "string"
  },
  "colors": {
    "primary": "string (#RRGGBB)",
    "secondary": "string (#RRGGBB)",
    "text": "string (#RRGGBB)"
  },
  "typography": {
    "fontFamily": "string",
    "headerSize": "integer",
    "bodySize": "integer",
    "smallSize": "integer"
  }
}
```

### VenuesInvoiceDataDTO

```json
{
  "header": {
    "invoiceNumber": "string (required)",
    "date": "datetime (required)",
    "reference": "string",
    "customerName": "string",
    "customerAddress": "string"
  },
  "shipment": {
    "service": "string",
    "origin": "string",
    "destination": "string",
    "awb": "string",
    "mbl": "string",
    "hbl": "string",
    "weight": "decimal",
    "volume": "string"
  },
  "charges": [
    {
      "sequence": "integer",
      "description": "string (required)",
      "ratePerUnit": "decimal",
      "quantity": "decimal",
      "price": "decimal",
      "value": "decimal",
      "currency": "string"
    }
  ],
  "total": {
    "subtotal": "decimal",
    "tax": "decimal",
    "total": "decimal (required)",
    "currency": "string"
  },
  "bankAccounts": [
    {
      "bankName": "string",
      "accountName": "string",
      "accountNumber": "string",
      "iban": "string",
      "swift": "string",
      "currency": "string"
    }
  ]
}
```

### AccountStatementDataDTO

```json
{
  "header": {
    "accountNumber": "string (required)",
    "accountName": "string",
    "fromDate": "datetime (required)",
    "toDate": "datetime (required)",
    "currency": "string"
  },
  "transactions": [
    {
      "date": "datetime (required)",
      "reference": "string",
      "description": "string",
      "debit": "decimal",
      "credit": "decimal",
      "balance": "decimal (required)",
      "currency": "string",
      "additionalFields": {
        "accountNo": "string",
        "accountName": "string",
        "supplierInvoiceNo": "string",
        "invoiceNo": "string"
      }
    }
  ],
  "summary": {
    "openingBalance": "decimal",
    "totalDebits": "decimal",
    "totalCredits": "decimal",
    "closingBalance": "decimal",
    "currency": "string"
  }
}
```

## Error Responses

### Validation Error (400)

```json
{
  "errors": [
    "ReportType is required",
    "Company name is required"
  ],
  "warnings": [
    "No charges found in invoice"
  ]
}
```

### Server Error (500)

```json
{
  "error": "An internal error occurred while generating the report"
}
```

## Logo and Icon Integration

### Base64 Format

```json
{
  "logo": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=="
}
```

### URL Format

```json
{
  "logo": "https://example.com/logo.png"
}
```

### Multiple Icons

```json
{
  "icons": {
    "header_icon": "data:image/png;base64,...",
    "footer_icon": "data:image/svg+xml;base64,...",
    "watermark": "https://example.com/watermark.png"
  }
}
```

## Rate Limiting

No rate limiting is currently implemented. Consider implementing rate limiting for production use.

## CORS

CORS is enabled for all origins in development. Configure appropriately for production.

## Testing

### Using cURL

```bash
# Get available templates
curl -X GET "http://localhost:5000/api/templatereports/templates"

# Generate invoice
curl -X POST "http://localhost:5000/api/templatereports/generate" \
  -H "Content-Type: application/json" \
  -d @sample_invoice_request.json \
  --output invoice.pdf
```

### Using Postman

1. Import the API collection (if available)
2. Set the base URL to your server
3. Use the sample requests provided in the documentation
4. Ensure Content-Type is set to application/json for POST requests

## Migration Guide

### From Legacy to Template-Based API

1. **Identify Current Report Type**: Determine which template matches your current report
2. **Update Request Structure**: Convert your data to the new template format
3. **Add Branding**: Include company information and logos
4. **Test Thoroughly**: Validate output matches expectations
5. **Update Integration**: Switch to new endpoints

### Backward Compatibility

The legacy endpoints remain functional and will automatically use the template engine for PDF generation, ensuring seamless migration.

## Support

For technical support or questions about the API, please contact the development team or refer to the additional documentation in the `/docs` folder.