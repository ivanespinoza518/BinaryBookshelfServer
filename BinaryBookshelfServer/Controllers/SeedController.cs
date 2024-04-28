using System.Security;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BinaryBookshelfServer.Data;
using BinaryBookshelfServer.Data.Models;
using BinaryBookshelfServer.Data.Source;
using CsvHelper;
using CsvHelper.Configuration;

namespace BinaryBookshelfServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(ApplicationDbContext db, IHostEnvironment environment) : ControllerBase
    {
        private readonly string _pathName = Path.Combine(environment.ContentRootPath, "Data/Source/books.csv");

        /// <summary>
        /// POST: api/Seed
        /// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// </summary>
        [HttpPost("Author")]
        public async Task<ActionResult<Author>> SeedAuthor()
        {
            // Prevents non-development environments from running this method
            if (!environment.IsDevelopment())
                throw new SecurityException("Not allowed.");

            Dictionary<string, Author> authorsByName = db.Authors
                .AsNoTracking().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };

            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);

            List<BooksCsv>? records = csv.GetRecords<BooksCsv>().ToList();
            foreach (BooksCsv record in records)
            {
                if (authorsByName.ContainsKey(record.author))
                {
                    continue;
                }

                Author author = new()
                {
                    Name = record.author,
                    Background = record.background
                };
                await db.Authors.AddAsync(author);
                authorsByName.Add(record.author, author);
            }

            await db.SaveChangesAsync();

            return new JsonResult(authorsByName.Count);
        }

        [HttpPost("Category")]
        public async Task<ActionResult<Category>> SeedCategory()
        {
            // Prevents non-development environments from running this method
            if (!environment.IsDevelopment())
                throw new SecurityException("Not allowed.");

            Dictionary<string, Category> categoriesByName = db.Categories
                .AsNoTracking().ToDictionary(x => x.Label, StringComparer.OrdinalIgnoreCase);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };

            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);

            List<BooksCsv>? records = csv.GetRecords<BooksCsv>().ToList();
            foreach (BooksCsv record in records)
            {
                if (categoriesByName.ContainsKey(record.category))
                {
                    continue;
                }

                Category category = new()
                {
                    Label = record.category
                };
                await db.Categories.AddAsync(category);
                categoriesByName.Add(record.category, category);
            }

            await db.SaveChangesAsync();

            return new JsonResult(categoriesByName.Count);
        }

        [HttpPost("Book")]
        public async Task<ActionResult<Book>> SeedBook()
        {
            // Prevents non-development environments from running this method
            if (!environment.IsDevelopment())
                throw new SecurityException("Not allowed.");

            Dictionary<string, Author> authors = await db.Authors//.AsNoTracking()
                .ToDictionaryAsync(a => a.Name);

            Dictionary<string, Category> categories = await db.Categories
                .ToDictionaryAsync(c => c.Label);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };

            int bookCount = 0;
            using (StreamReader reader = new(_pathName))
            using (CsvReader csv = new(reader, config))
            {
                IEnumerable<BooksCsv>? records = csv.GetRecords<BooksCsv>();
                foreach (BooksCsv record in records)
                {
                    if (!authors.TryGetValue(record.author, out Author? author))
                    {
                        Console.WriteLine($"Author not found for {record.title}");
                        return NotFound(record);
                    }

                    if (!categories.TryGetValue(record.category, out Category? category))
                    {
                        Console.WriteLine($"Category not found for {record.title}");
                        return NotFound(record);
                    }

                    if (!record.edition.HasValue || !record.price.HasValue)
                    {
                        Console.WriteLine($"Skipping {record.title}");
                        continue;
                    }
                    Book book = new()
                    {
                        Title = record.title,
                        Subtitle = record.subtitle,
                        Description = record.description,
                        Edition = (int) record.edition,
                        Isbn13 = record.isbn_13,
                        ImageUrl = record.image_url,
                        Price = (decimal) record.price.Value,
                        AuthorId = author.Id,
                        CategoryId = category.Id,
                    };
                    db.Books.Add(book);
                    bookCount++;
                }
                await db.SaveChangesAsync();
            }
            return new JsonResult(bookCount);
        }
    }
}
