using CongestionTax.Business.Commands;
using CongestionTax.Domain;
using CongestionTax.Domain.Extensions;
using CongestionTax.Test.TestInfrastructure;
using Xunit.Abstractions;

namespace CongestionTax.Test.IntegrationTests;

public class FeeReachDailyLimitTests : IClassFixture<FeeReachDailyLimitTestsFixture>
{
    private readonly FeeReachDailyLimitTestsFixture _fixture;

    public FeeReachDailyLimitTests(FeeReachDailyLimitTestsFixture fixture,
        ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task VerifyStockholmDailyLimit()
    {
        var result = _fixture.Factory.DailyFeeRepository.Get(
            _fixture.RegisterNumber.GenerateDailyFeeReference(_fixture.CreateDate, _fixture.Stockholm));

        Assert.Equal(135, result.Fee);
    }

    [Fact]
    public async Task VerifyGothenburgDailyLimit()
    {
        var result = _fixture.Factory.DailyFeeRepository.Get(
            _fixture.RegisterNumber.GenerateDailyFeeReference(_fixture.CreateDate, _fixture.Gothenburg));

        Assert.Equal(60, result.Fee);
    }
}

public class FeeReachDailyLimitTestsFixture : IntegrationTestFixture
{
    public string RegisterNumber;
    public DateTime CreateDate;
    public Vehicle Vehicle = Vehicle.Regular;
    public string Stockholm = "Stockholm";
    public string Gothenburg = "Gothenburg";
        
    protected override async Task ArrangeAsync()
    {
        RegisterNumber = "abc123";
        CreateDate = DateTime.Parse("2023-01-19 08:00:00");
    }

    protected override async Task ActAsync()
    {
        // Stockholm count multiple entry per hour
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate.AddMinutes(30)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate.AddMinutes(40)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate.AddMinutes(50)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate.AddMinutes(60)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate.AddMinutes(70)));

        // Gothenburg count single entry with max fee per hour
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate.AddMinutes(61)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate.AddMinutes(122)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate.AddMinutes(183)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate.AddMinutes(244)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate.AddMinutes(305)));
        await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate.AddMinutes(366)));
    }
}