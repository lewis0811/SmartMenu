using AutoMapper;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IBoxService
    {
        IEnumerable<Box> GetAll(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Box> GetAllWithBoxItems(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize);
        Box Add(BoxCreateDTO boxCreateDTO);
        Box Update(int boxId, BoxUpdateDTO boxUpdateDTO);
        void Delete(int boxId);
    }
}
