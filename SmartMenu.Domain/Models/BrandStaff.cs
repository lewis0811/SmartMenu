using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class BrandStaff : BaseModel
    {
        public int BrandStaffID { get; set; }
        [Required]
        public int BrandID { get; set; }
        [Required]
        public Guid UserID { get; set; }

        //[ForeignKey("BrandID")]
        //public Brand? Brand { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}