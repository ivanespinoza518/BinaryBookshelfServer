namespace BinaryBookshelfServer.Data.Dto
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        public required string Label { get; set; }

        public int? TotalBooks { get; set; } = null!;
    }
}
