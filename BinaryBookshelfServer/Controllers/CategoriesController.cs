using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BinaryBookshelfServer.Data;
using BinaryBookshelfServer.Data.Dto;
using BinaryBookshelfServer.Data.Models;

namespace BinaryBookshelfServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ApplicationDbContext context) : ControllerBase
    {
        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<ApiResult<CategoryDTO>>> GetCategories(
            int pageIndex = 0,
            int pageSize = 10,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            return await ApiResult<CategoryDTO>.CreateAsync(
                context.Categories.AsNoTracking()
                    .Select(c => new CategoryDTO()
                    {
                        Id = c.Id,
                        Label = c.Label,
                        TotalBooks = c.Books!.Count
                    }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // GET: api/Categories/BooksOfCategory/5
        [HttpGet("BooksOfCategory/{id}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksOfCategory(int id)
        {
            return await context.Books.Where(b => b.CategoryId == id).ToListAsync();
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            context.Entry(category).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return context.Categories.Any(e => e.Id == id);
        }

        [HttpPost]
        [Route("IsDupeField")]
        public bool IsDupeField(
            int categoryId,
            string fieldName,
            string fieldValue)
        {
            switch (fieldName)
            {
                case "label":
                    return context.Categories.Any(
                    c => c.Label == fieldValue && c.Id != categoryId);
                default:
                    return false;
            }
        }
    }
}
