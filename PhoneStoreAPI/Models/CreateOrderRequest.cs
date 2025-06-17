namespace PhoneStoreAPI.Models
{
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public string? Address { get; set; }
    }
}