# 🚀 ReportHub API Requests - Invoice & Account Statement

**Base URL**: `http://localhost:5000/api`  
**Content-Type**: `application/json`  
**Authentication**: JWT Bearer Token (if required)

---

## 📋 Table of Contents

1. [Template-Based API Endpoints](#template-based-api-endpoints)
2. [Invoice API Requests](#invoice-api-requests) 
3. [Account Statement API Requests](#account-statement-api-requests)
4. [Legacy API Endpoints](#legacy-api-endpoints)
5. [Response Formats](#response-formats)
6. [Error Handling](#error-handling)

---

## 🎯 Template-Based API Endpoints

### 1. Generate Template Report (Universal)
```http
POST /api/templatereports/generate
Content-Type: application/json
```

### 2. Get Available Templates
```http
GET /api/templatereports/templates
```

### 3. Generate Venues Invoice (Direct)
```http
POST /api/templatereports/venues-invoice
Content-Type: application/json
```

### 4. Generate Account Statement (Direct)
```http
POST /api/templatereports/account-statement
Content-Type: application/json
```

### 5. Validate Template Request
```http
POST /api/templatereports/validate
Content-Type: application/json
```

---

## 🧾 Invoice API Requests

### 🎯 Complete Venues Invoice Request

```http
POST /api/templatereports/venues-invoice
Content-Type: application/json
```

**Request Body:**
```json
{
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
      "text": "#333333"
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
      }
    ]
  },
  "configuration": {
    "title": "Invoice",
    "includePageNumbers": true,
    "includeHeader": true,
    "includeFooter": true
  }
}
```

### 🎯 Universal Template Invoice Request

```http
POST /api/templatereports/generate
Content-Type: application/json
```

**Request Body:**
```json
{
  "reportType": "venues_invoice",
  "branding": {
    "company": {
      "name": "Your Company Name",
      "address": "Your Address",
      "phone": "+1234567890",
      "email": "contact@company.com"
    },
    "logo": "data:image/png;base64,YOUR_LOGO_BASE64",
    "colors": {
      "primary": "#0066CC",
      "secondary": "#F0F0F0"
    }
  },
  "data": {
    "header": {
      "invoiceNumber": "INV-2025-001",
      "date": "2025-01-15T00:00:00Z",
      "customerName": "Customer Name"
    },
    "charges": [
      {
        "sequence": 1,
        "description": "Service Description",
        "ratePerUnit": 100.00,
        "quantity": 1,
        "value": 100.00,
        "currency": "USD"
      }
    ],
    "total": {
      "total": 100.00,
      "currency": "USD"
    }
  }
}
```

---

## 📊 Account Statement API Requests

### 🎯 Complete Account Statement Request

```http
POST /api/templatereports/account-statement
Content-Type: application/json
```

**Request Body:**
```json
{
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

### 🎯 Universal Template Account Statement Request

```http
POST /api/templatereports/generate
Content-Type: application/json
```

**Request Body:**
```json
{
  "reportType": "account_statement",
  "branding": {
    "company": {
      "name": "Your Company Name",
      "address": "Your Address",
      "phone": "+1234567890",
      "email": "contact@company.com"
    },
    "colors": {
      "primary": "#0066CC",
      "secondary": "#F0F0F0"
    }
  },
  "data": {
    "header": {
      "accountNumber": "ACC-001",
      "accountName": "Main Account",
      "fromDate": "2025-01-01T00:00:00Z",
      "toDate": "2025-01-31T23:59:59Z",
      "currency": "USD"
    },
    "transactions": [
      {
        "date": "2025-01-05T00:00:00Z",
        "reference": "TRX001",
        "description": "Transaction Description",
        "debit": 0,
        "credit": 1000.00,
        "balance": 1000.00,
        "currency": "USD"
      }
    ],
    "summary": {
      "openingBalance": 0,
      "totalCredits": 1000.00,
      "closingBalance": 1000.00,
      "currency": "USD"
    }
  }
}
```

---

## 🔄 Legacy API Endpoints (Backward Compatibility)

### Generate PDF Report
```http
POST /api/reports/generate-pdf
Content-Type: application/json
```

### Generate Report with Format
```http
POST /api/reports/generate
Content-Type: application/json
```

### Get Supported Formats
```http
GET /api/reports/supported-formats
```

---

## 📄 Response Formats

### Success Response (PDF File)
```http
HTTP/1.1 200 OK
Content-Type: application/pdf
Content-Disposition: attachment; filename="Invoice_20250115_103000.pdf"
Content-Length: [file-size]

[PDF binary data]
```

### Templates List Response
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
    },
    {
      "templateId": "account_statement",
      "displayName": "Account Statement",
      "description": "Professional account statement template",
      "version": "1.0.0",
      "category": "Financial Statements",
      "tags": ["statement", "accounting", "financial"],
      "isActive": true
    }
  ],
  "count": 2
}
```

### Validation Response
```json
{
  "isValid": true,
  "errors": [],
  "warnings": ["No charges found in invoice"]
}
```

---

## ❌ Error Handling

### Validation Error (400 Bad Request)
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

### Server Error (500 Internal Server Error)
```json
{
  "error": "An internal error occurred while generating the report"
}
```

### Not Found Error (404 Not Found)
```json
{
  "error": "Template 'invalid_template' not found"
}
```

---

## 🔑 Authentication (If Required)

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## 🧪 Testing with cURL

### Generate Invoice
```bash
curl -X POST "http://localhost:5000/api/templatereports/venues-invoice" \
  -H "Content-Type: application/json" \
  -d @invoice_request.json \
  --output invoice.pdf
```

### Generate Account Statement
```bash
curl -X POST "http://localhost:5000/api/templatereports/account-statement" \
  -H "Content-Type: application/json" \
  -d @statement_request.json \
  --output statement.pdf
```

### Get Available Templates
```bash
curl -X GET "http://localhost:5000/api/templatereports/templates"
```

---

## 📝 Notes

- All dates should be in ISO 8601 format (`YYYY-MM-DDTHH:mm:ssZ`)
- Logo can be provided as base64 string or URL
- Currency codes should follow ISO 4217 standard (JOD, USD, EUR)
- Numeric values should be provided as decimal numbers
- Response files are generated in PDF format by default

---

**Last Updated**: January 15, 2025  
**API Version**: v1.0  
**Framework**: .NET Core 8.0