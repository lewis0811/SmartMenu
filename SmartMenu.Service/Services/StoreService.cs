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
    public class StoreService : IStoreService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Store Add(StoreCreateDTO storeCreateDTO)
        {
            var brand = _unitOfWork.BrandRepository
                .EnableQuery()
                .Include(c => c.Stores!.Where(d => !d.IsDeleted))
                .Where(c => c.BrandId == storeCreateDTO.BrandID && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Brand not found or deleted");

            var data = _mapper.Map<Store>(storeCreateDTO);
            data.StoreCode = InitializeStoreCode(brand);

            _unitOfWork.StoreRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        private static string InitializeStoreCode(Brand brand)
        {
            string tempCode = "";

            var words = brand.BrandName.Split(' ');
            if (words.Length > 0)
            {
                foreach (var word in words)
                {
                    tempCode += word.Take(1).FirstOrDefault().ToString().ToUpper();
                }
            }
            else
            {
                tempCode = brand.BrandName.Take(1).FirstOrDefault().ToString().ToUpper();
            }

            var nextStoreNumber = brand.Stores!.Count + 1;
            return tempCode + nextStoreNumber.ToString("D3");
        }

        public void Delete(int storeId)
        {
            var data = _unitOfWork.StoreRepository.Find(c => c.StoreId == storeId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Store not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.StoreRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<Store> GetAll(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreRepository.EnableQuery();
            var result = DataQuery(data, storeId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Store>();
        }

        public Store GetStoreWithStaffs(int storeId, Guid userId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreRepository.EnableQuery()
                .Include(c => c.Staffs)
                .Where(s => s.Staffs.Any(staff => !staff.IsDeleted && staff.UserId == userId))
                .FirstOrDefault() ?? throw new Exception("Store not found or deleted");

            return data;
        }

        public IEnumerable<Store> GetStoreWithMenus(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreRepository.EnableQuery();
            data = data.Include(c => c.StoreMenus);

            var result = DataQuery(data, storeId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Store>();
        }
        public IEnumerable<Store> GetStoreWithCollections(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreRepository.EnableQuery();
            data = data.Include(c => c.StoreCollections);

            var result = DataQuery(data, storeId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Store>();
        }

        public Store Update(int storeId, StoreUpdateDTO storeUpdateDTO)
        {
            var data = _unitOfWork.StoreRepository.Find(c => c.StoreId == storeId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store not found or deleted");

            _mapper.Map(storeUpdateDTO, data);
            _unitOfWork.StoreRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private static IEnumerable<Store> DataQuery(IQueryable<Store> data, int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(data => data.IsDeleted == false);

            if (storeId != null)
            {
                data = data
                    .Where(c => c.StoreId == storeId);
            }

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
                        c.StoreLocation.Contains(searchString)
                    || c.StoreCode.Contains(searchString)
                    || c.StoreContactEmail.Contains(searchString)
                    || c.StoreContactNumber.Contains(searchString));
            }

            return PaginatedList<Store>.Create(data, pageNumber, pageSize);
        }

    }
}
