using System.ComponentModel.DataAnnotations;
namespace SolBlog2.Web.Models
{
    public class BlogPostInput
    {
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Abstract { get; set; }

        public string Content { get; set; } = string.Empty;

        [Display(Name = "Published?")]
        public bool IsPublished { get; set; }
    }
}
