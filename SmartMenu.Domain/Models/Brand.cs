using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class Brand : BaseModel
    {
        public int BrandId { get; set; }

        [Required]
        public string BrandName { get; set; } = string.Empty;

        public string? BrandDescription { get; set; }
        public string? BrandImage { get; set; }

        [EmailAddress]
        [Required]
        public string BrandContactEmail { get; set; } = string.Empty;

        public ICollection<BrandStaff> BrandStaffs { get; set; } = new List<BrandStaff>();
        public ICollection<Store>? Stores { get; set; } = new List<Store>();
        public ICollection<Category>? Categories { get; set; } = new List<Category>();
    }
}