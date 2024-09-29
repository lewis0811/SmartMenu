using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class BrandStaffService : IBrandStaffService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BrandStaffService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public BrandStaff Add(BrandStaffCreateDTO brandStaffCreateDTO)
        {
            var data = _mapper.Map<BrandStaff>(brandStaffCreateDTO);
            if (data.StoreId == 0) data.StoreId = null;

            var user = _unitOfWork.UserRepository.Find(c => c.UserId == brandStaffCreateDTO.UserId && !c.IsDeleted)
                .FirstOrDefault() ?? throw new Exception($"User id: {brandStaffCreateDTO.UserId} not exist.");

            var existUser = _unitOfWork.BrandStaffRepository.EnableQuery()
                .FirstOrDefault(c => c.UserId == brandStaffCreateDTO.UserId && !c.IsDeleted);
            if (existUser != null) throw new Exception($"User already define in brand id: {existUser.BrandId}");

            var existUser2 = _unitOfWork.BrandStaffRepository.EnableQuery()
                .FirstOrDefault(c => c.StoreId == brandStaffCreateDTO.StoreId && !c.IsDeleted);
            if (existUser2 != null) throw new Exception($"User already define in store id: {existUser2.StoreId}");

            _unitOfWork.BrandStaffRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int brandStaffId)
        {
            var data = _unitOfWork.BrandStaffRepository.Find(c => c.BrandStaffId == brandStaffId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Staff not found or deleted");

            var userId = data.UserId;

            _unitOfWork.BrandStaffRepository.Remove(data);
            _unitOfWork.Save();

            var user = _unitOfWork.UserRepository.Find(c => c.UserId == userId).FirstOrDefault() ?? throw new Exception("User not found or is deleted");
            _unitOfWork.UserRepository.Remove(user);
            _unitOfWork.Save();
        }

        public IEnumerable<BrandStaff> GetAll(int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BrandStaffRepository.EnableQuery()
                .Include(c => c.User).Where(c => c.User!.IsDeleted == false);
            var result = DataQuery(data, brandStaffId, brandId, userId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<BrandStaff>();
        }

        public BrandStaff Update(int brandStaffId, BrandStaffUpdateDTO brandStaffUpdateDTO)
        {
            var data = _unitOfWork.BrandStaffRepository.Find(c => c.BrandStaffId == brandStaffId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("BrandStaff not found or deleted");

            var user = _unitOfWork.UserRepository.Find(c => c.UserId == brandStaffUpdateDTO.UserId)
                .FirstOrDefault() ?? throw new Exception($"User id: {brandStaffUpdateDTO.UserId} not exist.");

            var store = _unitOfWork.StoreRepository.Find(c => c.StoreId == brandStaffUpdateDTO.StoreId)
                .FirstOrDefault() ?? throw new Exception($"Store id: {brandStaffUpdateDTO.StoreId} not exist.");

            _mapper.Map(brandStaffUpdateDTO, data);
            _unitOfWork.BrandStaffRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        private IEnumerable<BrandStaff> DataQuery(IQueryable<BrandStaff> data, int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (brandStaffId != null)
            {
                data = data
                    .Where(c => c.BrandStaffId == brandStaffId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandId == brandId);
            }
            if (userId != null)
            {
                data = data
                    .Where(c => c.UserId == userId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.User!.Email.Contains(searchString)
                    || c.User.UserName.Contains(searchString));
            }

            return PaginatedList<BrandStaff>.Create(data, pageNumber, pageSize);
        }
    }
}