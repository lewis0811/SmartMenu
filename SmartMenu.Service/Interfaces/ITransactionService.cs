using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> AddTransaction(TransactionCreateDTO transactionCreateDTO);
        void Delete(int transactionId);
        IEnumerable<Transaction> GetAll(int? transactionId, int? deviceSubscriptionId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Transaction> GetByBrand(int brandId);
        Task<Transaction> Update(int transactionId, TransactionUpdateDTO transactionUpdateDTO);
    }
}
