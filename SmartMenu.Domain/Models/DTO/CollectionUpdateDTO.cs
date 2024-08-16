using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class CollectionUpdateDTO
    {
        [Required]
        public string CollectionName { get; set; } = string.Empty;

        public string CollectionDescription { get; set; } = string.Empty;
    }
}