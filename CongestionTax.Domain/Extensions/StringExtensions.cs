namespace CongestionTax.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string GenerateDailyFeeReference(this string registrationNumber, DateTime entryTime, string city)
        {
            return $"{city}_{registrationNumber}_{entryTime:yyyyMMdd}";
        }
    }
}