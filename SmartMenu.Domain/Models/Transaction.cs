using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Transaction : BaseModel
    {
        public int TransactionId { get; set; }
        public int DeviceSubscriptionId { get; set; }

        [Range(50000, double.MaxValue, ErrorMessage = "Amount must be greater than 50000")]
        public decimal Amount { get; set; }

        public Payment_Status Payment_Status { get; set; }
        public PayType PayType { get; set; }
        public DateTime PayDate { get; set; } = DateTime.Now;

        //[ForeignKey(nameof(DeviceSubscriptionId))]
        //public DeviceSubscription? DeviceSubscription { get; set; }
    }
}