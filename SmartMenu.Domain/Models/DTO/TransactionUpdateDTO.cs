using SmartMenu.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class TransactionUpdateDTO
    {
        //public DateTime Payment_Date { get; set; }
        public Payment_Status Payment_Status { get; set; }
        public PayType PayType { get; set; }
        //public DateTime PayDate { get; set; }
    }
}