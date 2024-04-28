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
    public class BooksController(ApplicationDbContext context) : ControllerBase
    {
        // GET: api/Books
        // GET: api/Books?pageIndex=0&pageSize=10
        // GET: api/Books?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
        [HttpGet]
        public async Task<ActionResult<ApiResult<BookDTO>>> GetBooks(
            int pageIndex = 0,
            int pageSize = 10,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            return await ApiResult<BookDTO>.CreateAsync(
                context.Books.AsNoTracking()
                    .Select(b => new BookDTO()
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Subtitle = b.Subtitle,
                        Description = b.Description,
                        Edition = b.Edition,
                        Isbn13 = b.Isbn13,
                        ImageUrl = b.ImageUrl,
                        Price = b.Price,
                        AuthorId = b.Author!.Id,
                        AuthorName = b.Author!.Name,
                        CategoryId = b.Category!.Id,
                        CategoryLabel = b.Category!.Label
                    }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            context.Entry(book).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            context.Books.Add(book);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            context.Books.Remove(book);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return context.Books.Any(e => e.Id == id);
        }

        [HttpPost]
        [Route("IsDupeBook")]
        public bool IsDupeBook(Book book)
        {
            return context.Books.AsNoTracking().Any(
                e => e.Title == book.Title
                && e.Subtitle == book.Subtitle
                && e.Edition == book.Edition
                && e.Isbn13 == book.Isbn13
                && e.AuthorId == book.AuthorId
                && e.CategoryId == book.CategoryId
                && e.Id != book.Id);
        }
    }
}
