
using CongestionTax.Domain.Extensions;
using CongestionTax.Domain.Services;

namespace CongestionTax.Domain;

public class Rule : EntityBase
{
    public string City { get; set; }
    public int Version { get; set; }
    public int? CountOnceWithinMinutes { get; set; }
    public bool FreeForWeekend { get; set; }
    public bool FreeForHoliday { get; set; }
    public bool FreeBeforeHoliday { get; set; }
    public int? FreeMonth { get; set; }
    public decimal MaxAmountPerDay { get; set; }
    public List<Vehicle> TollFreeVehicles { get; set; }
    public HashSet<TimeRange> Fees { get; set; }

    private readonly Dictionary<double, decimal> _feeTable;
            

    public Rule(string reference, HashSet<TimeRange> fees) : base(reference)
    {
        Fees = fees;
        var test = Fees.Select(f => f.ToFee());
        _feeTable = Fees.Select(f => f.ToFee()).ToDictionary(k => k.Key, k => k.Value);
    }

    public async Task<(decimal totalFee, decimal currentTollFee, DateTime? accumulateFeeTime)> CalculateFee(Vehicle vehicle, DateTime entryTime,
        decimal totalFee, DateTime? lastAccumulateFeeTime, List<EntryItem> lastEntries, IHolidayService holidayService)
    {
        if (TollFreeVehicles.Contains(vehicle))
            return (totalFee, 0, lastAccumulateFeeTime);

        if (totalFee == MaxAmountPerDay || await IsFreeDay(entryTime, holidayService))
            return (totalFee, 0, lastAccumulateFeeTime);

        var entryTimeSecond = entryTime.TimeOfDay.TotalSeconds;
        var currentTollFee = GetCurrentEntryFee(entryTimeSecond);

        var refreshedResult =  Refresh(totalFee, currentTollFee, entryTime, lastAccumulateFeeTime, lastEntries);

        return (refreshedResult.totalFee > MaxAmountPerDay ? MaxAmountPerDay : refreshedResult.totalFee,
            refreshedResult.currentTollFee, refreshedResult.accumulateFeeTime);
    }

    public async Task<bool> IsFreeDay(DateTime entryTime, IHolidayService holidayService)
    {
        if (FreeMonth == entryTime.Month)
        {
            return true;
        }

        if (FreeForHoliday && await holidayService.IsHolidayAsync(entryTime, "SE")) 
        {
            return true;
        }

        if (FreeBeforeHoliday && await holidayService.IsHolidayAsync(entryTime.AddDays(1), "SE"))
        {
            return true;
        }

        if (FreeForWeekend && entryTime.IsWeekend())
        {
            return true;
        }

        return false;
    }

    public decimal GetCurrentEntryFee(double entryTimeSecond)
    {
        if (!Fees.Any())
            return 0;

        var index = Fees.Select( f => f.From.TotalSeconds).ToList();

        var left = 0;
        var right = index.Count - 1;
        var targetIndex = -1;

        while (left <= right)
        {
            var mid = left + right >> 1;
            if (index[mid] == entryTimeSecond)
            {
                targetIndex = mid;
                break;
            }
            else if (index[mid] > entryTimeSecond)
            {
                right = mid - 1;
            }
            else if (index[mid] < entryTimeSecond)
            {
                left = mid + 1;
            }
            else if (index[left] == entryTimeSecond)
            {
                targetIndex = left;
                break;
            }
        }

        if (targetIndex < 0)
        {
            targetIndex = left > right ? right : left;
        }

        return _feeTable[index[targetIndex]];
    }

    private (decimal totalFee, decimal currentTollFee, DateTime accumulateFeeTime) Refresh(decimal currentTotalFee, decimal currentTollFee,
        DateTime currentEntryTime, DateTime? lastAccumulateFeeTime, IEnumerable<EntryItem> entries)
    {
        if (!CountOnceWithinMinutes.HasValue || !lastAccumulateFeeTime.HasValue)
        {
            return (currentTotalFee + currentTollFee, currentTollFee, currentEntryTime);
        }

        if (currentEntryTime > lastAccumulateFeeTime.Value.AddMinutes(CountOnceWithinMinutes.Value))
        {
            return (currentTotalFee + currentTollFee, currentTollFee, currentEntryTime);
        }

        var previousFee = entries.Where(e => e.Time >= lastAccumulateFeeTime).Select(e => e.Fee).Max();

        if (currentTollFee > previousFee)
        {
            return (currentTotalFee + (currentTollFee - previousFee), currentTollFee, lastAccumulateFeeTime.Value);
        }

        return (currentTotalFee, currentTollFee, lastAccumulateFeeTime.Value);
    }
}

public class TimeRange
{
    public TimeRange(TimeSpan from, decimal price)
    {
        From = from;
        Price = price;
    }

    public TimeSpan From { get; set; }
    public decimal Price { get; set; }

    public KeyValuePair<double, decimal> ToFee()
    {
        return new KeyValuePair<double, decimal>(From.TotalSeconds, Price);
    }
}