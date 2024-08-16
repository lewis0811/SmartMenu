using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class DeviceSubscriptionUpdateDTO
    {
        public DateTime SubscriptionStartDate { get; set; }

        [Compare(nameof(SubscriptionStartDate), ErrorMessage = "End date must be after start date")]
        public DateTime SubscriptionEndDate { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; }
    }
}