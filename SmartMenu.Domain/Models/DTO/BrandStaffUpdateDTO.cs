using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BrandStaffUpdateDTO
    {
        [Required]
        public Guid UserId { get; set; }

        public int StoreId { get; set; }
    }
}