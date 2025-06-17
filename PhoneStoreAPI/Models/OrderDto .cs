namespace PhoneStoreAPI.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Address { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }
}
