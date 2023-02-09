using CongestionTax.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CongestionTax.Domain.Extensions
{
    public static class DomainExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new HolidayServiceConfig();
            configuration.Bind("HolidayService", config);


            services.AddHttpClient<IHolidayService, HolidayService>(client =>
            {
                client.BaseAddress = new Uri(config.EndPoint);
            });

            return services;
        }
    }
}
