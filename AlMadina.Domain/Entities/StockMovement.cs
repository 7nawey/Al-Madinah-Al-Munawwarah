using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlMadina.Domain.Entities
{
    public class StockMovement
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public StockMovementType Type { get; set; }
        public string? Notes { get; set; }
       
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

    public enum StockMovementType
    {
        In,
        Out,
        Adjust
    }
}
