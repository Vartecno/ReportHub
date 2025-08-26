using Microsoft.Extensions.DependencyInjection;
using ReportHub.Common.DataHelpers;
using ReportHub.Common.DataHelpers.IDataHelpers;

namespace ReportHub.Common.Base;

public static class CustomServiceCollectionExtensions
{
    public static void RepositroyScoped(this IServiceCollection services)
    {
        services.AddScoped<IClient, Client>();

        services.AddHttpClient();
    }
}
