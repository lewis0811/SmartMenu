using AutoMapper;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<Transaction> GetAll(int? transactionId, int? deviceSubscriptionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.TransactionRepository.EnableQuery();
            var result = DataQuery(data, transactionId, deviceSubscriptionId, searchString, pageNumber, pageSize);
            return result;
        }

        public async Task<Transaction> AddTransaction(TransactionCreateDTO transactionCreateDTO)
        {
            var deviceSubscription = await _unitOfWork.DeviceSubscriptionRepository.FindObjectAsync(c => c.DeviceSubscriptionId == transactionCreateDTO.DeviceSubscriptionId && c.IsDeleted == false)
                ?? throw new Exception("Device subscription not found or deleted.");

            var data = _mapper.Map<Transaction>(transactionCreateDTO);
            data.Payment_Status = Payment_Status.Pending;

            _unitOfWork.TransactionRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public async Task<Transaction> Update(int transactionId, TransactionUpdateDTO transactionUpdateDTO)
        {
            var transaction = await _unitOfWork.TransactionRepository.FindObjectAsync(c => c.TransactionId == transactionId && c.IsDeleted == false)
                ?? throw new Exception("Transaction not found or deleted.");

            _mapper.Map(transactionUpdateDTO, transaction);

            _unitOfWork.TransactionRepository.Update(transaction);
            _unitOfWork.Save();

            return transaction;
        }

        public void Delete(int transactionId)
        {
            var data = _unitOfWork.TransactionRepository.Find(c => c.TransactionId == transactionId && c.IsDeleted == false).FirstOrDefault()
                            ?? throw new Exception("Transaction not found or deleted.");

            data.IsDeleted = true;

            _unitOfWork.TransactionRepository.Update(data);
            _unitOfWork.Save();
        }

        private static IEnumerable<Transaction> DataQuery(IQueryable<Transaction> data, int? transactionId, int? deviceSubscriptionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => !c.IsDeleted);

            if (transactionId != null)
            {
                data = data
                    .Where(c => c.TransactionId == transactionId);
            }

            if (deviceSubscriptionId != null)
            {
                data = data
                    .Where(c => c.DeviceSubscriptionId == deviceSubscriptionId);
            }

            if (searchString != null)
            {
                if (Enum.TryParse(typeof(PayType), searchString, out object? result))
                {
                    if (result != null)
                    {
                        data = data
                        .Where(c => c.PayType.Equals(result));
                    }
                    
                }

                if (Enum.TryParse(typeof(Payment_Status), searchString, out object? result2))
                {
                    if (result2 != null)
                    {
                        data = data
                            .Where(c => c.Payment_Status.Equals(result2));
                    }

                }
            }

            return PaginatedList<Transaction>.Create(data, pageNumber, pageSize);
        }
    }
}