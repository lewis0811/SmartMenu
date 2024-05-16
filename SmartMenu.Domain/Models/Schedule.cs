using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models
{
    public class Schedule : BaseModel
    {
        public int ScheduleID { get; set; }
        public double DurationByHour { get; set; }
    }
}
