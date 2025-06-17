namespace PhoneStoreAPI.Models
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
        public string ProductName { get; set; } 
        public string CategoryNameVN { get; set; }
    }
}
