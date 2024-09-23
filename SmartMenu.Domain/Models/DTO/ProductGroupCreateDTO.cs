using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductGroupCreateDTO
    {
        public int? MenuID { get; set; }
        public int? CollectionID { get; set; }

        [Required]
        public string ProductGroupName { get; set; } = string.Empty;


        public bool HaveNormalPrice { get; set; }
    }
}
