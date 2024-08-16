using SmartMenu.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class TransactionCreateDTO
    {
        public int DeviceSubscriptionId { get; set; }

        [Range(50000, double.MaxValue, ErrorMessage = "Amount must be greater than 50000")]
        public decimal Amount { get; set; }

        //public DateTime Payment_Date { get; set; }
        public PayType PayType { get; set; }
        //public DateTime PayDate { get; set; }
    }
}