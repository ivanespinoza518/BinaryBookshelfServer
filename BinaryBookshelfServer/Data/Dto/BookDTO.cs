namespace BinaryBookshelfServer.Data.Dto
{
    public class BookDTO
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Subtitle { get; set; }

        public required string Description { get; set; }

        public int Edition { get; set; }

        public required string Isbn13 { get; set; }

        public required string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int AuthorId { get; set; }

        public string? AuthorName { get; set; } = null!;

        public int CategoryId { get; set; }

        public string? CategoryLabel { get; set; } = null!;
    }
}
