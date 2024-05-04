using System.Security;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
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
    public class SeedController(
        ApplicationDbContext db, 
        RoleManager<IdentityRole> roleManager,
        UserManager<BinaryBookshelfUser> userManager,
        IHostEnvironment environment, 
        IConfiguration configuration) 
        : ControllerBase
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
            {
                throw new SecurityException("Not allowed.");
            }

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
            {
                throw new SecurityException("Not allowed.");
            }

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
            {
                throw new SecurityException("Not allowed.");
            }

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

        [HttpPost("User")]
        public async Task<ActionResult> SeedUsers()
        {
            // Prevents non-development environments from running this method
            if (!environment.IsDevelopment())
            {
                throw new SecurityException("Not allowed.");
            }

            // setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            // create the default roles (if they don't exist yet)
            if (await roleManager.FindByNameAsync(role_RegisteredUser) is null)
            {
                await roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
            }
            if (await roleManager.FindByNameAsync(role_Administrator) is null)
            {
                await roleManager.CreateAsync(new IdentityRole(role_Administrator));
            }

            // create a list to track the newly added users
            List<BinaryBookshelfUser> addedUserList = [];

            // create a new admin user account
            (string name_Admin, string email_Admin) = ("admin", "admin@email.com");
            BinaryBookshelfUser user_Admin = new()
            {
                UserName = name_Admin,
                Email = email_Admin,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            // check if the admin name already exists
            if (await userManager.FindByNameAsync(name_Admin) is not null)
            {
                user_Admin.UserName = "admin2";
                user_Admin.Email = "admin2@email.com";
            }
            // insert the admin user into the DB
            _ = await userManager.CreateAsync(user_Admin, configuration["DefaultPasswords:Administrator"])
                ?? throw new InvalidOperationException();

            // assign the "RegisteredUser" and "Administrator" roles
            await userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
            await userManager.AddToRoleAsync(user_Admin, role_Administrator);

            // confirm the e-mail and remove lockout
            user_Admin.EmailConfirmed = true;
            user_Admin.LockoutEnabled = false;

            // add the admin user to the added users list
            addedUserList.Add(user_Admin);

            // create a new standard user account
            (string name, string email) = ("user", "user@email.com");
            BinaryBookshelfUser user = new()
            {
                UserName = name,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            if (await userManager.FindByNameAsync(name) is not null)
            {
                user.UserName = "user2";
                user.Email = "user2@email.com";
            }
            _ = await userManager.CreateAsync(user, configuration["DefaultPasswords:RegisteredUser"])
                ?? throw new InvalidOperationException();
            await userManager.AddToRoleAsync(user, role_RegisteredUser);
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            addedUserList.Add(user);

            // if we added at least one user, persist the changes into the DB
            if (addedUserList.Count > 0)
            {
                await db.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                Count = addedUserList.Count,
                Users = addedUserList
            });
        }
    }
}
