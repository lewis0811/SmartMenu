using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class StoreDeviceUpdateDTO
    {
        [Required]
        public string StoreDeviceName { get; set; } = string.Empty;

        [Required]
        public int DeviceWidth { get; set; }
        [Required]
        public int DeviceHeight { get; set; }

        public bool IsApproved { get; set; } = false;

        //[Required]
        //public DisplayType DisplayType { get; set; }

        //public bool IsDisplay { get; set; } = false;
    }
}