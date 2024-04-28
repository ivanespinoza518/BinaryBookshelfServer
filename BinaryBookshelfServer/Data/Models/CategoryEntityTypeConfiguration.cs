using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BinaryBookshelfServer.Data.Models
{
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(category => category.Id);
            builder.Property<int>(category => category.Id)
                .IsRequired()
                .HasColumnType("int");
            builder.Property<string>(category => category.Label)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnType("varchar(50)");
            builder.HasIndex(category => category.Label);
        }
    }
}
