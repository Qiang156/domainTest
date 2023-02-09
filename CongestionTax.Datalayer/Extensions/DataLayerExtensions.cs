using CongestionTax.DataLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CongestionTax.DataLayer.Extensions;

public static class DataLayerExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services)
    {
        services.AddSingleton<IDailyFeeRepository, DailyFeeRepository>();

        services.AddSingleton<IRuleRepository, RuleRepository>();

        return services;
    }
}