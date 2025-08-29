using Domain.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TicketingSystem.Services
{
    //public class ProductService : IProductService
    //{
    //    private readonly ApplicationDbContext _context;

    //    public ProductService(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    //    {
    //        return await _context.Products
    //            .OrderBy(p => p.ProductName)
    //            .ToListAsync();
    //    }

    //    public async Task<Product?> GetProductByIdAsync(int id)
    //    {
    //        return await _context.Products
    //            .FirstOrDefaultAsync(p => p.Id == id);
    //    }

    //    public async Task<Product> CreateProductAsync(Product product)
    //    {
    //        _context.Products.Add(product);
    //        await _context.SaveChangesAsync();
    //        return product;
    //    }

    //    public async Task<Product?> UpdateProductAsync(int id, Product product)
    //    {
    //        var existingProduct = await _context.Products.FindAsync(id);
    //        if (existingProduct == null)
    //            return null;

    //        existingProduct.ProductName = product.ProductName;
    //        existingProduct.UnitPrice = product.UnitPrice;

    //        await _context.SaveChangesAsync();
    //        return existingProduct;
    //    }

    //    public async Task<bool> DeleteProductAsync(int id)
    //    {
    //        var product = await _context.Products.FindAsync(id);
    //        if (product == null)
    //            return false;

    //        // Check if product is used in any orders
    //        var hasOrders = await _context.OrderItems
    //            .AnyAsync(oi => oi.ProductId == id);

    //        if (hasOrders)
    //            throw new InvalidOperationException("Cannot delete product that has been ordered");

    //        _context.Products.Remove(product);
    //        await _context.SaveChangesAsync();
    //        return true;
    //    }
    //}
}
