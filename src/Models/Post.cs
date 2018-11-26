using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lrndrpub.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint PostId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Slug { get; set; }
        [Required]
        public string Content { get; set; }
        public string CoverImage { get; set; }

        [Required]
        public bool IsPublished { get; set; } = true;
        public bool CommentsOpen { get; set; } = false;
        public bool IsPage { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public uint CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public uint UpdatedBy { get; set; }
        public DateTime PublishedAt { get; set; }
        public uint PublishedBy { get; set; }
    }
}
