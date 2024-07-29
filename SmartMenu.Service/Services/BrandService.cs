using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Services
{
    public class BrandService : IBrandService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Brand Add(BrandCreateDTO brandCreateDTO)
        {

            var data = _mapper.Map<Brand>(brandCreateDTO);

            _unitOfWork.BrandRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int brandId)
        {
            var data = _unitOfWork.BrandRepository.Find(c => c.BrandId == brandId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Box not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.BrandRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<Brand> GetAll(int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BrandRepository.EnableQuery();
            var result = DataQuery(data, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Brand>();
        }

        public IEnumerable<Brand> GetBranchWithBrandStaff(int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BrandRepository.EnableQuery();
            data = data.Include(c => c.BrandStaffs);

            var result = DataQuery(data, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Brand>();
        }

        public IEnumerable<Brand> GetBranchWithStore(int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BrandRepository.EnableQuery();
            data = data.Include(c => c.Stores);

            var result = DataQuery(data, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Brand>();
        }

        public Brand Update(int brandId, BrandUpdateDTO brandUpdateDTO)
        {
            var data = _unitOfWork.BrandRepository.Find(c => c.BrandId == brandId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Brand not found or deleted");

            _mapper.Map(brandUpdateDTO, data);
            _unitOfWork.BrandRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private static IEnumerable<Brand> DataQuery(IQueryable<Brand> data, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(data => data.IsDeleted == false);

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandId == brandId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c =>
                        c.BrandName.Contains(searchString)
                        || c.BrandDescription!.Contains(searchString)
                        || c.BrandContactEmail.Contains(searchString));
            }

            return PaginatedList<Brand>.Create(data, pageNumber, pageSize);
        }
    }
}
