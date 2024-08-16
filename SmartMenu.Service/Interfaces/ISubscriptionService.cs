using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> AddSubscription(SubscriptionCreateDTO subscriptionCreateDTO);
        void DeleteSubscription(int subscriptionId);
        IEnumerable<Subscription> GetAll(int? subscriptionId, string? searchString, int pageNumber, int pageSize);
        Task<Subscription> UpdateSubscription(int subscriptionId, SubscriptionUpdateDTO subscriptionUpdateDTO);
    }
}