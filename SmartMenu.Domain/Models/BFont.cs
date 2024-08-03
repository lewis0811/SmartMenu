using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class BFont : BaseModel
    {
        public int BFontId { get; set; }
        [Required]
        public string FontName { get; set; } = string.Empty;
        [Required]
        public string FontPath { get; set; } = string.Empty;


    }
}
