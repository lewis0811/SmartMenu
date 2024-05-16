using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IBrandRepository BrandRepository { get; }
        IBrandStaffRepository BrandStaffRepository { get; }
        IUserRepository UserRepository { get; }
        IStoreRepository StoreRepository { get; }
        IRoleRepository RoleRepository { get; }

        int Save();
    }
}
