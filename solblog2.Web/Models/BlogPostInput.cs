using System.ComponentModel.DataAnnotations;
namespace solblog2.Web.Models
{
    public class BlogPostInput
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Abstract { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
