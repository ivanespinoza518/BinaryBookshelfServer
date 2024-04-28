namespace BinaryBookshelfServer.Data.Source
{
    public class BooksCsv
    {
        public string title { get; set; } = null!;
        public string? subtitle { get; set; }
        public string description { get; set; } = null!;
        public int? edition { get; set; }
        public string isbn_13 { get; set; } = null!;
        public string image_url { get; set; } = null!;
        public decimal? price { get; set; }
        public string author { get; set; } = null!;
        public string? background { get; set; }
        public string category { get; set; } = null!;
    }
}
