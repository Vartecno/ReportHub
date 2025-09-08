# ReportHub Template System - Sample Requests

This document provides sample JSON requests for the new centralized reporting system that supports 50+ report types with dynamic branding.

## Table of Contents
- [Venues Invoice Sample](#venues-invoice-sample)
- [Account Statement Sample](#account-statement-sample)
- [API Endpoints](#api-endpoints)
- [Logo Integration](#logo-integration)

## Venues Invoice Sample

### Basic Venues Invoice Request

```json
{
  "reportType": "venues_invoice",
  "branding": {
    "company": {
      "name": "Nuba Logistics",
      "address": "Amman-Jordan, Mecca Street, Rashed Al-Neimat Building No.9, 3rd Floor, Office No.301",
      "phone": "+96265812954",
      "email": "info@nubalogistic.com",
      "city": "Amman",
      "country": "Jordan"
    },
    "logo": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==",
    "colors": {
      "primary": "#0066CC",
      "secondary": "#F0F0F0",
      "accent": "#FF6600",
      "text": "#333333",
      "background": "#FFFFFF"
    },
    "typography": {
      "fontFamily": "Arial",
      "headerSize": 18,
      "bodySize": 11,
      "smallSize": 9
    }
  },
  "data": {
    "header": {
      "invoiceNumber": "202500306",
      "date": "2025-01-15T00:00:00Z",
      "reference": "NUBA-REF-12345",
      "customerName": "VENUES FOR CHEMICALS",
      "customerAddress": "Amman, Jordan"
    },
    "shipment": {
      "service": "FCL Sea Freight",
      "origin": "Shanghai",
      "destination": "Aqaba",
      "awb": "VENUES202500306",
      "mbl": "MBL789012345",
      "hbl": "HBL345678901",
      "weight": 15000,
      "volume": "28 CBM"
    },
    "charges": [
      {
        "sequence": 1,
        "description": "Ocean Freight",
        "ratePerUnit": 850.000,
        "quantity": 1,
        "price": 850.000,
        "value": 850.000,
        "currency": "JOD"
      },
      {
        "sequence": 2,
        "description": "Documentation Fee",
        "ratePerUnit": 50.000,
        "quantity": 1,
        "price": 50.000,
        "value": 50.000,
        "currency": "JOD"
      },
      {
        "sequence": 3,
        "description": "Port Handling",
        "ratePerUnit": 120.000,
        "quantity": 1,
        "price": 120.000,
        "value": 120.000,
        "currency": "JOD"
      }
    ],
    "total": {
      "subtotal": 1020.000,
      "tax": 0,
      "total": 1020.000,
      "currency": "JOD"
    },
    "bankAccounts": [
      {
        "bankName": "Arab Bank",
        "accountName": "Nuba Logistics",
        "accountNumber": "0123456789",
        "iban": "JO94ARAB0123456789012345",
        "swift": "ARABJOAX100",
        "currency": "JOD"
      },
      {
        "bankName": "Arab Bank",
        "accountName": "Nuba Logistics",
        "accountNumber": "0987654321",
        "iban": "JO94ARAB0987654321098765",
        "swift": "ARABJOAX100",
        "currency": "USD"
      },
      {
        "bankName": "Arab Bank",
        "accountName": "Nuba Logistics",
        "accountNumber": "1122334455",
        "iban": "JO94ARAB1122334455667788",
        "swift": "ARABJOAX100",
        "currency": "EUR"
      }
    ]
  },
  "configuration": {
    "title": "Invoice",
    "generatedDate": "2025-01-15T10:30:00Z",
    "author": "System Generated",
    "includePageNumbers": true,
    "includeHeader": true,
    "includeFooter": true,
    "culture": "en-US"
  }
}
```

## Account Statement Sample

### Basic Account Statement Request

```json
{
  "reportType": "account_statement",
  "branding": {
    "company": {
      "name": "Nuba logistics",
      "address": "Jordan, Mecca Street, Rashed Al-Neimat Building No.9, 3rd Floor, Office",
      "phone": "+96265812954",
      "email": "info@nubalogistic.com",
      "city": "Amman",
      "country": "Jordan"
    },
    "logo": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==",
    "colors": {
      "primary": "#0066CC",
      "secondary": "#F0F0F0",
      "text": "#333333"
    },
    "typography": {
      "fontFamily": "Arial",
      "headerSize": 16,
      "bodySize": 10,
      "smallSize": 8
    }
  },
  "data": {
    "header": {
      "accountNumber": "123456789",
      "accountName": "Main Operating Account",
      "fromDate": "2025-01-01T00:00:00Z",
      "toDate": "2025-01-31T23:59:59Z",
      "currency": "JOD"
    },
    "transactions": [
      {
        "date": "2025-01-05T00:00:00Z",
        "reference": "PAY001",
        "description": "Customer Payment",
        "debit": 0,
        "credit": 1500.000,
        "balance": 1500.000,
        "currency": "JOD",
        "additionalFields": {
          "accountNo": "123456",
          "accountName": "CUSTOMER RECEIVABLES",
          "supplierInvoiceNo": "",
          "invoiceNo": "INV-2025-001"
        }
      },
      {
        "date": "2025-01-10T00:00:00Z",
        "reference": "EXP001",
        "description": "Office Rent",
        "debit": 800.000,
        "credit": 0,
        "balance": 700.000,
        "currency": "JOD",
        "additionalFields": {
          "accountNo": "654321",
          "accountName": "OFFICE EXPENSES",
          "supplierInvoiceNo": "RENT-JAN-2025",
          "invoiceNo": ""
        }
      },
      {
        "date": "2025-01-15T00:00:00Z",
        "reference": "PAY002",
        "description": "Service Revenue",
        "debit": 0,
        "credit": 2200.000,
        "balance": 2900.000,
        "currency": "JOD",
        "additionalFields": {
          "accountNo": "789123",
          "accountName": "SERVICE REVENUE",
          "supplierInvoiceNo": "",
          "invoiceNo": "INV-2025-002"
        }
      }
    ],
    "summary": {
      "openingBalance": 0,
      "totalDebits": 800.000,
      "totalCredits": 3700.000,
      "closingBalance": 2900.000,
      "currency": "JOD"
    }
  },
  "configuration": {
    "title": "Account Statements",
    "generatedDate": "2025-01-15T10:30:00Z",
    "includePageNumbers": true,
    "includeFooter": true,
    "culture": "en-US"
  }
}
```

## API Endpoints

### Template-Based Endpoints

#### Generate Report
```
POST /api/templatereports/generate
Content-Type: application/json
```

#### Get Available Templates
```
GET /api/templatereports/templates
```

#### Get Template Sample Data
```
GET /api/templatereports/templates/{templateId}/sample
```

#### Get Template Configuration Schema
```
GET /api/templatereports/templates/{templateId}/schema
```

#### Validate Template Request
```
POST /api/templatereports/validate
Content-Type: application/json
```

### Backward Compatibility Endpoints

#### Generate Venues Invoice
```
POST /api/templatereports/venues-invoice
Content-Type: application/json
```

#### Generate Account Statement
```
POST /api/templatereports/account-statement
Content-Type: application/json
```

## Logo Integration

### Base64 Image Format
```json
{
  "branding": {
    "logo": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=="
  }
}
```

### URL Image Format
```json
{
  "branding": {
    "logo": "https://example.com/path/to/logo.png"
  }
}
```

### Multiple Icons
```json
{
  "branding": {
    "logo": "data:image/png;base64,...",
    "icons": {
      "header_icon": "data:image/png;base64,...",
      "footer_icon": "data:image/svg+xml;base64,...",
      "watermark": "data:image/png;base64,..."
    }
  }
}
```

## Response Format

### Success Response
The API returns the generated PDF file with appropriate headers:

```
HTTP/1.1 200 OK
Content-Type: application/pdf
Content-Disposition: attachment; filename="Invoice_20250115_103000.pdf"
Content-Length: [file-size]

[PDF binary data]
```

### Error Response
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

## Template Discovery

### Get Available Templates Response
```json
{
  "templates": [
    {
      "templateId": "venues_invoice",
      "displayName": "Venues Invoice",
      "description": "Professional invoice template for logistics and shipping services",
      "version": "1.0.0",
      "category": "Invoices",
      "tags": ["billing", "invoice", "financial", "logistics", "shipping"],
      "isActive": true
    },
    {
      "templateId": "account_statement",
      "displayName": "Account Statement",
      "description": "Professional account statement template for financial transactions",
      "version": "1.0.0",
      "category": "Financial Statements",
      "tags": ["statement", "accounting", "financial"],
      "isActive": true
    }
  ],
  "count": 2
}
```

This template system allows you to easily add new report types by creating new template classes and registering them in the dependency injection container. Each template is completely self-contained and configurable.