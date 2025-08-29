using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{

    //[ApiController]
    //[Route("api/[controller]")]
    //public class ProductsController : ControllerBase
    //{
    //    private readonly IProductService _productService;

    //    public ProductsController(IProductService productService)
    //    {
    //        _productService = productService;
    //    }

    //    /// <summary>
    //    /// Get all products (Tickets, Memberships, Extras)
    //    /// </summary>
    //    [HttpGet]
    //    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    //    {
    //        try
    //        {
    //            var products = await _productService.GetAllProductsAsync();
    //            var productDtos = products.Select(p => new ProductDto
    //            {
    //                Id = p.Id,
    //                ProductName = p.ProductName,
    //                UnitPrice = p.UnitPrice
    //            });
    //            //use ApiResponse to return the list of products
    //            if (!productDtos.Any())
    //                return NotFound(new { message = "No products found" });
    //            return Ok(new ApiResponse<IEnumerable<ProductDto>>(200, productDtos, "Products retrieved successfully"));
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
    //        }
    //    }

    //    /// <summary>
    //    /// Get product by ID
    //    /// </summary>
    //    [HttpGet("{id}")]
    //    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    //    {
    //        try
    //        {
    //            var product = await _productService.GetProductByIdAsync(id);
    //            if (product == null)
    //                return NotFound(new { message = "Product not found" });

    //            var productDto = new ProductDto
    //            {
    //                Id = product.Id,
    //                ProductName = product.ProductName,
    //                UnitPrice = product.UnitPrice
    //            };
    //            //use ApiResponse to return the product
    //            return Ok(new ApiResponse<ProductDto>(200, productDto, "Product retrieved successfully"));
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new { message = "An error occurred while retrieving the product", error = ex.Message });
    //        }
    //    }

    //    /// <summary>
    //    /// Add new product (Admin only)
    //    /// </summary>
    //    [HttpPost]
    //    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //                return BadRequest(ModelState);

    //            var product = new Product
    //            {
    //                ProductName = createProductDto.ProductName,
    //                UnitPrice = createProductDto.UnitPrice
    //            };

    //            var createdProduct = await _productService.CreateProductAsync(product);

    //            var productDto = new ProductDto
    //            {
    //                Id = createdProduct.Id,
    //                ProductName = createdProduct.ProductName,
    //                UnitPrice = createdProduct.UnitPrice
    //            };
    //            //use ApiResponse to return the created product
    //            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, new ApiResponse<ProductDto>(201, productDto, "Product created successfully"));
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new { message = "An error occurred while creating the product", error = ex.Message });
    //        }
    //    }

    //    /// <summary>
    //    /// Update existing product (Admin only)
    //    /// </summary>
    //    [HttpPut("{id}")]
    //    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //                return BadRequest(ModelState);

    //            var product = new Product
    //            {
    //                ProductName = updateProductDto.ProductName,
    //                UnitPrice = updateProductDto.UnitPrice
    //            };

    //            var updatedProduct = await _productService.UpdateProductAsync(id, product);
    //            if (updatedProduct == null)
    //                return NotFound(new { message = "Product not found" });

    //            var productDto = new ProductDto
    //            {
    //                Id = updatedProduct.Id,
    //                ProductName = updatedProduct.ProductName,
    //                UnitPrice = updatedProduct.UnitPrice
    //            };
    //            // use ApiResponse to return the updated product
    //            return Ok(new ApiResponse<ProductDto>(200, productDto, "Product updated successfully"));
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new { message = "An error occurred while updating the product", error = ex.Message });
    //        }
    //    }

    //    /// <summary>
    //    /// Delete product (Admin only)
    //    /// </summary>
    //    [HttpDelete("{id}")]
    //    public async Task<ActionResult> DeleteProduct(int id)
    //    {
    //        try
    //        {
    //            var result = await _productService.DeleteProductAsync(id);
    //            if (!result)
    //                return NotFound(new { message = "Product not found" });

    //            return Ok(new { message = "Product deleted successfully" });
    //        }
    //        catch (InvalidOperationException ex)
    //        {
    //            return BadRequest(new { message = ex.Message });
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new { message = "An error occurred while deleting the product", error = ex.Message });
    //        }
    //    }
    //}


}
