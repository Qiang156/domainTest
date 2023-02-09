using CongestionTax.Domain;

namespace CongestionTax.Api.Models
{
    public class AddEntryRequest
    {
        public string RegistrationNumber { get; set; }
        public Vehicle Vehicle { get; set; }
        public string City { get; set; }
    }
}