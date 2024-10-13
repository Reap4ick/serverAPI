public class OrderEntity
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string UserEmail { get; set; }
    public decimal TotalAmount { get; set; }

    // Додано: Зв'язок з продуктами
    public ICollection<OrderProductEntity> OrderProducts { get; set; }
}
