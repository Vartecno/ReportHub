using Microsoft.Extensions.DependencyInjection;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
using ReportHub.Common.Templates;
using ReportHub.Objects.Interfaces;

namespace ReportHub.Common.Configuration
{
    
    public static class TemplateEngineServiceExtensions
    {        
        public static IServiceCollection AddTemplateEngine(this IServiceCollection services)
        {
             services.AddSingleton<ITemplateEngine, TemplateEngine>();
            services.AddScoped<ITemplateEngineService, TemplateEngineService>();
            services.AddScoped<TemplateBasedPDFGenerator>();
            

            services.AddScoped<IFormatSpecificGenerator>(provider => 
                provider.GetRequiredService<TemplateBasedPDFGenerator>());

            return services;
        }
        
        public static IServiceCollection AddReportTemplates(this IServiceCollection services)
        {
            services.AddSingleton<IReportTemplate, SalesInvoiceTemplate>();
            services.AddSingleton<IReportTemplate, AccountStatementTemplate>();
           

            return services;
        }
        
        public static IServiceCollection AddReportServices(this IServiceCollection services)
        {
     
            
            return services;
        }        
        public static IServiceProvider ConfigureTemplateEngine(this IServiceProvider serviceProvider)
        {
            var templateEngine = serviceProvider.GetRequiredService<ITemplateEngine>();
            var templates = serviceProvider.GetServices<IReportTemplate>();

            foreach (var template in templates)
            {
                templateEngine.RegisterTemplate(template);
            }

            return serviceProvider;
        }
    }
}