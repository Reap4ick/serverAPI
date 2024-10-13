namespace ApiStore.Models.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
