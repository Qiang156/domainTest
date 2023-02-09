using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CongestionTax.Business.Extensions
{
    public static class CongestionTaxExtensions
    {
        public static IServiceCollection AddCongestionTax(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddMediatR(assemblies);

            return services;
        }
    }
}