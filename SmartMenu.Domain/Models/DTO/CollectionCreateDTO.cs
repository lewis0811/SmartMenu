using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class CollectionCreateDTO
    {
        public int BrandID { get; set; }

        [Required]
        public string CollectionName { get; set; } = string.Empty;

        public string CollectionDescription { get; set; } = string.Empty;
        public string? CollectionBackgroundImgPath { get; set; }
    }
}
