using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.DB;
using ProductCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ProductCatalogContext _context;

    public CategoriesController(ProductCatalogContext context)
    {
        _context = context;
    }

    // POST: api/categories - Create a new category
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
    }

    // GET: api/categories - Retrieve all categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    // GET: api/categories/{id} - Retrieve a category by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategoryById(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }

    // PUT: api/categories/{id} - Update an existing category
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category updatedCategory)
    {
        if (id != updatedCategory.CategoryId)
        {
            return BadRequest();
        }

        _context.Entry(updatedCategory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Categories.Any(e => e.CategoryId == id))
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

    // DELETE: api/categories/{id} - Delete a category by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
