using AutoMapper;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            _unitOfWork.BrandStaffRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int brandStaffId)
        {
            var data = _unitOfWork.BrandStaffRepository.Find(c => c.BrandStaffId == brandStaffId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Staff not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.BrandStaffRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<BrandStaff> GetAll(int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BrandStaffRepository.EnableQuery();
            var result = DataQuery(data, brandStaffId, brandId, userId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<BrandStaff>();
        }

        public BrandStaff Update(int brandStaffId, BrandStaffCreateDTO brandStaffCreateDTO)
        {
            var data = _unitOfWork.BrandStaffRepository.Find(c => c.BrandStaffId == brandStaffId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Staff not found or deleted");

            _mapper.Map(brandStaffCreateDTO, data);
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


            return PaginatedList<BrandStaff>.Create(data, pageNumber, pageSize);
        }
    }
}
