using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AlMadina.Application.DTOs;
using AlMadina.Application.Interfaces;
using AlMadina.Application.Interfaces.Services;
using AlMadina.Domain.Entities;

namespace AlMadina.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        // ================= GET ALL (PAGINATION) =================
        public async Task<PagedResult<ProductDto>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            var query = _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<List<ProductDto>>(products)
            };
        }

        // ================= SEARCH (PRODUCT + CATEGORY + BARCODE) =================
        public async Task<IEnumerable<ProductDto>> SearchAsync(string keyword)
        {
            keyword = keyword?.Trim() ?? "";

            var products = await _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .Where(x =>
                    x.NameAr.Contains(keyword) ||
                    x.NameEn.Contains(keyword) ||
                    (x.Barcode != null && x.Barcode.Contains(keyword)) ||
                    (x.Category != null &&
                     (x.Category.NameAr.Contains(keyword) ||
                      x.Category.NameEn.Contains(keyword)))
                )
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<ProductDto>>(products);
        }

        // ================= GET BY ID =================
        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        // ================= GET FOR UPDATE (SEARCH BY NAME OR BARCODE) =================
        public async Task<ProductDto?> GetForUpdateAsync(string value)
        {
            value = value?.Trim() ?? "";

            var product = await _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.NameAr == value ||
                    x.NameEn == value ||
                    x.Barcode == value);

            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        // ================= FEATURED =================
        public async Task<IEnumerable<ProductDto>> GetFeaturedAsync()
        {
            var products = await _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .Where(x => x.IsFeatured)
                .ToListAsync();

            return _mapper.Map<List<ProductDto>>(products);
        }

        // ================= DISCOUNTED =================
        public async Task<IEnumerable<ProductDto>> GetDiscountedAsync()
        {
            var products = await _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .Where(x => x.DiscountPercentage > 0)
                .ToListAsync();

            return _mapper.Map<List<ProductDto>>(products);
        }

        // ================= BEST SELLING (REAL SALES LOGIC) =================
        public async Task<IEnumerable<ProductDto>> GetBestSellingAsync()
        {
            var products = await _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .Include(x => x.OrderItems)
                .Select(p => new
                {
                    Product = p,
                    SoldQty = p.OrderItems.Sum(o => o.Quantity)
                })
                .OrderByDescending(x => x.SoldQty)
                .Take(30)
                .Select(x => x.Product)
                .ToListAsync();

            return _mapper.Map<List<ProductDto>>(products);
        }

        // ================= OTHERS =================
        public async Task<IEnumerable<ProductDto>> GetOthersAsync()
        {
            var products = await _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .Where(x => x.Category != null && x.Category.NameEn == "Others")
                .ToListAsync();

            return _mapper.Map<List<ProductDto>>(products);
        }

        // ================= CREATE =================
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var exists = await _unitOfWork.Products
                .GetAllQueryable()
                .AnyAsync(x => x.NameAr == dto.NameAr);

            if (exists)
                throw new Exception("Product already exists");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                NameAr = dto.NameAr,
                NameEn = string.IsNullOrWhiteSpace(dto.NameEn) ? dto.NameAr : dto.NameEn,
                Barcode = dto.Barcode,
                Price = dto.Price,
                CostPrice = dto.CostPrice,
                StockQuantity = dto.StockQuantity
            };

            Category category;

            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                category = await _unitOfWork.Categories
                    .GetAllQueryable()
                    .FirstOrDefaultAsync(x =>
                        x.NameAr == dto.CategoryName ||
                        x.NameEn == dto.CategoryName);

                if (category == null)
                {
                    category = new Category
                    {
                        Id = Guid.NewGuid(),
                        NameAr = dto.CategoryName,
                        NameEn = dto.CategoryName,
                        IsActive = true
                    };

                    await _unitOfWork.Categories.AddAsync(category);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                category = await _unitOfWork.Categories
                    .GetAllQueryable()
                    .FirstOrDefaultAsync(x => x.NameEn == "Others");
            }

            product.CategoryId = category.Id;

            if (dto.Image != null)
                product.ImageUrl = await _fileService.UploadImageAsync(dto.Image);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        // ================= UPDATE =================
        public async Task<bool> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products
                .GetByIdAsync(dto.Id);

            if (product == null)
                return false;

            product.NameAr = dto.NameAr;
            product.NameEn = dto.NameEn;
            product.Barcode = dto.Barcode;
            product.Price = dto.Price;
            product.CostPrice = dto.CostPrice;
            product.StockQuantity = dto.StockQuantity;
            product.IsActive = dto.IsActive;

            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                var category = await _unitOfWork.Categories
                    .GetAllQueryable()
                    .FirstOrDefaultAsync(x =>
                        x.NameAr == dto.CategoryName ||
                        x.NameEn == dto.CategoryName);

                if (category != null)
                    product.CategoryId = category.Id;
            }

            if (dto.Image != null)
                product.ImageUrl = await _fileService.UploadImageAsync(dto.Image);

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return false;

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<PagedResult<ProductDto>> GetProductsByCategoryAsync(
    Guid categoryId,
    int pageNumber = 1,
    int pageSize = 40)
        {
            var query = _unitOfWork.Products
                .GetAllQueryable()
                .Include(x => x.Category)
                .Where(x => x.CategoryId == categoryId)
                .OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<List<ProductDto>>(products)
            };
        }
    }
}