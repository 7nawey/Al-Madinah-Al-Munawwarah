namespace AlMadina.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }  // Identity FK فقط

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public string CustomerPhone { get; set; }

        public string CustomerAddress { get; set; }

        public string? Notes { get; set; }

        public decimal DeliveryFee { get; set; }

        public bool IsPaid { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}