using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.DAO.Implementation
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly SmartMenuDBContext _context;
        public TransactionRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }
    }
}
