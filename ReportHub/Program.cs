using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using ReportHub.Common.Authentication; 
using ReportHub.Common.Base;
using ReportHub.Common.Helper.DataHelpers;
using ReportHub.Common.Helper.DataHelpers.IDataHelpers;
using ReportHub.Middlewares;
using Swashbuckle.AspNetCore.Filters;
using System.Data;
using System.Data.SqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//builder.Configuration.AddJsonFile("LogisticsSalesSettings.json", optional: false, reloadOnChange: true);

ReportHub.Common.Base.ServiceCollectionExtensions.RepositroyScoped(builder.Services);
ReportHub.Common.Base.CustomServiceCollectionExtensions.RepositroyScoped(builder.Services);

//builder.Services.AddDbContext<LogisticsSales.DataAccess.Base.LogisticsSalesContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("LogisticsSalesDB"));
//});


//builder.Services.AddTransient<IDbConnection>((sp) => new SqlConnection(builder.Configuration.GetConnectionString("DappereSchoolImageDB")));



builder.Services.AddHttpContextAccessor();




builder.Services.AddSwaggerGen(setup =>
{
    setup.EnableAnnotations();
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put ONLY your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    setup.SwaggerDoc("v1", new OpenApiInfo { Title = "API v1", Version = "v1" });
    setup.SwaggerDoc("v2", new OpenApiInfo { Title = "API v2", Version = "v2" });

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    setup.OperationFilter<SecurityRequirementsOperationFilter>();

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:SecretKey").Value ?? "")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{

    if (app.Environment.IsDevelopment())
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Base");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Main");
    }
    else
    {
        // To deploy on IIS
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Base");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Main");
    }

    // display the Request Duration
    c.DisplayRequestDuration();

    // Inject custom JavaScript and CSS
    c.InjectStylesheet("/swagger-ui/swagger-custom.css");
    c.InjectJavascript("/swagger-ui/swagger-custom.js");
});

AutoMapperWebConfiguration.Activate();
app.UseStaticFiles();
app.UseRouting();

app.UseMiddleware<TokenMiddleware>();
app.UseMiddleware<ApiWrapper>();

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
