using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class MenuUpdateDTO
    {

        [Required]
        public string MenuName { get; set; } = string.Empty;

        public string MenuDescription { get; set; } = string.Empty;

    }
}
