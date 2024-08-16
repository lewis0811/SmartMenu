using AutoMapper;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Subscription> AddSubscription(SubscriptionCreateDTO subscriptionCreateDTO)
        {
            var existSubscription = await _unitOfWork.SubscriptionRepository.FindObjectAsync(c => c.Name.Equals(subscriptionCreateDTO.Name));
            if (existSubscription != null) throw new Exception($"Subscription name: `{subscriptionCreateDTO.Name}` already exist!");

            var subscription = _mapper.Map<Subscription>(subscriptionCreateDTO);
            _unitOfWork.SubscriptionRepository.Add(subscription);
            _unitOfWork.Save();

            return subscription;
        }

        public async Task<Subscription> UpdateSubscription(int subscriptionId, SubscriptionUpdateDTO subscriptionUpdateDTO)
        {
            var existSubscription = await _unitOfWork.SubscriptionRepository.FindObjectAsync(c => c.SubscriptionId.Equals(subscriptionId))
                ?? throw new Exception("Subscription not found");

            _mapper.Map(subscriptionUpdateDTO, existSubscription);
            _unitOfWork.SubscriptionRepository.Update(existSubscription);
            _unitOfWork.Save();

            return existSubscription;
        }

        public  void DeleteSubscription(int subscriptionId)
        {
            var existSubscription = _unitOfWork.SubscriptionRepository.Find(c => c.SubscriptionId.Equals(subscriptionId)).FirstOrDefault()
                ?? throw new Exception("Subscription not found");

            existSubscription.IsDeleted = true;
            _unitOfWork.SubscriptionRepository.Update(existSubscription);
            _unitOfWork.Save();
        }

        public IEnumerable<Subscription> GetAll(int? subscriptionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.SubscriptionRepository.EnableQuery();
            var result = DataQuery(data, subscriptionId, searchString, pageNumber, pageSize);

            return result;
        }

        private static IEnumerable<Subscription> DataQuery(IQueryable<Subscription> data, int? subscriptionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (subscriptionId != null)
            {
                data = data.Where(c => c.SubscriptionId == subscriptionId);
            }

            if (searchString != null)
            {
                if (int.TryParse(searchString, out int result))
                {
                    data = data.Where(c => c.DayDuration.Equals(result));

                    return PaginatedList<Subscription>.Create(data, pageNumber, pageSize);
                }

                if (decimal.TryParse(searchString, out decimal result2))
                {
                    data = data.Where(c => c.Price.Equals(result2));

                    return PaginatedList<Subscription>.Create(data, pageNumber, pageSize);
                }

                data = data.Where(c => c.Name.Contains(searchString)
                || c.Description!.Contains(searchString));
                
            }

            return PaginatedList<Subscription>.Create(data, pageNumber, pageSize);
        }


    }
}