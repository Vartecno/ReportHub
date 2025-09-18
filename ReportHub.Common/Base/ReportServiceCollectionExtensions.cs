using Microsoft.Extensions.DependencyInjection;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
namespace ReportHub.Common.Base 
{
    public static class ReportServiceCollectionExtensions
    {
        public static IServiceCollection AddReportGeneration(this IServiceCollection services)
        {
           
     
            services.AddTransient<IFormatSpecificGenerator, Word_GeneratorHelper>();
            services.AddTransient<IFormatSpecificGenerator, Excel_GeneratorHelper>();

            
            services.AddTransient<IReportGeneratorService, ReportGenerationService>();

            return services;
        }

        public static IServiceCollection AddReportGenerationWithCustomGenerators(
            this IServiceCollection services,
            params Type[] customGeneratorTypes)
        {
            services.AddReportGeneration();

            foreach (var generatorType in customGeneratorTypes)
            {
                if (!typeof(IFormatSpecificGenerator).IsAssignableFrom(generatorType))
                {
                    throw new ArgumentException($"Type {generatorType.Name} must implement IFormatSpecificGenerator");
                }
                services.AddTransient(typeof(IFormatSpecificGenerator), generatorType);
            }

            return services;
        }
    }   
}