using Microsoft.EntityFrameworkCore;
using AlMadina.Application.Interfaces;
using AlMadina.Domain.Entities;
using AlMadina.Infrastructure.Persistence;

namespace AlMadina.Infrastructure.Repositories
{
    public class ProductRepository
        : GenericRepository<Product>,
          IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public IQueryable<Product> GetAllQueryable()
        {
            return _context.Products.AsQueryable();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedAsync()
        {
            return await _context.Products
                .Where(x => x.IsFeatured)
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetDiscountedAsync()
        {
            return await _context.Products
                .Where(x => x.DiscountPercentage > 0)
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBestSellingAsync()
        {
            return await _context.Products
                .Include(x => x.OrderItems)
                .Include(x => x.Category)
                .OrderByDescending(x =>
                    x.OrderItems.Sum(o => o.Quantity))
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetOthersAsync()
        {
            return await _context.Products
                .Where(x => x.CategoryId == null)
                .Include(x => x.Category)
                .ToListAsync();
        }
    }
}