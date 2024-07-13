﻿using AutoMapper;
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
            var layer = _unitOfWork.BrandRepository
                .Find(c => c.BrandId == storeCreateDTO.BrandID && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Brand not found or deleted");

            var data = _mapper.Map<Store>(storeCreateDTO);

            _unitOfWork.StoreRepository.Add(data);
            _unitOfWork.Save();

            return data;
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

        public IEnumerable<Store> GetStoreWithStaffs(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreRepository.EnableQuery();
            data = data.Include(c => c.Staffs);

            var result = DataQuery(data, storeId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Store>();
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
                        c.StoreLocation.ToString().Contains(searchString)
                    || c.StoreContactEmail.ToString().Contains(searchString)
                    || c.StoreContactNumber.ToString().Contains(searchString));
            }

            return PaginatedList<Store>.Create(data, pageNumber, pageSize);
        }
    }
}
