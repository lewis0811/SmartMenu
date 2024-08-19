using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class DeviceSubscription : BaseModel
    {
        public int DeviceSubscriptionId { get; set; }

        public int StoreDeviceId { get; set; }
        //public int SubscriptionId { get; set; }

        public DateTime SubscriptionStartDate { get; set; }

        [Compare(nameof(SubscriptionStartDate), ErrorMessage = "End date must be after start date")]
        public DateTime SubscriptionEndDate { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; }

        [ForeignKey(nameof(StoreDeviceId))]
        public StoreDevice? StoreDevice { get; set; }

        //[ForeignKey(nameof(SubscriptionId))]
        //public Subscription? Subscription { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }
}