namespace SolBlog2.Web.Models
{

    public sealed class BlogPostVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Abstract { get; set; }
        public string Content { get; set; } = "";
        public string Slug { get; set; } = "";
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsPublished { get; set; }
    }
}
