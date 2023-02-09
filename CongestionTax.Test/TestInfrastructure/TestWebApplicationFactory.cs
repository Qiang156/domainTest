using CongestionTax.DataLayer.Repositories;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CongestionTax.Test.TestInfrastructure;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public IMediator Mediator => Services.GetRequiredService<IMediator>();
    public IDailyFeeRepository DailyFeeRepository => Services.GetRequiredService<IDailyFeeRepository>();

    public TestWebApplicationFactory()
    {
           
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
    }
}