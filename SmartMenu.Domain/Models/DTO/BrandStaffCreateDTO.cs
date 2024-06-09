using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BrandStaffCreateDTO
    {
        public int BrandID { get; set; }
        [Required]
        public Guid UserID { get; set; }
    }
}
