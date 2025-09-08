# ReportHub Deployment Guide

This guide provides instructions for deploying the ReportHub centralized reporting system that supports 50+ report types with dynamic branding.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Development Environment](#development-environment)
- [Production Deployment](#production-deployment)
- [Configuration](#configuration)
- [Testing Deployment](#testing-deployment)
- [Monitoring](#monitoring)
- [Troubleshooting](#troubleshooting)

## Prerequisites

### System Requirements
- .NET 9.0 SDK or later
- Operating System: Windows, Linux, or macOS
- Memory: Minimum 2GB RAM (4GB recommended for production)
- Storage: Minimum 500MB free space
- Network: HTTP/HTTPS access for API endpoints

### Dependencies
- QuestPDF (included via NuGet)
- Microsoft.AspNetCore.App
- System.Text.Json

## Development Environment

### Local Development Setup

1. **Clone or Extract the Project**
   ```bash
   cd /path/to/ReportHub
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Solution**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   cd ReportHub
   dotnet run
   ```

5. **Access the Application**
   - API: `http://localhost:5000/api`
   - Swagger UI: `http://localhost:5000`

### Development Configuration

Update `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "ReportHub": "Debug"
    }
  },
  "AllowedHosts": "*",
  "ReportHub": {
    "MaxFileSize": "10MB",
    "TempDirectory": "./temp",
    "LogoMaxSize": "5MB"
  }
}
```

## Production Deployment

### Option 1: Self-Contained Deployment

1. **Publish the Application**
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/win-x64
   # For Linux
   dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish/linux-x64
   ```

2. **Copy Published Files**
   Copy the contents of the `./publish` folder to your target server.

3. **Set Permissions (Linux)**
   ```bash
   chmod +x ReportHub
   ```

4. **Run the Application**
   ```bash
   # Windows
   ./ReportHub.exe
   
   # Linux
   ./ReportHub
   ```

### Option 2: Framework-Dependent Deployment

1. **Publish the Application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Install .NET Runtime on Target Server**
   Ensure .NET 9.0 runtime is installed on the target server.

3. **Run the Application**
   ```bash
   dotnet ReportHub.dll
   ```

### Option 3: Docker Deployment

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ReportHub/ReportHub.csproj", "ReportHub/"]
COPY ["ReportHub.Common/ReportHub.Common.csproj", "ReportHub.Common/"]
COPY ["ReportHub.Objects/ReportHub.Objects.csproj", "ReportHub.Objects/"]
COPY ["ReportHub.Services/ReportHub.Services.csproj", "ReportHub.Services/"]
RUN dotnet restore "ReportHub/ReportHub.csproj"
COPY . .
WORKDIR "/src/ReportHub"
RUN dotnet build "ReportHub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportHub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportHub.dll"]
```

Build and run:

```bash
docker build -t reporthub .
docker run -p 8080:80 reporthub
```

### Option 4: IIS Deployment (Windows)

1. **Install ASP.NET Core Hosting Bundle**
   Download and install from Microsoft's website.

2. **Publish Application**
   ```bash
   dotnet publish -c Release -o ./publish/iis
   ```

3. **Configure IIS**
   - Create a new site in IIS Manager
   - Point to the published folder
   - Set application pool to "No Managed Code"

4. **Configure web.config**
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <system.webServer>
       <handlers>
         <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
       </handlers>
       <aspNetCore processPath="dotnet" arguments=".\ReportHub.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" />
     </system.webServer>
   </configuration>
   ```

## Configuration

### Production Configuration

Update `appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "ReportHub": "Information"
    },
    "Console": {
      "IncludeScopes": false
    }
  },
  "AllowedHosts": "yourdomain.com",
  "Urls": "http://0.0.0.0:5000;https://0.0.0.0:5001",
  "ReportHub": {
    "MaxFileSize": "50MB",
    "TempDirectory": "/tmp/reporthub",
    "LogoMaxSize": "10MB",
    "EnableCaching": true,
    "CacheExpirationMinutes": 60
  }
}
```

### Environment Variables

Set these environment variables for production:

```bash
# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:5000"
export ReportHub__TempDirectory="/tmp/reporthub"

# Windows
set ASPNETCORE_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://0.0.0.0:5000
set ReportHub__TempDirectory=C:\temp\reporthub
```

### Security Configuration

1. **HTTPS Configuration**
   ```json
   {
     "Kestrel": {
       "Certificates": {
         "Default": {
           "Path": "certificate.pfx",
           "Password": "your-certificate-password"
         }
       }
     }
   }
   ```

2. **CORS Configuration**
   ```csharp
   // In Program.cs for production
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("ProductionPolicy", policy =>
       {
           policy.WithOrigins("https://yourdomain.com")
                 .AllowAnyMethod()
                 .AllowAnyHeader();
       });
   });
   ```

## Testing Deployment

### Health Check

1. **Test API Availability**
   ```bash
   curl -X GET "http://your-server:5000/api/templatereports/templates"
   ```

2. **Test Report Generation**
   ```bash
   curl -X POST "http://your-server:5000/api/templatereports/generate" \
     -H "Content-Type: application/json" \
     -d @test_request.json \
     --output test_report.pdf
   ```

3. **Verify Swagger UI** (Development/Staging only)
   Navigate to `http://your-server:5000` to access Swagger UI.

### Load Testing

Use tools like Apache Bench or Artillery for load testing:

```bash
# Apache Bench example
ab -n 100 -c 10 http://your-server:5000/api/templatereports/templates
```

## Monitoring

### Logging

1. **Application Logs**
   - Default location: Console output
   - Production: Configure structured logging (Serilog, NLog)

2. **Log Levels**
   - Information: General application flow
   - Warning: Unusual but handled situations
   - Error: Error conditions that need attention

### Performance Monitoring

1. **Key Metrics**
   - Request duration
   - Memory usage
   - PDF generation time
   - Error rates

2. **Monitoring Tools**
   - Application Insights (Azure)
   - Prometheus + Grafana
   - Custom dashboards

### Health Checks

Add health check endpoints:

```csharp
// In Program.cs
builder.Services.AddHealthChecks();

// In pipeline
app.MapHealthChecks("/health");
```

## Troubleshooting

### Common Issues

1. **"Template not found" Error**
   - Verify templates are registered in DI container
   - Check template IDs match request
   - Review startup logs for registration errors

2. **PDF Generation Fails**
   - Check QuestPDF license configuration
   - Verify font availability on server
   - Check image format compatibility

3. **High Memory Usage**
   - Monitor concurrent requests
   - Implement request throttling
   - Consider scaling horizontally

4. **Slow Response Times**
   - Profile PDF generation process
   - Optimize template rendering
   - Consider caching strategies

### Diagnostic Commands

```bash
# Check application status
curl -I http://your-server:5000/health

# View application logs
tail -f /path/to/logs/application.log

# Check memory usage
top -p $(pgrep -f ReportHub)

# Test template validation
curl -X POST "http://your-server:5000/api/templatereports/validate" \
  -H "Content-Type: application/json" \
  -d '{"reportType":"venues_invoice"}'
```

### Support Information

- **Log Location**: Check console output or configured log files
- **Configuration**: Review `appsettings.json` and environment variables
- **Templates**: Verify in `/api/templatereports/templates` endpoint
- **Sample Data**: Use `/api/templatereports/templates/{id}/sample` for testing

## Scaling Considerations

### Horizontal Scaling

1. **Load Balancer Configuration**
   - Session affinity not required
   - Health check endpoint: `/health`
   - Distribute load evenly

2. **Stateless Design**
   - No shared state between instances
   - Each instance can handle any request
   - Template registration happens at startup

### Performance Optimization

1. **Caching Strategy**
   - Cache compiled templates
   - Cache frequently used logos
   - Implement response caching for metadata endpoints

2. **Resource Management**
   - Limit concurrent PDF generations
   - Implement proper disposal patterns
   - Monitor memory allocation

## Backup and Recovery

1. **Application Backup**
   - Source code and configuration files
   - Custom templates and assets
   - Log files (if needed)

2. **Recovery Process**
   - Restore application files
   - Verify configuration
   - Test template functionality
   - Monitor for errors

This deployment guide ensures a smooth and reliable deployment of the ReportHub system in various environments.