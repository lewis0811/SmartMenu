using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class StoreDeviceCreateDTO
    {
        public int StoreID { get; set; }

        [Required]
        public string StoreDeviceName { get; set; } = string.Empty;

        public float DeviceWidth { get; set; } = 0;
        public float DeviceHeight { get; set; } = 0;

        public bool IsDisplay { get; set; } = false;
    }
}