namespace BinaryBookshelfServer.Data.Dto
{
    public class AuthorDTO
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Background { get; set; }

        public int? TotalBooks { get; set; } = null!;
    }
}
