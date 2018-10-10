using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lrndrpub.Models
{
    public enum SettingType
    {
        Core,
        Blog, 
        Theme
    }

    public class SettingsValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint SettingsId { get; set; }

        [Required]
        public string Key { get; set; }
        public string Value { get; set; }

        [Required]
        public SettingType Type { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public uint CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public uint UpdatedBy { get; set; }
        
    }
}
