using CongestionTax.Business.Commands;
using CongestionTax.Domain;
using CongestionTax.Domain.Extensions;
using CongestionTax.Test.TestInfrastructure;
using Xunit.Abstractions;

namespace CongestionTax.Test.IntegrationTests
{
    public class FeeWithinOneHourTests : IClassFixture<FeeWithinOneHourTestsFixture>
    {
        private readonly FeeWithinOneHourTestsFixture _fixture;

        public FeeWithinOneHourTests(FeeWithinOneHourTestsFixture fixture,
            ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task VerifyMultipleCountEntriesPerHour()
        {
            var result = _fixture.Factory.DailyFeeRepository.Get(
                _fixture.RegisterNumber.GenerateDailyFeeReference(_fixture.CreateDate, _fixture.Stockholm));

            Assert.Equal(47, result.Fee);
            Assert.Equal(2, result.Entries.Count);
        }

        [Fact]
        public async Task VerifySingleCountEntriesPerHour()
        {
            var result = _fixture.Factory.DailyFeeRepository.Get(
                _fixture.RegisterNumber.GenerateDailyFeeReference(_fixture.CreateDate, _fixture.Gothenburg));

            Assert.Equal(8, result.Fee);
            Assert.Equal(2, result.Entries.Count);
            Assert.Equal(_fixture.CreateDate, result.LastAccumulateFeeTime);
        }
    }

    public class FeeWithinOneHourTestsFixture : IntegrationTestFixture
    {
        public string RegisterNumber;
        public DateTime CreateDate;
        public Vehicle Vehicle = Vehicle.Regular;
        public string Stockholm = "Stockholm";
        public string Gothenburg = "Gothenburg";
        
        protected override async Task ArrangeAsync()
        {
            RegisterNumber = "abc123";
            CreateDate = DateTime.Parse("2023-01-19 08:30:00");
        }

        protected override async Task ActAsync()
        {
            // Stockholm count multiple entry per hour
            await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate));
            await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Stockholm, CreateDate.AddMinutes(30)));

            // Gothenburg count single entry with max fee per hour
            await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate));
            await Factory.Mediator.Send(new AddEntryCommand(RegisterNumber, Vehicle, Gothenburg, CreateDate.AddMinutes(30)));
        }
    }
}
