using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlMadina.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Product>? Products { get; set; }
    }
}