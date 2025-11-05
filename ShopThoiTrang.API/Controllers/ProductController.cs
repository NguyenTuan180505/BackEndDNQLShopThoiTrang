using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Services;

namespace ShopThoiTrangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int? categoryId, [FromQuery] string? sortBy)
        {
            var products = await _service.GetAllProducts(search, categoryId, sortBy);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetProductById(id);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại" });
            return Ok(product);
        }
    }
}
