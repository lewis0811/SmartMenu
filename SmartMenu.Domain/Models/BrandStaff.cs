using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class BrandStaff : BaseModel
    {
        public int BrandStaffId { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public Guid UserId { get; set; }

        public int? StoreId { get; set; }

        [ForeignKey(nameof(BrandId))]
        public Brand? Brand { get; set; } //

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; } //
    }
}