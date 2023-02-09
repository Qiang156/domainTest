using System.Text.Json;

namespace CongestionTax.Domain.Services;

public class HolidayService : IHolidayService
{
    private readonly HttpClient _httpClient;
 
    private readonly JsonSerializerOptions _jsonSerializerOptions 
        = new() { PropertyNameCaseInsensitive = true };

    public HolidayService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsHolidayAsync(DateTime date, string country)
    {
        var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{date.Year}/{country}");

        if (response.IsSuccessStatusCode)
        {
            await using var jsonStream = await response.Content.ReadAsStreamAsync();
            var publicHolidays = JsonSerializer.Deserialize<PublicHoliday[]>(jsonStream, _jsonSerializerOptions);

            if (publicHolidays != null && publicHolidays.Any())
            {
                return publicHolidays.Any(holiday => holiday.Date.DayOfYear == date.DayOfYear);
            }
        }

        return false;
    }
}