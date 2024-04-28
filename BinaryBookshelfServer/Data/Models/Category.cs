namespace BinaryBookshelfServer.Data.Models
{
    public class Category
    {
        /// <summary>
        /// The unique id and primary key for this category
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Category label
        /// </summary>
        public required string Label { get; set; }

        /// <summary>
        /// A collection of all books with this category
        /// </summary>
        public ICollection<Book> Books { get; } = [];
    }
}
