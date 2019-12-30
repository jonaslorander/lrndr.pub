using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lrndrpub.Models
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint AuthorId { get; set; }

        [StringLength(160)]
        public string Fullname { get; set; }

        [StringLength(160)]
        public string UserName { get; set; }

        [StringLength(160)]
        public string IdentityId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public bool IsAdmin { get; set;  }
        public string Slug { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CreatedAt { get; set; }
        public uint CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime UpdatedAt { get; set; }
        public uint UpdatedBy { get; set; }
    }
}
