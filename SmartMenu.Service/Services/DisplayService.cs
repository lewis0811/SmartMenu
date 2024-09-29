using AutoMapper;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing;

using Font = System.Drawing.Font;
using System.Text.Json;
using SmartMenu.DAO.Implementation;
using System.Net;
using System.Linq.Expressions;
using CloudinaryDotNet.Actions;
using System.IO;
using Microsoft.Extensions.Configuration;
using static System.Formats.Asn1.AsnWriter;
using System.Globalization;
#pragma warning disable CA1416 // Validate platform compatibility

namespace SmartMenu.Service.Services
{
    public class DisplayService : IDisplayService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;

        public DisplayService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cloudinary = new(configuration.GetSection("Cloudinary:CLOUDINARY_URL").Value);
        }

        // GET
        public IEnumerable<Display> GetAll(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.DisplayRepository.EnableQuery()
                .Include(c => c.Menu)
                .Include(c => c.Collection);

            var result = DataQuery(data, displayId, menuId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Display>();
        }

        public IEnumerable<Display> GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.DisplayRepository.EnableQuery()
                .Include(c => c.Menu)
                    .ThenInclude(c => c!.ProductGroups!.Where(d => d.IsDeleted == false))
                .Include(c => c.Collection)

                .Include(c => c.DisplayItems.Where(d => d.IsDeleted == false))!
                    .ThenInclude(c => c.ProductGroup)
                        .ThenInclude(c => c!.ProductGroupItems)!
                            .ThenInclude(c => c!.Product)
                                .ThenInclude(c => c!.ProductSizePrices)
                .Include(c => c.DisplayItems.Where(d => d.IsDeleted == false))!
                    .ThenInclude(c => c.Box)
                        .ThenInclude(c => c!.BoxItems);

            var result = DataQuery(data, displayId, menuId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Display>();
        }

        public async Task<Display> GetByDeviceId(int? deviceId)
        {
            var device = await _unitOfWork.StoreDeviceRepository
                .EnableQuery()
                    .Include(c => c.Displays!.Where(d => !d.IsDeleted))
                .FirstOrDefaultAsync(c => c.StoreDeviceId == deviceId && !c.IsDeleted)
                ?? throw new Exception("Device not found or deleted");

            if (device.Displays!.Count == 0) throw new Exception("Device has no display");

            var hourNow = DateTime.Now.Hour;
            float minute = DateTime.Now.Minute;
            float floatHour = hourNow + (float)(minute / 60);

            var store = await _unitOfWork.StoreRepository.EnableQuery()
                .Include(c => c.StoreProducts!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Product!)
                        .ThenInclude(c => c.ProductSizePrices)
                .FirstOrDefaultAsync(c => c.StoreId == device.StoreId) ?? throw new Exception("Store not found");

            Display unchageDisplay = new();

            unchageDisplay = await _unitOfWork.DisplayRepository.EnableQuery()
                .Include(c => c.Template)
                .Where(c => c.StoreDeviceId == device.StoreDeviceId && !c.IsDeleted && c.ActiveHour < floatHour)
                .OrderByDescending(c => c.ActiveHour)
                .FirstOrDefaultAsync()! ?? new Display();

            if (unchageDisplay.Template == null)
            {
                unchageDisplay = await _unitOfWork.DisplayRepository
                    .EnableQuery()
                    .Include(c => c.Template)
                    .Where(c => c.StoreDeviceId == device.StoreDeviceId && c.IsDeleted == false && c.ActiveHour > floatHour)
                    .OrderBy(c => c.ActiveHour)
                    .FirstOrDefaultAsync()! ?? throw new Exception("Fail to get display");
            }

            return unchageDisplay;
        }

        public IEnumerable<Display> GetWithItemsV2(int storeId, int? deviceId, int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreRepository.EnableQuery()
                .Include(c => c.StoreDevices)
                    .ThenInclude(c => c.Displays!)
                        .ThenInclude(c => c.Menu)

                .Include(c => c.StoreDevices)
                    .ThenInclude(c => c.Displays!)
                        .ThenInclude(c => c.Collection)
                .Where(c => c.StoreId == storeId && !c.IsDeleted)
                .SelectMany(c => c.StoreDevices!)
                    .SelectMany(c => c.Displays!);

            if (deviceId != null)
            {
                data = data.Where(c => c.StoreDeviceId == deviceId && !c.IsDeleted);
            }

            return DataQuery(data, displayId, menuId, collectionId, searchString, pageNumber, pageSize);
        }

        // GET IMAGE

        public async Task<string> GetImageByTimeAsync(int deviceId)
        {
            var device = await _unitOfWork.StoreDeviceRepository
                .EnableQuery()
                    .Include(c => c.Displays!.Where(d => !d.IsDeleted))
                .FirstOrDefaultAsync(c => c.StoreDeviceId == deviceId && !c.IsDeleted)
                ?? throw new Exception("Device not found or deleted");

            if (device.Displays!.Count == 0) throw new Exception("Device has no display");

            var hourNow = DateTime.Now.Hour;
            float minute = DateTime.Now.Minute;
            float floatHour = hourNow + (float)(minute / 60);

            Display unchageDisplay = new Display();
            Display display = new();

            unchageDisplay = _unitOfWork.DisplayRepository.EnableQuery()
                .Where(c => c.StoreDeviceId == device.StoreDeviceId && !c.IsDeleted && c.ActiveHour < floatHour)
                .OrderByDescending(c => c.ActiveHour)
                .FirstOrDefault()!;

            unchageDisplay ??= _unitOfWork.DisplayRepository
                    .EnableQuery()
                    .Where(c => c.StoreDeviceId == device.StoreDeviceId && c.IsDeleted == false && c.ActiveHour > floatHour)
                    .OrderBy(c => c.ActiveHour)
                    .FirstOrDefault()! ?? throw new Exception("Fail to get display");

            if (unchageDisplay.IsChanged == false && unchageDisplay.DisplayImgPath != null)
            {
                return unchageDisplay.DisplayImgPath;
            }

            var store = await _unitOfWork.StoreRepository.EnableQuery()
                .Include(c => c.StoreProducts!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Product!)
                        .ThenInclude(c => c.ProductSizePrices)
                .FirstOrDefaultAsync(c => c.StoreId == device.StoreId) ?? throw new Exception("Store not found");




            display = await _unitOfWork.DisplayRepository
                .EnableQuery()
                .Where(c => c.StoreDeviceId == device.StoreDeviceId && !c.IsDeleted && c.ActiveHour < floatHour)
                .Include(c => c.Menu)
                .Include(c => c.Collection!)
                .Include(c => c.Template!)
                    .ThenInclude(c => c.Layers!)
                        .ThenInclude(c => c.LayerItem!)

                .Include(c => c.Template!)
                    .ThenInclude(c => c.Layers!)
                        .ThenInclude(c => c.Boxes!)
                            .ThenInclude(c => c.BoxItems!)
                                .ThenInclude(c => c.BFont)

                .Include(c => c.DisplayItems!)
                    .ThenInclude(c => c.ProductGroup!)
                        .ThenInclude(c => c.ProductGroupItems!)
                            .ThenInclude(c => c.Product)
                                .ThenInclude(c => c!.ProductSizePrices)

                .OrderByDescending(c => c.ActiveHour)
                .FirstOrDefaultAsync()! ?? new Display();

            // If null then mean there's no display that have activeHour < recent hour
            if (display.Template == null)
            {
                display = await _unitOfWork.DisplayRepository
                    .EnableQuery()
                    .Where(c => c.StoreDeviceId == device.StoreDeviceId && c.IsDeleted == false && c.ActiveHour > floatHour)
                    .Include(c => c.Menu!)
                    .Include(c => c.Collection!)

                    .Include(c => c.Template!)
                        .ThenInclude(c => c.Layers!)
                            .ThenInclude(c => c.LayerItem!)

                    .Include(c => c.Template!)
                        .ThenInclude(c => c.Layers!)
                            .ThenInclude(c => c.Boxes!)
                                .ThenInclude(c => c.BoxItems!)
                                    .ThenInclude(c => c.BFont)

                    .Include(c => c.DisplayItems!)
                        .ThenInclude(c => c.ProductGroup!)
                            .ThenInclude(c => c.ProductGroupItems!)
                                .ThenInclude(c => c.Product!)
                                    .ThenInclude(c => c.ProductSizePrices)

                    .OrderBy(c => c.ActiveHour)
                    .FirstOrDefaultAsync()! ?? throw new Exception("Fail to get display");
            }

            var result = await InitializeDisplayImageForStoreProductAsync(display, store);
            return result;
        }

        public async Task<string> GetImageByDisplayAsync(int displayId)
        {
            var tempDisplay = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && !c.IsDeleted)
                .FirstOrDefault() ?? throw new Exception("Display not found or deleted");
            if (tempDisplay.IsChanged == false && tempDisplay.DisplayImgPath != null)
            {
                return tempDisplay.DisplayImgPath;
            }

            var display = await _unitOfWork.DisplayRepository.EnableQuery()
                .Include(c => c.Template!)
                    .ThenInclude(c => c.Layers!)
                        .ThenInclude(c => c.LayerItem!)
                .Include(c => c.Template!)
                    .ThenInclude(c => c.Layers!)
                        .ThenInclude(c => c.Boxes!)
                            .ThenInclude(c => c.BoxItems!)
                                .ThenInclude(c => c.BFont)
                .Include(c => c.DisplayItems!)
                    .ThenInclude(c => c.ProductGroup!)
                        .ThenInclude(c => c.ProductGroupItems!)
                            .ThenInclude(c => c.Product!)
                                .ThenInclude(c => c.ProductSizePrices)
                .FirstOrDefaultAsync(c => c.DisplayId == displayId && c.IsDeleted == false)
                ?? throw new Exception("Display not found or deleted");



            var result = await InitializeDisplayImageAsync(display);
            return result;
        }

        public async Task<string> GetTemplateImageAsync(int displayId)
        {
            var display = await _unitOfWork.DisplayRepository.FindObjectAsync(c => c.DisplayId == displayId && !c.IsDeleted)
               ?? throw new Exception("Display not found");

            if (display.IsChanged == false)
            {
                var template = await _unitOfWork.TemplateRepository.FindObjectAsync(c => c.TemplateId == display.TemplateId && !c.IsDeleted)
                    ?? throw new Exception("Template not found or deleted");
                if (template.TemplateImgPath != null) return template.TemplateImgPath;
            }

            if (display.IsChanged == false && display.DisplayImgPath != null)
            {
                return display.DisplayImgPath;
            }

            var result = await InitializeTemplateImageAsync(display);
            return result;
        }


        // POST

        public Display AddDisplay(DisplayCreateDTO displayCreateDTO)
        {
            if (displayCreateDTO.MenuId != null && displayCreateDTO.CollectionId != null) throw new Exception("MenuId and CollectionId cannot be using at the same time");

            var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
            var menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            var collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

            if (storeDevice == null) throw new Exception("StoreDevice not found or deleted");
            if (menu == null) throw new Exception("Menu not found or deleted");
            if (collection == null) throw new Exception("Collection not found or deleted");
            if (template == null) throw new Exception("Template not found or deleted");
            //if (storeDevice.RatioType == RatioType.Horizontal && template.TemplateType == TemplateType.Vertical) throw new Exception("Template ratio type is vertical");
            //if (storeDevice.RatioType == RatioType.Vertical && template.TemplateType == TemplateType.Horizontal) throw new Exception("Template ratio type is horizontal");

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public async Task<Display> AddDisplayV2Async(DisplayCreateDTO displayCreateDTO)
        {
            if (displayCreateDTO.CollectionId == 0) displayCreateDTO.CollectionId = null;
            if (displayCreateDTO.MenuId == 0) displayCreateDTO.MenuId = null;

            var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
            var templateExist = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

            if (storeDevice == null) throw new Exception("StoreDevice not found or deleted");
            if (templateExist == null) throw new Exception("Template not found or deleted");
            //if (storeDevice.RatioType == RatioType.Horizontal && templateExist.TemplateType == TemplateType.Vertical) throw new Exception("Template ratio type is vertical but device ratio type is horizontal");
            //if (storeDevice.RatioType == RatioType.Vertical && templateExist.TemplateType == TemplateType.Horizontal) throw new Exception("Template ratio type is horizontal but device ratio type is vertical");

            switch (displayCreateDTO.MenuId != null)
            {
                case true:
                    var isMenuExistInStore = await _unitOfWork.StoreMenuRepository.EnableQuery().AnyAsync(c => c.StoreId == storeDevice.StoreId && c.MenuId == displayCreateDTO.MenuId && !c.IsDeleted);
                    if (!isMenuExistInStore) throw new Exception($"Menu Id: {displayCreateDTO.MenuId} not exist in Store Id: {storeDevice.StoreId}");
                    break;

                case false:
                    var isCollectionExistInStore = await _unitOfWork.StoreCollectionRepository.EnableQuery().AnyAsync(c => c.StoreId == storeDevice.StoreId && c.CollectionId == displayCreateDTO.CollectionId && !c.IsDeleted);
                    if (!isCollectionExistInStore) throw new Exception($"Collection Id: {displayCreateDTO.CollectionId} not exist in Store Id: {storeDevice.StoreId}");
                    break;
            }

            var existDisplay = _unitOfWork.DisplayRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId
            && c.ActiveHour == displayCreateDTO.ActiveHour && !c.IsDeleted).FirstOrDefault();
            if (existDisplay != null) throw new Exception($"{displayCreateDTO.ActiveHour} already exist in store");

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            var template = await _unitOfWork.TemplateRepository.EnableQuery()
                .Include(c => c.Layers!)
                    .ThenInclude(c => c.Boxes)
                .FirstOrDefaultAsync(c => c.TemplateId == data.TemplateId) ?? throw new Exception("Template not found");

            try
            {
                var layers = template.Layers!.Where(c => c.LayerType == LayerType.Content).ToList();
                var boxes = layers.SelectMany(c => c.Boxes!).Where(c => c.BoxType == BoxType.UseInDisplay).ToList();
                if (boxes.Count == 0) throw new Exception("Template has no boxes used for display");

                switch (displayCreateDTO.MenuId != null)
                {
                    case true:
                        var menu = await _unitOfWork.MenuRepository.EnableQuery()
                           .Include(c => c.ProductGroups)
                           .FirstOrDefaultAsync(c => c.MenuId == data.MenuId && c.IsDeleted == false) ?? throw new Exception("Menu not found or deleted");

                        var menuProductGroups = menu.ProductGroups!.Where(c => !c.IsDeleted).ToList();
                        if (menuProductGroups.Count == 0) throw new Exception("This menu doesn't have any product group to initialize");
                        //if (boxes.Count < menuProductGroups.Count) throw new Exception("Not enough boxes to display all product groups");

                        switch (boxes.Count < menuProductGroups.Count)
                        {
                            case false:
                                foreach (var productGroup in menuProductGroups)
                                {
                                    var box = boxes[menuProductGroups.IndexOf(productGroup)];
                                    //var productGroup = menuProductGroups[boxes.IndexOf(box)];
                                    var displayItem = new DisplayItem
                                    {
                                        DisplayId = data.DisplayId,
                                        BoxId = box.BoxId,
                                        ProductGroupId = productGroup.ProductGroupId

                                    };
                                    _unitOfWork.DisplayItemRepository.Add(displayItem);
                                    _unitOfWork.Save();
                                }
                                break;

                            case true:
                                foreach (var box in boxes)
                                {
                                    var productGroup = menuProductGroups[boxes.IndexOf(box)];
                                    var displayItem = new DisplayItem
                                    {
                                        DisplayId = data.DisplayId,
                                        BoxId = box.BoxId,
                                        ProductGroupId = productGroup.ProductGroupId

                                    };
                                    _unitOfWork.DisplayItemRepository.Add(displayItem);
                                    _unitOfWork.Save();
                                }
                                break;
                        }


                        break;

                    case false:
                        var collection = await _unitOfWork.CollectionRepository.EnableQuery()
                           .Include(c => c.ProductGroups)
                           .FirstOrDefaultAsync(c => c.CollectionId == data.CollectionId && c.IsDeleted == false) ?? throw new Exception("Collection not found or deleted");

                        var collectionProductGroups = collection.ProductGroups!.Where(c => !c.IsDeleted).ToList();
                        //if (boxes.Count < collectionProductGroups.Count) throw new Exception("Not enough boxes to display all product groups");

                        foreach (var box in boxes)
                        {
                            var productGroup = collectionProductGroups[boxes.IndexOf(box)];
                            var displayItem = new DisplayItem
                            {
                                DisplayId = data.DisplayId,
                                BoxId = box.BoxId,
                                ProductGroupId = productGroup.ProductGroupId

                            };
                            _unitOfWork.DisplayItemRepository.Add(displayItem);
                            _unitOfWork.Save();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.DisplayRepository.Remove(data);
                _unitOfWork.Save();
                throw new Exception($"Display item fail to initialize : {ex.Message}");
            }

            return data;
        }
        // PUT

        public Display Update(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            if (displayUpdateDTO.MenuId == 0) displayUpdateDTO.MenuId = null;
            if (displayUpdateDTO.CollectionId == 0) displayUpdateDTO.CollectionId = null;

            var display = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId)
                .FirstOrDefault() ?? throw new Exception("Display not found");


            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayUpdateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

            var menu = new Menu();
            if (displayUpdateDTO.MenuId != null)
            {
                menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayUpdateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            }
            else { menu = null; }

            var collection = new Collection();
            if (displayUpdateDTO.CollectionId != null)
            {
                collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayUpdateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            }
            else { collection = null; }

            if (menu == null && collection == null) throw new Exception("Menu/Collection not found or deleted");
            //if (collection == null && displayCreateDTO.CollectionId != 0) return BadRequest("Collection not found or deleted");
            if (template == null) throw new Exception("Template not found or deleted");
            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Display not found or deleted");

            _mapper.Map(displayUpdateDTO, data);
            _unitOfWork.DisplayRepository.Update(data);
            _unitOfWork.Save();

            // Delete old displayItem
            var displayItems = _unitOfWork.DisplayItemRepository.Find(c => c.DisplayId == data.DisplayId).ToList();

            foreach (var item in displayItems)
            {
                _unitOfWork.DisplayItemRepository.Remove(item);
            }
            _unitOfWork.Save();

            //AddDisplayItem(template, menu, collection, data);

            return data;
        }


        // DELETE

        public void Delete(int displayId)
        {
            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Display not found or deleted");

            var displayItems = _unitOfWork.DisplayItemRepository.Find(c => c.DisplayId == data.DisplayId).ToList();

            _unitOfWork.DisplayItemRepository.RemoveRange(displayItems);
            _unitOfWork.Save();

            _unitOfWork.DisplayRepository.Remove(data);
            _unitOfWork.Save();
        }


        // LOGIC

        public async Task<string> InitializeDisplayImageForStoreProductAsync(Display display, Store store)
        {
            #region Initialize path

            var displayItems = display.DisplayItems.ToList();

            var storeProducts = store.StoreProducts!
                .Where(c => c.IsAvailable == true && c.IsDeleted == false)
                .ToList();

            var storeDevice = await _unitOfWork.StoreDeviceRepository
                .FindObjectAsync(c => c.StoreId == store.StoreId && c.IsDeleted == false);

            var template = display.Template
            ?? throw new Exception("Template not found");

            var backgroundLayer = template.Layers!
                .FirstOrDefault(c => c.LayerType == LayerType.BackgroundImage);

            var imgLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Image)
                .OrderBy(c => c.ZIndex)
                .ToList();

            var textLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Text)
                .OrderBy(c => c.ZIndex)
                .ToList();

            var contentLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Content)
                .OrderBy(c => c.ZIndex)
                .ToList();

            #endregion

            #region Draw path
            // Initialize bitmap
            using Bitmap b = new(width: template.TemplateWidth, height: template.TemplateHeight);
            using Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;


            // Draw background layer
            if (backgroundLayer != null && backgroundLayer.LayerItem != null)
            {
                DrawBackgroundFromLayerForDisplay(backgroundLayer, g, template.TemplateWidth, template.TemplateHeight);
            }

            // Draw image layer
            if (imgLayers.Count > 0)
            {
                DrawImageFromLayersForDisplay(imgLayers, g);
            }

            // Draw text layer
            if (textLayers.Count > 0)
            {
                await DrawTextFromLayersAsync(textLayers, g, _unitOfWork);
            }

            //Draw content layer
            if (displayItems.Count > 0)
            {
                await DrawContentFromLayerWithStoreProductAsync(displayItems, g, storeProducts, _unitOfWork);
            }

            //// Draw border
            //Pen pen = new(Color.Black, 2)
            //{
            //    Alignment = PenAlignment.Inset //<-- this
            //};
            //g.DrawRectangle(pen, 0, 0, template.TemplateWidth, template.TemplateHeight);

            #endregion

            string savePath = $"{Directory.GetCurrentDirectory()}" + @$"\wwwroot\images\display{display.DisplayId}.png";
            //ScaleBitmapAndSave(b, 100, savePath);

            if (template.TemplateType == TemplateType.Horizontal && storeDevice.DeviceHeight > storeDevice.DeviceWidth)
            {
                var temp = (int)storeDevice.DeviceHeight;
                storeDevice.DeviceHeight = (int)storeDevice.DeviceWidth;
                storeDevice.DeviceWidth = temp;
            }
            else if (template.TemplateType == TemplateType.Vertical && storeDevice.DeviceWidth > storeDevice.DeviceHeight)
            {
                var temp = (int)storeDevice.DeviceWidth;
                storeDevice.DeviceWidth = (int)storeDevice.DeviceHeight;
                storeDevice.DeviceHeight = temp;
            }

            ScaleBitmapAndSave(b, (int)storeDevice.DeviceWidth, (int)storeDevice.DeviceHeight, savePath);
            UploadToCloud(display, savePath);

            // Reset Display change field
            display.IsChanged = false;
            _unitOfWork.DisplayRepository.Update(display);
            _unitOfWork.Save();

            //b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\template4.png");
            return savePath;
        }
        public async Task<string> InitializeDisplayImageAsync(Display display)
        {
            #region Initialize path

            var displayItems = display.DisplayItems!.ToList();

            var storeDevice = await _unitOfWork.StoreDeviceRepository
                .FindObjectAsync(c => c.StoreDeviceId == display.StoreDeviceId && c.IsDeleted == false);

            var template = display.Template
            ?? throw new Exception("Template not found");

            var backgroundLayer = template.Layers!
                .FirstOrDefault(c => c.LayerType == LayerType.BackgroundImage);

            var imgLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Image)
                .OrderBy(c => c.ZIndex)
                .ToList();

            var textLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Text)
                .OrderBy(c => c.ZIndex)
                .ToList();

            var contentLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Content)
                .OrderBy(c => c.ZIndex)
                .ToList();

            #endregion

            #region Draw path
            // Initialize bitmap


            using Bitmap b = new(width: template.TemplateWidth, height: template.TemplateHeight);
            using Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;


            // Draw background layer
            if (backgroundLayer != null && backgroundLayer.LayerItem != null)
            {
                DrawBackgroundFromLayerForDisplay(backgroundLayer, g, template.TemplateWidth, template.TemplateHeight);
            }

            // Draw image layer
            if (imgLayers.Count > 0 && imgLayers != null)
            {
                DrawImageFromLayersForDisplay(imgLayers, g);
            }

            // Draw text layer
            if (textLayers.Count > 0 && textLayers != null)
            {
                await DrawTextFromLayersAsync(textLayers, g, _unitOfWork);
            }

            //// Draw content layer
            if (displayItems.Count > 0 && displayItems != null)
            {
                await DrawContentFromLayerAsync(displayItems, g, _unitOfWork);
            }

            // Draw border
            //Pen pen = new(Color.Black, 2)
            //{
            //    Alignment = PenAlignment.Inset //<-- this
            //};
            //g.DrawRectangle(pen, 0, 0, template.TemplateWidth, template.TemplateHeight);

            #endregion

            string savePath = $"{Directory.GetCurrentDirectory()}" + @$"\wwwroot\images\display{display.DisplayId}.png";
            //ScaleBitmapAndSave(b, 100, savePath);

            if (template.TemplateType == TemplateType.Horizontal && storeDevice.DeviceHeight > storeDevice.DeviceWidth)
            {
                var temp = (int)storeDevice.DeviceHeight;
                storeDevice.DeviceHeight = (int)storeDevice.DeviceWidth;
                storeDevice.DeviceWidth = temp;
            }
            else if (template.TemplateType == TemplateType.Vertical && storeDevice.DeviceWidth > storeDevice.DeviceHeight)
            {
                var temp = (int)storeDevice.DeviceWidth;
                storeDevice.DeviceWidth = (int)storeDevice.DeviceHeight;
                storeDevice.DeviceHeight = temp;
            }

            ScaleBitmapAndSave(b, (int)storeDevice.DeviceWidth, (int)storeDevice.DeviceHeight, savePath);
            //b.Save(savePath);
            UploadToCloud(display, savePath); blob: http://localhost:5063/9fa245e0-797f-451a-a749-a4b0da465c4b

            // Reset Display change field
            display.IsChanged = false;
            _unitOfWork.DisplayRepository.Update(display);
            _unitOfWork.Save();

            return savePath;
        }
        public async Task<string> InitializeTemplateImageAsync(Display display)
        {
            #region Initialize path
            var template = await _unitOfWork.TemplateRepository.EnableQuery()
                .Include(c => c.Layers!)
                    .ThenInclude(c => c.LayerItem!)
                .Include(c => c.Layers!)
                    .ThenInclude(c => c.Boxes!)
                        .ThenInclude(c => c.BoxItems!)
                            .ThenInclude(c => c.BFont)
                .FirstOrDefaultAsync(c => c.TemplateId == display.TemplateId)
            ?? throw new Exception("Template not found");

            var backgroundLayer = template.Layers!
                .FirstOrDefault(c => c.LayerType == LayerType.BackgroundImage);

            var imgLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Image)
                .OrderBy(c => c.ZIndex)
                .ToList();

            var textLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Text)
                .OrderBy(c => c.ZIndex)
                .ToList();

            var contentLayers = template.Layers!
                .Where(c => c.LayerType == LayerType.Content)
                .OrderBy(c => c.ZIndex)
                .ToList();

            #endregion

            #region Draw path
            // Initialize bitmap
            using Bitmap b = new(template.TemplateWidth, template.TemplateHeight);
            using Graphics g = Graphics.FromImage(b);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Draw border
            Pen pen1 = new(Color.Black, 2)
            {
                Alignment = PenAlignment.Inset //<-- this
            };
            g.DrawRectangle(pen1, 0, 0, template.TemplateWidth, template.TemplateHeight);

            // Draw background layer
            if (backgroundLayer != null)
            {
                DrawBackgroundFromLayer(g, template.TemplateWidth, template.TemplateHeight);
            }

            // Draw image layer
            if (imgLayers.Count > 0)
            {
                DrawImageFromLayers(imgLayers, g);
            }

            // Draw text layer
            if (textLayers.Count > 0)
            {
                DrawTextFromLayers(textLayers, g);
            }

            // Draw content layer
            if (contentLayers.Count > 0)
            {
                DrawContentFromLayers(contentLayers, g);
            }



            #endregion

            string savePath = $"{Directory.GetCurrentDirectory()}" + @$"\wwwroot\images\template.png";
            ScaleBitmapAndSave(b, 100, savePath);
            UploadToCloud(template, savePath);

            // Reset Display change field
            display.IsChanged = false;
            _unitOfWork.DisplayRepository.Update(display);
            _unitOfWork.Save();

            //b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\template3.png");
            return savePath;
        }



        // Methods
        private static void DrawBackgroundFromLayer(Graphics g, int templateWidth, int templateHeight)
        {
            using Bitmap b1 = new(templateWidth, templateHeight);
            using Graphics g1 = Graphics.FromImage(b1);
            g1.Clear(Color.Gray); // replace with drawImage background

            g.DrawImage(b1, 0, 0);

        }
        private static void DrawImageFromLayers(List<Domain.Models.Layer> imgLayers, Graphics g)
        {
            foreach (var layer in imgLayers)
            {
                var box = layer.Boxes!.FirstOrDefault(c => c.LayerId == layer.LayerId) ?? throw new Exception("Box not found in img layer");
                var rect = new RectangleF(0, 0, box.BoxWidth, box.BoxHeight);

                using Bitmap b2 = new((int)Math.Round(box.BoxWidth), (int)Math.Round(box.BoxHeight));
                using Graphics graphics = Graphics.FromImage(b2);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.Clear(RandomColor());
                graphics.DrawString("IMG layer",
                    new Font("Arial", 32, FontStyle.Bold),
                    new SolidBrush(Color.White),
                    rect,
                    new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                Pen pen = new(Color.Black, 2)
                {
                    Alignment = PenAlignment.Inset //<-- this
                };
                graphics.DrawRectangle(pen, rect);

                g.DrawImage(b2, box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
            }
        }
        private static void DrawTextFromLayers(List<Domain.Models.Layer> textLayers, Graphics g)
        {
            foreach (var layer in textLayers)
            {
                var box = layer.Boxes!.FirstOrDefault(c => c.LayerId == layer.LayerId) ?? throw new Exception("Box not found in text layer");
                var rect = new RectangleF(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);

                //using Bitmap b2 = new((int)Math.Round(box.BoxWidth), (int)Math.Round(box.BoxHeight));
                //using Graphics graphics = Graphics.FromImage(b2);
                //graphics.CompositingQuality = CompositingQuality.HighQuality;
                //graphics.SmoothingMode = SmoothingMode.HighQuality;
                //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Draw text base on box item type
                var item = box.BoxItems!.FirstOrDefault() ?? throw new Exception("Box not found");
                var boxStyle = GetStyle(box.BoxItems!.FirstOrDefault()?.Style);

                // Rotation
                var tempRotate = boxStyle!.Rotation;

                // 1. Save the current transformation state
                System.Drawing.Drawing2D.Matrix originalMatrix = g.Transform;

                // 2. Translate to the center of the rectangle
                g.TranslateTransform(rect.X, rect.Y + rect.Height / 2);

                // 3. Apply the rotation 
                g.RotateTransform(tempRotate);

                // 4. Translate back to the original position 
                g.TranslateTransform(-(rect.X), -(rect.Y + rect.Height / 2));

                var font = (boxStyle != null && item.BFont != null && boxStyle.FontSize != 0)
                    ? Ultilities.InitializeFont(boxStyle.FontSize, boxStyle.FontStyle, item.BFont!)
                    : new Font("Arial", 16);

                var color = (boxStyle != null)
                    ? GetColor(boxStyle)
                    : Color.White;

                var stringFormat = (boxStyle != null)
                    ? new StringFormat { Alignment = boxStyle.Alignment }
                    : new StringFormat { Alignment = StringAlignment.Near };

                g.FillRectangle(new SolidBrush(RandomColor()), rect);
                g.DrawString("Text layer",
                    font,
                    new SolidBrush(color),
                    rect,
                    stringFormat);

                Pen pen = new(Color.Black, 2)
                {
                    Alignment = PenAlignment.Inset //<-- this
                };
                g.DrawRectangle(pen, rect);

                // Reset rotate
                g.Transform = originalMatrix;

                //g.DrawImage(b2, box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
            }
        }
        private static void DrawContentFromLayers(List<Domain.Models.Layer> contentLayers, Graphics g)
        {
            foreach (var layer in contentLayers)
            {
                foreach (var box in layer.Boxes!)
                {
                    var rect1 = new RectangleF(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);

                    //using Bitmap b1 = new((int)Math.Round(box.BoxWidth), (int)Math.Round(box.BoxHeight));
                    //using Graphics g1 = Graphics.FromImage(b1);
                    //g1.CompositingQuality = CompositingQuality.HighQuality;
                    //g1.SmoothingMode = SmoothingMode.HighQuality;
                    //g1.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //g1.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    //g.Clear(RandomColor());
                    var brush = new SolidBrush(RandomColor());

                    g.FillRectangle(brush, rect1);
                    g.DrawString("Content layer",
                        new Font("Arial", 20),
                        new SolidBrush(Color.White),
                        rect1,
                        new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // Draw border
                    Pen pen1 = new(Color.Black, 2)
                    {
                        Alignment = PenAlignment.Inset //<-- this
                    };
                    g.DrawRectangle(pen1, rect1);

                    //g.DrawImage(b1, box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);

                    // draw box items
                    foreach (var item in box.BoxItems!)
                    {
                        var boxStyle = GetStyle(item.Style);
                        var rect2 = new RectangleF(item.BoxItemX, item.BoxItemY, item.BoxItemWidth, item.BoxItemHeight);

                        //using Bitmap b2 = new((int)Math.Round(item.BoxItemWidth), (int)Math.Round(item.BoxItemHeight));
                        //using Graphics g2 = Graphics.FromImage(b2);
                        //g2.CompositingQuality = CompositingQuality.HighQuality;
                        //g2.SmoothingMode = SmoothingMode.HighQuality;
                        //g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //g2.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        //g2.Clear(RandomColor());

                        var brush2 = new SolidBrush(RandomColor());

                        g.FillRectangle(brush2, rect2);

                        // Draw text base on box item type
                        var font = (boxStyle != null && item.BFont != null && boxStyle.FontSize != 0)
                            ? Ultilities.InitializeFont(boxStyle.FontSize, boxStyle.FontStyle, item.BFont!)
                            : new Font("Arial", 16);

                        var color = (boxStyle != null)
                            ? GetColor(boxStyle)
                            : Color.White;

                        var stringFormat = (boxStyle != null)
                            ? new StringFormat { Alignment = boxStyle.Alignment }
                            : new StringFormat { Alignment = StringAlignment.Near };

                        switch (item.BoxItemType)
                        {
                            case BoxItemType.Content:
                                g.DrawString($"Content {item.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.Header:
                                g.DrawString($"Header {item.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.ProductName:
                                g.DrawString($"ProductName {item.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.ProductDescription:
                                g.DrawString($"ProductDesc {item.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.ProductPrice:
                                g.DrawString($"Price {item.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.ProductImg:
                                g.DrawString($"ProductImg {item.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.ProductIcon:
                                g.DrawString($"I {item.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                        }

                        // Draw border
                        Pen pen2 = new(Color.Black, 2)
                        {
                            Alignment = PenAlignment.Inset //<-- this
                        };
                        g.DrawRectangle(pen2, rect2);

                        //g.DrawImage(b2, item.BoxItemX, item.BoxItemY, item.BoxItemWidth, item.BoxItemHeight);
                    }
                }

            }
        }
        private static void DrawBackgroundFromLayerForDisplay(Domain.Models.Layer backgroundLayer, Graphics g, int templateWidth, int templateHeight)
        {
            //using Bitmap b1 = new(templateWidth, templateHeight);
            //using Graphics g1 = Graphics.FromImage(b1);
            //g1.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g1.SmoothingMode = SmoothingMode.AntiAlias;
            //g1.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //g1.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var layerItem = backgroundLayer.LayerItem;
            if (layerItem == null) return;

            using var image = InitializeImage(layerItem);
            g.DrawImage(image, 0, 0, templateWidth, templateHeight);
            //g1.Clear(Color.Gray); // replace with drawImage background

            //g.DrawImage(b1, 0, 0);

        }
        private static void DrawImageFromLayersForDisplay(List<Domain.Models.Layer> imgLayers, Graphics g)
        {
            foreach (var layer in imgLayers)
            {
                var box = layer.Boxes!.FirstOrDefault(c => c.LayerId == layer.LayerId) ?? throw new Exception("Box not found in img layer");
                var boxItem = box.BoxItems!.FirstOrDefault() ?? throw new Exception("Box item not found in img layer");
                var boxStyle = GetStyle(boxItem.Style) ?? throw new Exception("Box style not found in img layer");
                var destRect = new Rectangle((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);
                //var image = Image.FromFile(layer.LayerItem!.LayerItemValue!);
                var layerItem = layer.LayerItem;
                if (layerItem == null) continue;

                var image = InitializeImage(layerItem);
                DrawImageWithAlpha(g, image, destRect, boxStyle.Transparency);
            }
        }
        private static async Task DrawTextFromLayersAsync(List<Domain.Models.Layer> textLayers, Graphics g, IUnitOfWork _unitOfWork)
        {
            foreach (var layer in textLayers)
            {
                var box = layer.Boxes!.FirstOrDefault(c => c.LayerId == layer.LayerId) ?? throw new Exception("Box not found in text layer");
                var rect = new RectangleF(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);

                //using Bitmap b2 = new((int)Math.Round(box.BoxWidth), (int)Math.Round(box.BoxHeight));
                //using Graphics graphics = Graphics.FromImage(b2);
                //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //graphics.SmoothingMode = SmoothingMode.AntiAlias;
                //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                var layerItem = layer.LayerItem;
                if (layerItem == null) continue;

                // Get Style
                var style = GetStyle(box.BoxItems!.FirstOrDefault()!.Style) ?? throw new Exception("Style fail to get");

                // Rotation
                var tempRotate = style!.Rotation;

                // 1. Save the current transformation state
                System.Drawing.Drawing2D.Matrix originalMatrix = g.Transform;

                // 2. Translate to the center of the rectangle
                g.TranslateTransform(rect.X, rect.Y + rect.Height / 2);

                // 3. Apply the rotation 
                g.RotateTransform(tempRotate);

                // 4. Translate back to the original position 
                g.TranslateTransform(-(rect.X), -(rect.Y + rect.Height / 2));

                // Get text
                var text = (style.Uppercase)
                    ? layerItem.LayerItemValue.ToUpper()
                    : layerItem.LayerItemValue;

                // Get Font
                var bFont = (style != null)
                    ? await _unitOfWork.FontRepository.FindObjectAsync(c => c.BFontId.Equals(style!.BFontId))
                    : null;

                var font = (style != null && bFont != null)
                ? Ultilities.InitializeFont(style.FontSize, style.FontStyle, bFont)
                : new Font("Arial", 20);

                // Get Color
                var color = (style != null)
                    ? GetColor(style)
                : Color.White;

                // Get Alignment
                var stringFormat = (style != null)
                    ? new StringFormat { Alignment = style.Alignment }
                    : new StringFormat { Alignment = StringAlignment.Near };

                // Draw text
                //graphics.DrawString(text,
                //    font,
                //    new SolidBrush(color),
                //    rect,
                //    stringFormat);

                DrawStringWithAlpha(g, text ?? "Text", font, color, rect, style!.Transparency, stringFormat);

                // Reset rotate
                g.Transform = originalMatrix;

                // Draw border
                //Pen pen = new(Color.Black, 2)
                //{
                //    Alignment = PenAlignment.Inset //<-- this
                //};
                //graphics.DrawRectangle(pen, rect);

                //g.DrawImage(b2, box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
            }
        }
        private static async Task DrawContentFromLayerAsync(List<DisplayItem> displayItems, Graphics g, IUnitOfWork _unitOfWork)
        {
            foreach (var displayItem in displayItems)
            {
                var productGroup = displayItem.ProductGroup;
                var products = productGroup!.ProductGroupItems!
                    .Select(c => c.Product)
                    .ToList();
                var box = displayItem.Box ?? throw new Exception("Box not found in display item");

                var boxItems2 = box.BoxItems!.GroupBy(c => c.Order);

                //using Bitmap b1 = new((int)Math.Round(box.BoxWidth), (int)Math.Round(box.BoxHeight));
                //using Graphics g1 = Graphics.FromImage(b1);
                //g1.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g1.SmoothingMode = SmoothingMode.AntiAlias;
                //g1.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //g1.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                //// Draw border
                //Pen pen1 = new(Color.Black, 2)
                //{
                //    Alignment = PenAlignment.Inset //<-- this
                //};
                //g1.DrawRectangle(pen1, rect1);

                //g.DrawImage(b1, box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);

                // Draw box item
                foreach (var group in boxItems2)
                {
                    Product product;
                    try
                    {
                        product = products[group.Key - 1]!;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }

                    foreach (var boxItem in group)
                    {
                        var boxStyle = GetStyle(boxItem.Style);
                        //var rect2 = new RectangleF(0, 0, boxItem.BoxItemWidth, boxItem.BoxItemHeight);
                        var rect2 = new RectangleF(boxItem.BoxItemX, boxItem.BoxItemY, boxItem.BoxItemWidth, boxItem.BoxItemHeight);

                        //using Bitmap b2 = new((int)Math.Round(boxItem.BoxItemWidth), (int)Math.Round(boxItem.BoxItemHeight));
                        //using Graphics g2 = Graphics.FromImage(b2);
                        //g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //g2.SmoothingMode = SmoothingMode.AntiAlias;
                        //g2.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        //g2.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        // Get Style
                        var style = GetStyle(boxItem.Style);

                        // Get Font
                        var bFont = (style != null)
                            ? await _unitOfWork.FontRepository.FindObjectAsync(c => c.BFontId.Equals(style!.BFontId))
                            : null;

                        var font = (style != null && bFont != null)
                        ? Ultilities.InitializeFont(style.FontSize, style.FontStyle, bFont)
                        : new Font("Arial", 20);

                        // Get Color
                        var color = (style != null)
                            ? GetColor(style)
                        : Color.White;

                        // Get Alignment
                        var stringFormat = (style != null)
                            ? new StringFormat { Alignment = style.Alignment }
                            : new StringFormat { Alignment = StringAlignment.Near };

                        switch (boxItem.BoxItemType)
                        {
                            case BoxItemType.Content:
                                g.DrawString($"Content {boxItem.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.Header:
                                var headerText = (style != null && style.Uppercase == true) ? productGroup.ProductGroupName!.ToUpper() : productGroup.ProductGroupName!;
                                //g2.DrawString(headerText,
                                //    font,
                                //    new SolidBrush(color),
                                //    rect2,
                                //    stringFormat);
                                DrawStringWithAlpha(g, headerText, font, color, rect2, style!.Transparency, stringFormat);
                                break;
                            case BoxItemType.ProductName:
                                var productNameText = (style != null && style.Uppercase == true) ? product.ProductName!.ToUpper() : product.ProductName!;
                                //g2.DrawString(text,
                                //    font,
                                //    new SolidBrush(color),
                                //    rect2,
                                //    stringFormat);
                                DrawStringWithAlpha(g, productNameText, font, color, rect2, style!.Transparency, stringFormat);
                                break;
                            case BoxItemType.ProductDescription:
                                var productDescriptionText = (style != null && style.Uppercase == true) ? product.ProductDescription!.ToUpper() : product.ProductDescription!;
                                //g2.DrawString(productDescriptionText,
                                //    font,
                                //    new SolidBrush(color),
                                //    rect2,
                                //    stringFormat);
                                DrawStringWithAlpha(g, productDescriptionText, font, color, rect2, style!.Transparency, stringFormat);
                                break;
                            case BoxItemType.ProductPrice:
                                var productSizePrices = product.ProductSizePrices!.Where(c => !c.IsDeleted).ToList();
                                if (productSizePrices.Count == 0) break;

                                var prices = productSizePrices.Where(c => !c.IsDeleted).ToList();
                                if (prices.Count > 1)
                                {
                                    //var price = $"{prices[0].ProductSizeType.ToString()}:{prices[0].Price}  {prices[1].ProductSizeType.ToString()}:{prices[1].Price}  {prices[2].ProductSizeType.ToString()}:{prices[2].Price}";
                                    //g.DrawString(price,
                                    //    font,
                                    //    new SolidBrush(color),
                                    //    rect2,
                                    //    stringFormat);

                                    foreach (var price in prices)
                                    {
                                        var getPrice = price.Price;
                                        string formattedPrice = "";
                                        if (product.ProductPriceCurrency == ProductPriceCurrency.VND) formattedPrice = getPrice.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN")) + " ₫";
                                        else if (product.ProductPriceCurrency == ProductPriceCurrency.USD) formattedPrice = "$" + Math.Floor(getPrice).ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));

                                        switch (price.ProductSizeType)
                                        {
                                            case ProductSizeType.S:
                                                //g.DrawString(price.Price.ToString(),
                                                //    font,
                                                //    new SolidBrush(color),
                                                //    rect2);
                                                DrawStringWithAlpha(g, formattedPrice, font, color, rect2, style!.Transparency, stringFormat);
                                                break;

                                            case ProductSizeType.M:
                                                var textSizeM = g.MeasureString(price.Price.ToString(), font).Width;
                                                var newRectM = rect2;
                                                newRectM.X += newRectM.Width / 2 - (textSizeM / 2);

                                                //g.DrawString(price.Price.ToString(),
                                                //    font,
                                                //    new SolidBrush(color),
                                                //    newRectM);
                                                DrawStringWithAlpha(g, formattedPrice, font, color, newRectM, style!.Transparency, stringFormat);
                                                break;

                                            case ProductSizeType.L:
                                                var textSizeL = g.MeasureString(price.Price.ToString(), font).Width;
                                                var newRectL = rect2;
                                                newRectL.X = newRectL.Right - textSizeL;

                                                //g.DrawString(price.Price.ToString(),
                                                //    font,
                                                //    new SolidBrush(color),
                                                //    newRectL);
                                                DrawStringWithAlpha(g, formattedPrice, font, color, newRectL, style!.Transparency, stringFormat);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    var price = prices[0].Price;
                                    string formattedPrice = "";
                                    if (product.ProductPriceCurrency == ProductPriceCurrency.VND) formattedPrice = price.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN")) + " ₫";
                                    else if (product.ProductPriceCurrency == ProductPriceCurrency.USD) formattedPrice = "$" + Math.Floor(price).ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));

                                    //g2.DrawString(formattedPrice,
                                    //    font,
                                    //    new SolidBrush(color),
                                    //    rect2,    
                                    //    stringFormat);
                                    DrawStringWithAlpha(g, formattedPrice, font, color, rect2, style!.Transparency, stringFormat);
                                }

                                break;
                            case BoxItemType.ProductImg:
                                Image? productImg = null;
                                if (product.ProductImgPath == null) break;
                                try
                                {
                                    productImg = InitializeImage(product.ProductImgPath!);
                                }
                                catch (FileNotFoundException) { break; }

                                //g2.DrawImage(Image.FromFile(product.ProductImgPath!),
                                //    rect2);
                                DrawImageWithAlpha(g, productImg, new Rectangle((int)boxItem.BoxItemX, (int)boxItem.BoxItemY, (int)boxItem.BoxItemWidth, (int)boxItem.BoxItemHeight), style!.Transparency);
                                break;
                            case BoxItemType.ProductIcon:
                                //Image? productIcon = null;
                                //try
                                //{
                                //    productIcon = InitializeImage(product.ProductLogoPath!);
                                //}
                                //catch (FileNotFoundException) { break; }

                                ////g2.DrawString($"I {boxItem.Order}",
                                ////    font,
                                ////    new SolidBrush(color),
                                ////    rect2,
                                ////    stringFormat);
                                //DrawImageWithAlpha(g, productIcon, new Rectangle((int)boxItem.BoxItemX, (int)boxItem.BoxItemY, (int)boxItem.BoxItemWidth, (int)boxItem.BoxItemHeight), style!.Transparency);
                                break;
                        }

                        // Draw border
                        //Pen pen2 = new(Color.Red, 2)
                        //{
                        //    Alignment = PenAlignment.Inset //<-- this
                        //};
                        //g2.DrawRectangle(pen2, rect2.X, rect2.Y, rect2.Width, rect2.Height);

                        // Transparency
                        //if (boxStyle != null && boxStyle.Transparency < 100)
                        //{
                        //    var b2Trans = AlterTransparency(b2, boxStyle.Transparency);
                        //    g.DrawImage(b2Trans, boxItem.BoxItemX, boxItem.BoxItemY, boxItem.BoxItemWidth, boxItem.BoxItemHeight);
                        //}
                        //else // No transparency
                        //{
                        //}
                        //g.DrawImage(b2, boxItem.BoxItemX, boxItem.BoxItemY, boxItem.BoxItemWidth, boxItem.BoxItemHeight);
                    }
                }

            }
        }
        private static async Task DrawContentFromLayerWithStoreProductAsync(List<DisplayItem> displayItems, Graphics g, List<StoreProduct> storeProducts, IUnitOfWork _unitOfWork)
        {
            foreach (var displayItem in displayItems)
            {
                var productGroup = displayItem.ProductGroup;
                var products = productGroup!.ProductGroupItems!
                    .Select(c => c.Product)
                    .ToList();

                products = products.Intersect(storeProducts.Select(c => c.Product)).ToList();

                var box = displayItem.Box ?? throw new Exception("Box not found in display item");

                var boxItems2 = box.BoxItems!.GroupBy(c => c.Order);

                //using Bitmap b1 = new((int)Math.Round(box.BoxWidth), (int)Math.Round(box.BoxHeight));
                //using Graphics g1 = Graphics.FromImage(b1);
                //g1.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g1.SmoothingMode = SmoothingMode.AntiAlias;
                //g1.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //g1.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                //// Draw border
                //Pen pen1 = new(Color.Black, 2)
                //{
                //    Alignment = PenAlignment.Inset //<-- this
                //};
                //g1.DrawRectangle(pen1, rect1);

                //g.DrawImage(b1, box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);

                // Draw box item
                foreach (var group in boxItems2)
                {
                    Product product;
                    try
                    {
                        product = products[group.Key - 1]!;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }

                    foreach (var boxItem in group)
                    {
                        var boxStyle = GetStyle(boxItem.Style);
                        //var rect2 = new RectangleF(0, 0, boxItem.BoxItemWidth, boxItem.BoxItemHeight);
                        var rect2 = new RectangleF(boxItem.BoxItemX, boxItem.BoxItemY, boxItem.BoxItemWidth, boxItem.BoxItemHeight);

                        //using Bitmap b2 = new((int)Math.Round(boxItem.BoxItemWidth), (int)Math.Round(boxItem.BoxItemHeight));
                        //using Graphics g2 = Graphics.FromImage(b2);
                        //g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //g2.SmoothingMode = SmoothingMode.AntiAlias;
                        //g2.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        //g2.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        // Get Style
                        var style = GetStyle(boxItem.Style);

                        // Get Font
                        var bFont = (style != null)
                            ? await _unitOfWork.FontRepository.FindObjectAsync(c => c.BFontId.Equals(style!.BFontId))
                            : null;

                        var font = (style != null && bFont != null)
                        ? Ultilities.InitializeFont(style.FontSize, style.FontStyle, bFont)
                        : new Font("Arial", 20);

                        // Get Color
                        var color = (style != null)
                            ? GetColor(style)
                        : Color.White;

                        // Get Alignment
                        var stringFormat = (style != null)
                            ? new StringFormat { Alignment = style.Alignment }
                            : new StringFormat { Alignment = StringAlignment.Near };

                        switch (boxItem.BoxItemType)
                        {
                            case BoxItemType.Content:
                                g.DrawString($"Content {boxItem.Order}",
                                    font,
                                    new SolidBrush(color),
                                    rect2,
                                    stringFormat);
                                break;
                            case BoxItemType.Header:
                                var headerText = (style != null && style.Uppercase == true) ? productGroup.ProductGroupName!.ToUpper() : productGroup.ProductGroupName!;
                                //g2.DrawString(headerText,
                                //    font,
                                //    new SolidBrush(color),
                                //    rect2,
                                //    stringFormat);
                                DrawStringWithAlpha(g, headerText, font, color, rect2, style!.Transparency, stringFormat);
                                break;
                            case BoxItemType.ProductName:
                                var productNameText = (style != null && style.Uppercase == true) ? product.ProductName!.ToUpper() : product.ProductName!;
                                //g2.DrawString(text,
                                //    font,
                                //    new SolidBrush(color),
                                //    rect2,
                                //    stringFormat);
                                DrawStringWithAlpha(g, productNameText, font, color, rect2, style!.Transparency, stringFormat);
                                break;
                            case BoxItemType.ProductDescription:
                                var productDescriptionText = (style != null && style.Uppercase == true) ? product.ProductDescription!.ToUpper() : product.ProductDescription!;
                                //g2.DrawString(productDescriptionText,
                                //    font,
                                //    new SolidBrush(color),
                                //    rect2,
                                //    stringFormat);
                                DrawStringWithAlpha(g, productDescriptionText, font, color, rect2, style!.Transparency, stringFormat);
                                break;
                            case BoxItemType.ProductPrice:
                                var productSizePrices = product.ProductSizePrices!.ToList();
                                if (productSizePrices.Count == 0) break;

                                var prices = productSizePrices.ToList();
                                if (prices.Count > 1)
                                {
                                    //var price = $"{prices[0].ProductSizeType.ToString()}:{prices[0].Price}  {prices[1].ProductSizeType.ToString()}:{prices[1].Price}  {prices[2].ProductSizeType.ToString()}:{prices[2].Price}";
                                    //g.DrawString(price,
                                    //    font,
                                    //    new SolidBrush(color),
                                    //    rect2,
                                    //    stringFormat);

                                    foreach (var price in prices)
                                    {
                                        var getPrice = price.Price;
                                        string formattedPrice = "";
                                        if (product.ProductPriceCurrency == ProductPriceCurrency.VND) formattedPrice = getPrice.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN")) + " ₫";
                                        else if (product.ProductPriceCurrency == ProductPriceCurrency.USD) formattedPrice = "$" + Math.Floor(getPrice).ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));

                                        switch (price.ProductSizeType)
                                        {
                                            case ProductSizeType.S:
                                                //g.DrawString(price.Price.ToString(),
                                                //    font,
                                                //    new SolidBrush(color),
                                                //    rect2);
                                                DrawStringWithAlpha(g, formattedPrice, font, color, rect2, style!.Transparency, stringFormat);
                                                break;

                                            case ProductSizeType.M:
                                                var textSizeM = g.MeasureString(price.Price.ToString(), font).Width;
                                                var newRectM = rect2;
                                                newRectM.X += newRectM.Width / 2 - (textSizeM / 2);

                                                //g.DrawString(price.Price.ToString(),
                                                //    font,
                                                //    new SolidBrush(color),
                                                //    newRectM);
                                                DrawStringWithAlpha(g, formattedPrice, font, color, newRectM, style!.Transparency, stringFormat);
                                                break;

                                            case ProductSizeType.L:
                                                var textSizeL = g.MeasureString(price.Price.ToString(), font).Width;
                                                var newRectL = rect2;
                                                newRectL.X = newRectL.Right - textSizeL;

                                                //g.DrawString(price.Price.ToString(),
                                                //    font,
                                                //    new SolidBrush(color),
                                                //    newRectL);
                                                DrawStringWithAlpha(g, formattedPrice, font, color, newRectL, style!.Transparency, stringFormat);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    var price = prices[0].Price;
                                    string formattedPrice = "";
                                    if (product.ProductPriceCurrency == ProductPriceCurrency.VND) formattedPrice = price.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN")) + " ₫";
                                    else if (product.ProductPriceCurrency == ProductPriceCurrency.USD) formattedPrice = "$" + Math.Floor(price).ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));

                                    //g2.DrawString(formattedPrice,
                                    //    font,
                                    //    new SolidBrush(color),
                                    //    rect2,    
                                    //    stringFormat);
                                    DrawStringWithAlpha(g, formattedPrice, font, color, rect2, style!.Transparency, stringFormat);
                                }

                                break;
                            case BoxItemType.ProductImg:
                                Image? productImg = null;
                                try
                                {
                                    productImg = InitializeImage(product.ProductImgPath!);
                                }
                                catch (FileNotFoundException) { break; }

                                //g2.DrawImage(Image.FromFile(product.ProductImgPath!),
                                //    rect2);
                                DrawImageWithAlpha(g, productImg, new Rectangle((int)boxItem.BoxItemX, (int)boxItem.BoxItemY, (int)boxItem.BoxItemWidth, (int)boxItem.BoxItemHeight), style!.Transparency);
                                break;
                            case BoxItemType.ProductIcon:
                                ////var isEnableIcon = storeProducts.Any(c => c.IconEnable && c.ProductId == product.ProductId);
                                ////if (!isEnableIcon) break;

                                //Image? productIcon = null;
                                //try
                                //{
                                //    productIcon = InitializeImage(product.ProductLogoPath!);
                                //}
                                //catch (FileNotFoundException) { break; }

                                ////g2.DrawString($"I {boxItem.Order}",
                                ////    font,
                                ////    new SolidBrush(color),
                                ////    rect2,
                                ////    stringFormat);
                                //DrawImageWithAlpha(g, productIcon, new Rectangle(0, 0, (int)boxItem.BoxItemWidth, (int)boxItem.BoxItemHeight), style!.Transparency);
                                break;
                        }

                        // Draw border
                        //Pen pen2 = new(Color.Red, 2)
                        //{
                        //    Alignment = PenAlignment.Inset //<-- this
                        //};
                        //g2.DrawRectangle(pen2, rect2.X, rect2.Y, rect2.Width, rect2.Height);

                        // Transparency
                        //if (boxStyle != null && boxStyle.Transparency < 100)
                        //{
                        //    var b2Trans = AlterTransparency(b2, boxStyle.Transparency);
                        //    g.DrawImage(b2Trans, boxItem.BoxItemX, boxItem.BoxItemY, boxItem.BoxItemWidth, boxItem.BoxItemHeight);
                        //}
                        //else // No transparency
                        //{
                        //}

                        //g.DrawImage(b2, boxItem.BoxItemX, boxItem.BoxItemY, boxItem.BoxItemWidth, boxItem.BoxItemHeight);
                    }
                }

            }
        }

        // Ultilities
        private static IEnumerable<Display> DataQuery(IQueryable<Display> data, int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (displayId != null)
            {
                data = data.Where(c => c.DisplayId == displayId);
            }

            if (menuId != null)
            {
                data = data.Where(c => c.MenuId == menuId);
            }

            if (collectionId != null)
            {
                data = data.Where(c => c.CollectionId == collectionId);
            }

            if (searchString != null)
            {
                data = data.Where(c => c.ActiveHour.ToString().Contains(searchString));
            }

            return PaginatedList<Display>.Create(data, pageNumber, pageSize);
        }
        private void UploadToCloud(Display display, string savePath)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(savePath), // Specify the font file
                Folder = "displays",                         // Optional: Organize fonts in a folder
                PublicId = Path.GetFileNameWithoutExtension(savePath),  // Use file name as Public ID
            };
            var uploadResult = _cloudinary.Upload(uploadParams);
            if (uploadResult.Error != null)
            {
                throw new Exception($"Upload failed: {uploadResult.Error.Message}");
            }
            display.DisplayImgPath = uploadResult.SecureUrl.ToString();

            _unitOfWork.DisplayRepository.Update(display);
            _unitOfWork.Save();
        }

        private void UploadToCloud(Template template, string savePath)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(savePath), // Specify the font file
                Folder = "displays",                         // Optional: Organize fonts in a folder
                PublicId = Path.GetFileNameWithoutExtension(savePath),  // Use file name as Public ID
            };
            var uploadResult = _cloudinary.Upload(uploadParams);
            if (uploadResult.Error != null)
            {
                throw new Exception($"Upload failed: {uploadResult.Error.Message}");
            }
            template.TemplateImgPath = uploadResult.SecureUrl.ToString();

            _unitOfWork.TemplateRepository.Update(template);
            _unitOfWork.Save();
        }

        private static Image InitializeImage(string productImgPath)
        {
            string tempPath = $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\temp";
            var tempImgPath = Path.Combine(tempPath, Guid.NewGuid() + ".png");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            using (var client = new WebClient())
            {
                client.DownloadFile(productImgPath!, tempImgPath);
            }

            if (!File.Exists(tempImgPath))
            {
                throw new FileNotFoundException();
            }

            using var fs = new FileStream(tempImgPath, FileMode.Open, FileAccess.Read); // Open file for reading
            return (Image)Image.FromStream(fs).Clone();
        }
        private static Image InitializeImage(LayerItem layerItem)
        {
            string tempPath = $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\temp";
            var tempImgPath = Path.Combine(tempPath, Guid.NewGuid() + ".png");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            using (var client = new WebClient())
            {
                client.DownloadFile(layerItem.LayerItemValue!, tempImgPath);
            }

            if (!File.Exists(tempImgPath))
            {
                throw new FileNotFoundException();
            }

            using var fs = new FileStream(tempImgPath, FileMode.Open, FileAccess.Read); // Open file for reading
            return (Image)Image.FromStream(fs).Clone();
        }
        public static void DrawImageWithAlpha(Graphics g, Image image, Rectangle destRect, float alpha)
        {
            // Ensure alpha is between 0 and 1
            alpha = (float)alpha / 100f;

            // Create a color matrix with the specified alpha value
            ColorMatrix colorMatrix = new()
            {
                Matrix33 = alpha
            };

            // Create image attributes and set the color matrix
            using ImageAttributes imageAttributes = new();
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            // Draw the image with the specified alpha
            g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
        }
        public static void DrawStringWithAlpha(Graphics g, string text, Font font, Color color, RectangleF layoutRect, float alpha, StringFormat stringFormat)
        {
            alpha = (float)alpha / 100f;

            // Create a brush with the color and specified alpha
            Color colorWithAlpha = Color.FromArgb((int)(alpha * 255), color.R, color.G, color.B);
            using (Brush brush = new SolidBrush(colorWithAlpha))
            {
                // Draw the string with the specified alpha
                g.DrawString(text, font, brush, layoutRect, stringFormat);
            }
        }
        private static Style? GetStyle(string? style)
        {
            if (string.IsNullOrEmpty(style)) return null;

            var boxStyle = JsonSerializer.Deserialize<Style>(style) ?? throw new JsonException("Fail to deserialize json");
            return boxStyle;
        }
        private static Color GetColor(Style boxStyle)
        {
            ColorConverter colorConverter = new();
            return (Color)colorConverter.ConvertFromString(boxStyle.TextColor!)!;
        }
        private static Color RandomColor()
        {
            Random rnd = new();
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }
        public static void ScaleBitmapAndSave(Bitmap originalBitmap, int scalePercentage, string savePath)
        {
            if (scalePercentage < 1 || scalePercentage > 100)
                throw new ArgumentException("Scale percentage must be between 1 and 100");

            int newWidth = originalBitmap.Width * scalePercentage / 100;
            int newHeight = originalBitmap.Height * scalePercentage / 100;

            using Bitmap scaledBitmap = new(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(scaledBitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
            }

            scaledBitmap.Save(savePath);
        }
        public static void ScaleBitmapAndSave(Bitmap originalBitmap, int deviceWidth, int deviceHeight, string savePath)
        {

            using Bitmap scaledBitmap = new(deviceWidth, deviceHeight);
            using (Graphics graphics = Graphics.FromImage(scaledBitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(originalBitmap, 0, 0, deviceWidth, deviceHeight);
            }

            scaledBitmap.Save(savePath);
        }
        public void DeleteTempFile()
        {
            string tempPath = $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\temp";
            if (!Directory.Exists(tempPath))
            {
                throw new DirectoryNotFoundException($"Temporary directory not found: {tempPath}");
            }

            var files = Directory.EnumerateFiles(tempPath); // More efficient for large directories

            foreach (var file in files)
            {
                try
                {
                    if (Path.GetFileName(file) == "DONOTDELETE.txt")
                    {
                        continue;
                    }

                    var fileInfo = new FileInfo(file);

                    // Check file attributes before deletion
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        fileInfo.Attributes &= ~FileAttributes.ReadOnly; // Remove ReadOnly attribute
                    }

                    File.Delete(file);

                }
                catch (UnauthorizedAccessException ex)
                {
                    // Specific handling for permission issues
                    throw new UnauthorizedAccessException($"Access denied to file: {file}", ex);
                }
                catch (IOException ex)
                {
                    // Handle other file I/O errors (e.g., file in use)
                    throw new IOException($"Error deleting file: {file}", ex);
                }
            }
        }

    }
}

#pragma warning restore CA1416 // Validate platform compatibility