using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlMadina.Application.DTOs;
using AlMadina.Application.Interfaces;
using AlMadina.Domain.Entities;

namespace AlMadina.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ================== CREATE ORDER ==================
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                CustomerPhone = dto.CustomerPhone,
                CustomerAddress = dto.CustomerAddress,
                Notes = dto.Notes,
                DeliveryFee = dto.DeliveryFee,
                Status = OrderStatus.Pending,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };

            decimal total = 0;
            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Product {item.ProductId} not found");

                var unitPrice = product.Price;
                // apply discount if active
                if (product.DiscountPercentage.HasValue && product.DiscountPercentage > 0)
                {
                    unitPrice = product.Price * (1 - product.DiscountPercentage.Value / 100m);
                }

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = unitPrice * item.Quantity
                };

                total += orderItem.TotalPrice;
                order.Items.Add(orderItem);

                // decrease stock
                product.StockQuantity -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            order.TotalPrice = total + order.DeliveryFee;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { orderId = order.Id, total = order.TotalPrice, status = order.Status });
        }

        // ================== GET ALL ORDERS ==================
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _unitOfWork.Orders
                .GetAllQueryable()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(MapOrders(orders));
        }

        // ================== GET MY ORDERS ==================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders([FromQuery] string userId)
        {
            var orders = await _unitOfWork.Orders
                .GetAllQueryable()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(MapOrders(orders));
        }

        // ================== GET ORDER BY ID ==================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _unitOfWork.Orders
                .GetAllQueryable()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(MapOrder(order));
        }

        // ================== UPDATE STATUS / PAYMENT ==================
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null)
                return NotFound();

            order.Status = dto.Status;
            order.IsPaid = dto.IsPaid;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Updated");
        }

        // ================== DELETE ORDER ==================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            // restore stock
            var items = await _unitOfWork.OrderItems
                .GetAllQueryable()
                .Where(i => i.OrderId == id)
                .ToListAsync();

            foreach (var item in items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }

            _unitOfWork.Orders.Delete(order);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Deleted");
        }

        private static List<OrderDto> MapOrders(List<Order> orders)
        {
            return orders.Select(MapOrder).ToList();
        }

        private static OrderDto MapOrder(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CustomerPhone = order.CustomerPhone,
                CustomerAddress = order.CustomerAddress,
                Notes = order.Notes,
                DeliveryFee = order.DeliveryFee,
                IsPaid = order.IsPaid,
                Items = order.Items?.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.NameAr ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}
