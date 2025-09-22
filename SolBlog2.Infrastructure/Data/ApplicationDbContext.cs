using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <-- important
using Microsoft.EntityFrameworkCore;
using SolBlog2.Domain.Models;
using SolBlog2.Infrastructure.Auth;
using SolBlog2.Infrastructure.Data.Configurations;

namespace SolBlog2.Infrastructure.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);                 // keep Identity tables
            modelBuilder.ApplyConfiguration(new BlogPostConfiguration()); // your fluent mapping
        }
    }
}
