using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlMadina.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string? Barcode { get; set; }

        public decimal Price { get; set; }

        public decimal CostPrice { get; set; }

        public int StockQuantity { get; set; }

        public Guid? CategoryId { get; set; }

        public Category? Category { get; set; }

        public bool IsFeatured { get; set; } = false;

        public decimal? DiscountPercentage { get; set; }

        public DateTime? DiscountStartDate { get; set; }

        public DateTime? DiscountEndDate { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

     
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<OrderItem>? OrderItems { get; set; }


        public ICollection<StockMovement>? StockMovements { get; set; }
    }
}
