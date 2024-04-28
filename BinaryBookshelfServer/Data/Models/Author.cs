namespace BinaryBookshelfServer.Data.Models
{
    public class Author
    {
        /// <summary>
        /// The unique id and primary key for this author
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Author's name
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Author's background information
        /// </summary>
        public string? Background { get; set; }

        /// <summary>
        /// A collection of all books written by author
        /// </summary>
        public ICollection<Book> Books { get; } = [];
    }
}
