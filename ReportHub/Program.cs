using ReportHub.Common.Base;
using ReportHub.Common.Configuration;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
using ReportHub.Objects.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "ReportHub API", 
        Version = "v1",
        Description = "Centralized reporting system supporting 50+ report types with dynamic branding"
    });
    
    // Include XML comments for API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.SetMinimumLevel(LogLevel.Information);
});

// Add new template engine services
builder.Services.AddTemplateEngine();
builder.Services.AddReportTemplates();

// Legacy PDF generator services (for backward compatibility)
builder.Services.AddScoped<PDF_GeneratorHelper>();
builder.Services.AddScoped<Word_GeneratorHelper>();
builder.Services.AddScoped<Excel_GeneratorHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReportHub API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Initialize template engine with registered templates
app.Services.ConfigureTemplateEngine();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("ReportHub started successfully");

// Log available templates
var templateEngine = app.Services.GetRequiredService<ITemplateEngineService>();
var availableTemplates = templateEngine.GetAvailableTemplates();
logger.LogInformation("Available templates: {Count}", availableTemplates.Count);
foreach (var template in availableTemplates)
{
    logger.LogInformation("- {TemplateId}: {DisplayName}", template.TemplateId, template.DisplayName);
}

app.Run();
