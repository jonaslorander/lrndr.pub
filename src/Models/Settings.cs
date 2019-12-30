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
