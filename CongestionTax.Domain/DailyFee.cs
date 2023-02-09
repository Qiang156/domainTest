using CongestionTax.Domain.Extensions;
using CongestionTax.Domain.Services;

namespace CongestionTax.Domain;

public class DailyFee : EntityBase
{
    private DailyFee(Rule rule, string registrationNumber, Vehicle vehicle, string reference, DateTime createdDate, string city)
        : base(reference)
    {
        Rule = rule;
        RegistrationNumber = registrationNumber;
        Vehicle = vehicle;
        Entries = new List<EntryItem>();
        Fee = 0;
        CreatedDate = createdDate;
        City = city;
    }

    public Vehicle Vehicle { get; }
    public Rule Rule { get; }
    public string RegistrationNumber { get; }
    public DateTime? LastAccumulateFeeTime { get; private set; }
    public List<EntryItem> Entries { get; }
    public decimal Fee { get; private set; }
    public DateTime CreatedDate { get; }

    public string City { get; }

    public static DailyFee Create(Rule rule, string registrationNumber, Vehicle vehicle, DateTime createDate, string city)
    {
        var createDay = createDate;
        var reference = registrationNumber.GenerateDailyFeeReference(createDay, city);

        return new DailyFee(rule, registrationNumber, vehicle, reference, createDay, city);
    }

    public async Task AddEntry(DateTime entryTime, IHolidayService holidayService)
    {
        var (totalFee, currentTollFee, accumulateFeeTime) =
            await Rule.CalculateFee(Vehicle, entryTime, Fee, LastAccumulateFeeTime, Entries, holidayService);
        Fee = totalFee;
        LastAccumulateFeeTime = accumulateFeeTime;
        Entries.Add(new EntryItem
        {
            Time = entryTime,
            Fee = currentTollFee
        });
    }
}

public class EntityBase
{
    public EntityBase(string reference)
    {
        Reference = reference;
    }

    public string Reference { get; }
}