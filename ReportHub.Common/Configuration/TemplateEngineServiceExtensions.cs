using Microsoft.Extensions.DependencyInjection;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
using ReportHub.Objects.Interfaces;
using ReportHub.Objects.Templates;

namespace ReportHub.Common.Configuration
{
    /// <summary>
    /// Extension methods for configuring the template engine and related services
    /// </summary>
    public static class TemplateEngineServiceExtensions
    {
        /// <summary>
        /// Add template engine services to the DI container
        /// </summary>
        public static IServiceCollection AddTemplateEngine(this IServiceCollection services)
        {
            // Register template engine
            services.AddSingleton<ITemplateEngine, TemplateEngine>();
            services.AddScoped<ITemplateEngineService, TemplateEngineService>();

            // Register the new template-based PDF generator
            services.AddScoped<TemplateBasedPDFGenerator>();
            
            // Replace the old PDF generator with the new template-based one
            services.AddScoped<IFormatSpecificGenerator>(provider => 
                provider.GetRequiredService<TemplateBasedPDFGenerator>());

            return services;
        }

        /// <summary>
        /// Register all available report templates
        /// </summary>
        public static IServiceCollection AddReportTemplates(this IServiceCollection services)
        {
            services.AddSingleton<IReportTemplate, VenuesInvoiceTemplate>();
            services.AddSingleton<IReportTemplate, AccountStatementTemplate>();
            // Add more templates here as needed

            return services;
        }

        /// <summary>
        /// Add core ReportHub services to the DI container
        /// </summary>
        public static IServiceCollection AddReportServices(this IServiceCollection services)
        {
            // Register core ReportHub services
            // Add any additional services that are specific to ReportHub functionality
            // This method can be expanded as needed for future services
            
            return services;
        }

        /// <summary>
        /// Configure and initialize the template engine with all registered templates
        /// </summary>
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