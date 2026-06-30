using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlMadina.Application.Interfaces;
using AlMadina.Application.DTOs;
using AlMadina.Domain.Entities;

namespace AlMadina.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Produces("application/json")]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalProducts = await _unitOfWork.Products.CountAsync();
            var totalCategories = await _unitOfWork.Categories.CountAsync();
            var totalOrders = await _unitOfWork.Orders.CountAsync();
            var totalRevenue = await _unitOfWork.Orders.GetAllQueryable()
                .Where(o => o.Status == OrderStatus.Completed)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;

            var pendingOrders = await _unitOfWork.Orders.GetAllQueryable()
                .CountAsync(o => o.Status == OrderStatus.Pending);

            var lowStock = await _unitOfWork.Products.GetAllQueryable()
                .CountAsync(p => p.StockQuantity < 10);

            return Ok(new
            {
                totalProducts,
                totalCategories,
                totalOrders,
                totalRevenue,
                pendingOrders,
                lowStock
            });
        }

        [HttpGet("best-selling")]
        public async Task<IActionResult> GetBestSelling()
        {
            var items = await _unitOfWork.OrderItems.GetAllQueryable()
                .Include(i => i.Product)
                .GroupBy(i => new { i.ProductId, i.Product.NameAr, i.Product.ImageUrl })
                .Select(g => new
                {
                    productId = g.Key.ProductId,
                    nameAr = g.Key.NameAr,
                    imageUrl = g.Key.ImageUrl,
                    totalSold = g.Sum(i => i.Quantity),
                    totalRevenue = g.Sum(i => i.TotalPrice)
                })
                .OrderByDescending(x => x.totalSold)
                .Take(10)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("most-profitable")]
        public async Task<IActionResult> GetMostProfitable()
        {
            var items = await _unitOfWork.OrderItems.GetAllQueryable()
                .Include(i => i.Product)
                .GroupBy(i => new { i.ProductId, i.Product.NameAr, i.Product.ImageUrl, i.Product.CostPrice })
                .Select(g => new
                {
                    productId = g.Key.ProductId,
                    nameAr = g.Key.NameAr,
                    imageUrl = g.Key.ImageUrl,
                    totalSold = g.Sum(i => i.Quantity),
                    totalRevenue = g.Sum(i => i.TotalPrice),
                    totalCost = g.Sum(i => g.Key.CostPrice * i.Quantity),
                    profit = g.Sum(i => i.TotalPrice) - g.Sum(i => g.Key.CostPrice * i.Quantity)
                })
                .OrderByDescending(x => x.profit)
                .Take(10)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders()
        {
            var orders = await _unitOfWork.Orders.GetAllQueryable()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToListAsync();

            return Ok(orders.Select(o => new
            {
                o.Id,
                o.CreatedAt,
                o.TotalPrice,
                o.Status,
                o.IsPaid,
                o.CustomerPhone,
                itemsCount = o.Items.Count
            }));
        }

        [HttpGet("sales-chart")]
        public async Task<IActionResult> GetSalesChart([FromQuery] int days = 30)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var orders = await _unitOfWork.Orders.GetAllQueryable()
                .Where(o => o.CreatedAt >= startDate && o.Status == OrderStatus.Completed)
                .ToListAsync();

            var chartData = orders
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    revenue = g.Sum(o => o.TotalPrice),
                    orders = g.Count()
                })
                .OrderBy(x => x.date)
                .ToList();

            return Ok(chartData);
        }
    }
}
