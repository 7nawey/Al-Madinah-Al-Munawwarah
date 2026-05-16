using AlMadina.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlMadina.Application.DTOs
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public StockMovementType Type { get; set; }

        public string? Notes { get; set; }

        public DateTime Date { get; set; }
    }

    public class CreateStockMovementDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public StockMovementType Type { get; set; }

        public string? Notes { get; set; }
    }
}
