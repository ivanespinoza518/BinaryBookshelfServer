using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BinaryBookshelfServer.Data.Models
{
    public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(book => book.Id);
            builder.Property<int>(book => book.Id) // Id
                .IsRequired()
                .HasColumnType("int");
            builder.Property<string>(book => book.Title) // Title
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");
            builder.Property<string?>(book => book.Subtitle) // Subtitle
                .IsUnicode(false)
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");
            builder.Property<string>(book => book.Description) // Description
                .IsRequired()
                .IsUnicode(false)
                .HasColumnType("varchar(max)");
            builder.Property<int>(book => book.Edition) // Edition
                .HasColumnType("int");
            builder.Property<string>(book => book.Isbn13) // Isbn-13
                .IsRequired()
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnType("varchar(14)");
            builder.Property<string>(book => book.ImageUrl) // Image Url
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnType("varchar(100)");
            builder.Property<decimal>(book => book.Price) // Price
                .HasColumnType("numeric(8, 2)");
            builder
                .HasOne(book => book.Author)
                .WithMany(author => author.Books)
                .HasForeignKey(book => book.AuthorId); // Author
            builder
                .HasOne(book => book.Category)
                .WithMany(category => category.Books)
                .HasForeignKey(book => book.CategoryId); // Category
            builder.HasIndex(book => book.Title);
            builder.HasIndex(book => book.Price);
        }
    }
}