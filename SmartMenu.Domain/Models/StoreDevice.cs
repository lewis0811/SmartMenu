using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class StoreDevice : BaseModel
    {
        public int StoreDeviceID { get; set; }
        public int StoreID { get; set; }
        public int DisplayID { get; set; }

        [Required]
        public string StoreDeviceName { get; set; } = string.Empty;

        [Required]
        public DisplayType DisplayType { get; set; }

        public bool IsDisplay { get; set; } = false;

        [ForeignKey(nameof(StoreID))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(DisplayID))]
        public Display? Display { get; set; }
    }
}