using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.DAO.Implementation
{
    public class DeviceSubscriptionRepository : GenericRepository<DeviceSubscription>, IDeviceSubscriptionRepository
    {
        private readonly SmartMenuDBContext _context;
        public DeviceSubscriptionRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }
    }
}
