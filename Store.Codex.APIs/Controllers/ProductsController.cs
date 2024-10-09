using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Codex.Core.Dtos.Products;
using Store.Codex.Core.Entities;
using Store.Codex.Core.Helper;
using Store.Codex.Core.Services.Contract;
using Store.Codex.Core.Specifications.Products;

namespace Store.Codex.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]  // Get BaseUrl/api/Products  
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductSpecParams productSpec) // endpoint
        {
            var result = await _productService.GetAllProductsAsync(productSpec);

            return Ok(result); // ok returns the Status code 200 
        }

        [HttpGet("brands")]  // BaseUrl/api/Products/brands
        public async Task<IActionResult> GetAllBrands()
        {
            var result = await _productService.GetAllBrandsAsync();
            return Ok(result);
        }

        [HttpGet("types")]  // BaseUrl/api/Products/types
        public async Task<IActionResult> GetAllTypes()
        {
            var result = await _productService.GetAllTypesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]  // BaseUrl/api/Products/id   
        public async Task<IActionResult> GetProductById(int? id)
        {
            if (id is null) return BadRequest("Invalid Id !!");
            var result = await _productService.GetProductByIdAsync(id.Value);

            if(result is null) return NotFound($"The Product With Id: {id} Not Found At Database !");   
            return Ok(result);
        }
    }
}
