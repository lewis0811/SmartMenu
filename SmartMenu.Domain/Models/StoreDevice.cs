using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class StoreDevice : BaseModel
    {
        public int StoreDeviceId { get; set; }
        public int StoreId { get; set; }

        [Required]
        public string StoreDeviceName { get; set; } = string.Empty;

        public float DeviceWidth { get; set; } = 0;
        public float DeviceHeight { get; set; } = 0;

        //[ForeignKey(nameof(StoreId))]
        //public Store? Store { get; set; }

        public ICollection<Display>? Displays { get; set; }

    }
}