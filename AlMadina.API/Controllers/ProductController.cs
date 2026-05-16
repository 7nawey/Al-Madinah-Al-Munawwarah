using Microsoft.AspNetCore.Mvc;
using AlMadina.Application.DTOs;
using AlMadina.Application.Interfaces.Services;

namespace AlMadina.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // ================== GET PAGINATED PRODUCTS ==================
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts(
            int pageNumber = 1,
            int pageSize = 20)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // ================== SEARCH PRODUCTS ==================
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            var result = await _productService.SearchAsync(keyword);
            return Ok(result);
        }

        // ================== GET SINGLE PRODUCT BY ID ==================
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);

            if (result == null)
                return NotFound("Product not found");

            return Ok(result);
        }

        // ================== GET PRODUCT FOR EDIT (BY NAME OR BARCODE) ==================
        [HttpGet("edit/find")]
        public async Task<IActionResult> FindProductForEdit([FromQuery] string value)
        {
            var result = await _productService.GetForUpdateAsync(value);

            if (result == null)
                return NotFound("Product not found");

            return Ok(result);
        }

        // ================== FEATURED PRODUCTS ==================
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            return Ok(await _productService.GetFeaturedAsync());
        }

        // ================== DISCOUNTED PRODUCTS ==================
        [HttpGet("discounted")]
        public async Task<IActionResult> GetDiscountedProducts()
        {
            return Ok(await _productService.GetDiscountedAsync());
        }

        // ================== BEST SELLING PRODUCTS ==================
        [HttpGet("best-selling")]
        public async Task<IActionResult> GetBestSellingProducts()
        {
            return Ok(await _productService.GetBestSellingAsync());
        }

        // ================== OTHER CATEGORY PRODUCTS ==================
        [HttpGet("others")]
        public async Task<IActionResult> GetOtherProducts()
        {
            return Ok(await _productService.GetOthersAsync());
        }

        // ================== CREATE PRODUCT ==================
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(
            [FromForm] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return Ok(result);
        }

        // ================== UPDATE PRODUCT ==================
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(
            [FromForm] UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(dto);

            if (!result)
                return NotFound("Product not found");

            return Ok("Product updated successfully");
        }

        // ================== DELETE PRODUCT ==================
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteAsync(id);

            if (!result)
                return NotFound("Product not found");

            return Ok("Product deleted successfully");
        }

        // ================= CATEGORY PRODUCTS (PAGINATION LOAD MORE) =================
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(
            Guid categoryId,
            int pageNumber = 1,
            int pageSize = 40)
        {
            var result = await _productService
                .GetProductsByCategoryAsync(categoryId, pageNumber, pageSize);

            return Ok(result);
        }
    }
}