using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class MenuCreateDTO
    {
        public int BrandId { get; set; }

        [Required]
        public string MenuName { get; set; } = string.Empty;

        public string MenuDescription { get; set; } = string.Empty;
        //public string? MenuBackgroundImgPath { get; set; }
    }
}
