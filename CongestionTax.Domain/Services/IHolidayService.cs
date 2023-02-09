namespace CongestionTax.Domain.Services
{
    public interface IHolidayService
    {
        Task<bool> IsHolidayAsync(DateTime date, string country);
    }
}
