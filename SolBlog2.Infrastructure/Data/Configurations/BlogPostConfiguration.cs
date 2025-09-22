using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolBlog2.Domain.Models;


namespace SolBlog2.Infrastructure.Data.Configurations
{
    public sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> b)
        {
            b.ToTable("BlogPosts");

            b.HasKey(x => x.Id);

            // AuthorId & auditing (string keys; 450 aligns with ASP.NET Identity default)
            b.Property(x => x.AuthorId)
                .IsRequired()
                .HasMaxLength(450);

            b.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            b.Property(x => x.UpdatedBy)
                .HasMaxLength(450);

            // Core content
            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Abstract)
                .HasMaxLength(500);

            b.Property(x => x.Content)
                .IsRequired();

            b.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(200);

            b.HasIndex(x => x.Slug).IsUnique();

            // State flags
            b.Property(x => x.IsPublished).HasDefaultValue(false);
            b.Property(x => x.IsDeleted).HasDefaultValue(false);

            // ---- DateTimeOffset with SQLite ----
            // SQLite doesn’t have a native DateTimeOffset type. Use converters.
            var dtoToString = new ValueConverter<DateTimeOffset, string>(
                v => v.UtcDateTime.ToString("O"), // store as ISO 8601 UTC
                v => DateTimeOffset.Parse(v, null, System.Globalization.DateTimeStyles.RoundtripKind)
            );

            var nullableDtoToString = new ValueConverter<DateTimeOffset?, string?>(
                v => v.HasValue ? v.Value.UtcDateTime.ToString("O") : null,
                v => string.IsNullOrEmpty(v)
                        ? (DateTimeOffset?)null
                        : DateTimeOffset.Parse(v!, null, System.Globalization.DateTimeStyles.RoundtripKind)
            );

            b.Property(x => x.CreatedAt)
                .IsRequired()
                .HasConversion(dtoToString);

            b.Property(x => x.UpdatedAt)
                .HasConversion(nullableDtoToString);
        }
    }
}
