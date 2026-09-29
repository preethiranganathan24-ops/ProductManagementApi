using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.DB;
using ProductCatalog.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductCatalogContext _context;

    public ProductsController(ProductCatalogContext context)
    {
        _context = context;
    }

    // POST: api/products - Create a new product
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        if (!_context.Categories.Any(c => c.CategoryId == product.CategoryId))
        {
            return BadRequest("Invalid CategoryId.");
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
    }

    // GET: api/products - Retrieve all products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
    {
        return await _context.Products.Include(p => p.Category).ToListAsync();
    }

    // GET: api/products/{id} - Retrieve a product by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    // PUT: api/products/{id} - Update an existing product
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
    {
        if (id != updatedProduct.ProductId)
        {
            return BadRequest();
        }

        _context.Entry(updatedProduct).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(e => e.ProductId == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/products/{id} - Delete a product by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/products/{id}/stock/{stock} - Update stock of a product
    [HttpPut("{id}/stock/{stock}")]
    public async Task<IActionResult> UpdateProductStock(int id, int stock)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Stock = stock;
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/products/price/{price} - Retrieve products with price greater than a specified amount
    [HttpGet("price/{price}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPrice(decimal price)
    {
        return await _context.Products
            .Where(p => p.Price > price)
            .Include(p => p.Category)
            .ToListAsync();
    }

    // GET: api/products/category/{categoryId} - Retrieve products by category
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();
    }
}
