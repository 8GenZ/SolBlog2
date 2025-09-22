using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SolBlog2.Domain.Models
{
    public class BlogPost
    {
        [Required, MaxLength(450)]
        public string AuthorId { get; set; } = default!;
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;       
        public string? Content { get; set; }
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        [MaxLength(450)]
        public string CreatedBy { get; private set; } = string.Empty;
        public DateTimeOffset? UpdatedAt { get; private set; }
        [MaxLength(450)]
        public string? UpdatedBy { get; private set; }
        public void MarkCreated(string userId)
        {
            CreatedAt = DateTimeOffset.UtcNow;
            CreatedBy = userId;
            AuthorId = userId;  
        }
        public void MarkUpdated(string userId)
        {
            UpdatedAt = DateTimeOffset.UtcNow;
            UpdatedBy = userId;
        }
        [Required]
        public string? Slug { get; set; }
        [Display(Name = "Published?")]
        public bool IsPublished { get; set; }
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }
    }
}
