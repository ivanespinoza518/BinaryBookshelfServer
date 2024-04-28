namespace BinaryBookshelfServer.Data.Models
{
    public class Book
    {
        /// <summary>
        /// The unique id and primary key for this Book
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Book title
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Book subtitle
        /// </summary>
        public string? Subtitle { get; set; }

        /// <summary>
        /// Book description
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Book Edition
        /// </summary>
        public int Edition { get; set; }

        /// <summary>
        /// Book's international standard book number (ISBN-13)
        /// </summary>
        public required string Isbn13 { get; set;}

        /// <summary>
        /// Book image url
        /// </summary>
        public required string ImageUrl { get; set; }

        /// <summary>
        /// Book average price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Author Id (foreign key)
        /// The author who wrote this book
        /// </summary>
        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        /// <summary>
        /// Category Id (foreign key)
        /// The category related to this book
        /// </summary>
        public int CategoryId { get; set;  }
        public Category? Category { get; set; }
    }
}
