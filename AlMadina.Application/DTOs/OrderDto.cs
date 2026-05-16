using AlMadina.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlMadina.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }

        public string? Notes { get; set; }

        public decimal DeliveryFee { get; set; }

        public bool IsPaid { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }


    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public Guid OrderId { get; set; }

        public OrderStatus Status { get; set; }

        public bool IsPaid { get; set; }
    }
    public class CreateOrderDto
    {
        public string? UserId { get; set; }

        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }

        public string? Notes { get; set; }

        public decimal DeliveryFee { get; set; }

        public List<CreateOrderItemDto> Items { get; set; }
    }


    public class OrderItemDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }

}
