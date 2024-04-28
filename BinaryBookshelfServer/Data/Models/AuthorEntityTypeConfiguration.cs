using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BinaryBookshelfServer.Data.Models
{
    public class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");
            builder.HasKey(author => author.Id);
            builder.Property<int>(author => author.Id) // Id
                .IsRequired()
                .HasColumnType("int");
            builder.Property<string>(author => author.Name) // Name
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");
            builder.Property<string?>(author => author.Background) // Background
                .IsUnicode(false)
                .HasColumnType("varchar(max)");
            builder.HasIndex(author => author.Name);
        }
    }
}