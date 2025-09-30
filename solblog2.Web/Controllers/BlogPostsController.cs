using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolBlog2.Web.Models;
using SolBlog2.Domain.Models;
using SolBlog2.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolBlog2.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BlogPostsController> _log;

        public BlogPostsController(ApplicationDbContext context, ILogger<BlogPostsController> log)
        {
            _context = context;
            _log = log;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            return await _context.BlogPosts.ToListAsync();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(int id, [FromBody] BlogPostInput input)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            // Update allowed fields
            blogPost.Title = input.Title;
            blogPost.Abstract = input.Abstract ?? string.Empty;
            blogPost.Content = input.Content;
            blogPost.IsPublished = input.IsPublished;
            blogPost.MarkUpdated(User?.Identity?.Name ?? "system");

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]        
        public async Task<ActionResult<object>> PostBlogPost([FromBody] BlogPostInput input, [FromServices] ApplicationDbContext db)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var userId = User?.Identity?.Name ?? "system"; // replace with your real user id if available
            var entity = new BlogPost
            {
                Title = input.Title,
                Abstract = input.Abstract ?? "",
                Content = input.Content,
                Slug = ToSlug(input.Title),
                AuthorId = userId,
                IsPublished = input.IsPublished,
                IsDeleted = false
            };
            entity.Slug = await GenerateUniqueSlugAsync(input.Title, db);
            entity.MarkCreated(userId);

            _log.LogInformation("Saved: title='{Title}', abstract_len={LenA}, content_len={LenC}",
                entity.Title, (entity.Abstract ?? "").Length, (entity.Content ?? "").Length);
            db.BlogPosts.Add(entity);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBlogPost), new { id = entity.Id }, new { entity.Id, entity.Slug });
        }
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }


        private static string ToSlug(string s)
        {
            // normalize to lowercase letters/digits/hyphens; collapse spaces/hyphens
            var cleaned = new string(s.Trim().ToLowerInvariant()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : ' ')
                .ToArray());
            while (cleaned.Contains("  ")) cleaned = cleaned.Replace("  ", " ");
            return string.Join('-', cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        private async Task<string> GenerateUniqueSlugAsync(string title, ApplicationDbContext db, int? skipId = null)
        {
            var baseSlug = ToSlug(title);
            var slug = baseSlug;
            var i = 2;

            // if editing, ignore the current row
            bool exists = await db.BlogPosts
                .AnyAsync(p => p.Slug == slug && !p.IsDeleted && (skipId == null || p.Id != skipId.Value));

            while (exists)
            {
                slug = $"{baseSlug}-{i++}";
                exists = await db.BlogPosts
                    .AnyAsync(p => p.Slug == slug && !p.IsDeleted && (skipId == null || p.Id != skipId.Value));
            }

            return slug;
        }
    }
}
