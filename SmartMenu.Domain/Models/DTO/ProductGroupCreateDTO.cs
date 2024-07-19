using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductGroupCreateDTO
    {
        public int? MenuID { get; set; }
        public int? CollectionID { get; set; } 

        [Required]
        public string ProductGroupName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ProductGroupMaxCapacity { get; set; }

        public bool HaveNormalPrice { get; set; }
    }
}
