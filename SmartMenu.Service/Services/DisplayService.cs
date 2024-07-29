using AutoMapper;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Font = System.Drawing.Font;
#pragma warning disable CA1416 // Validate platform compatibility

namespace SmartMenu.Service.Services
{
    public class DisplayService : IDisplayService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DisplayService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

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
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.ProductGroup)!
                .ThenInclude(c => c!.ProductGroupItems)!
                .ThenInclude(c => c!.Product)
                .ThenInclude(c => c!.ProductSizePrices)
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.Box)
                .ThenInclude(c => c!.BoxItems);

            var result = DataQuery(data, displayId, menuId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Display>();
        }
        
        private static Template GetInitializeTemplate(Template templateResolution, StoreDevice storeDeviceResolution)
        {
            var templateWidth = templateResolution.TemplateWidth;
            var templateHeight = templateResolution.TemplateHeight;
            float deviceWidth;
            float deviceHeight;
            if (templateResolution.TemplateType == TemplateType.Horizontal)
            {
                deviceWidth = storeDeviceResolution.DeviceHeight;
                deviceHeight = storeDeviceResolution.DeviceWidth;
            } else
            {
                deviceWidth = storeDeviceResolution.DeviceWidth;
                deviceHeight = storeDeviceResolution.DeviceHeight;
            }

            //var layers = templateResolution.Layers!;

            foreach (var layer in templateResolution.Layers!)
            {
                foreach (var box in layer.Boxes!)
                {
                    box.BoxPositionX = box.BoxPositionX * deviceWidth / templateWidth;
                    box.BoxPositionY = box.BoxPositionY * deviceHeight / templateHeight;
                    box.BoxWidth = box.BoxWidth * deviceWidth / templateWidth;
                    box.BoxHeight = box.BoxHeight * deviceHeight / templateHeight;

                    foreach (var boxItem in box.BoxItems!)
                    {
                        boxItem.FontSize = boxItem.FontSize * deviceWidth / templateWidth;
                    }
                }
            }

            templateResolution.TemplateWidth = deviceWidth;
            templateResolution.TemplateHeight = deviceHeight;

            return templateResolution;
        }
        public async Task<string> GetImageByTimeAsync(int deviceId, string tempPath)
        {
            var device =  await _unitOfWork.StoreDeviceRepository
                .EnableQuery()
                .Include(c => c.Displays)
                .FirstOrDefaultAsync(c => c.StoreDeviceId == deviceId && c.IsDeleted == false)
                ?? throw new Exception("Device not found or deleted");

            if (device.Displays!.Count == 0) throw new Exception("Device has no display");

            var hourNow = DateTime.Now.Hour;
            float minute = DateTime.Now.Minute;
            float floatHour = hourNow + (float)(minute / 60);

            Display display = new ();


            display = await _unitOfWork.DisplayRepository
                .EnableQuery()
                .Where(c => c.StoreDeviceId == device.StoreDeviceId && !c.IsDeleted && c.ActiveHour < floatHour)
                .Include(c => c.Menu)
                .Include(c => c.Collection!)
                .Include(c => c.Template!)
                .ThenInclude(c => c.Layers!.Where(c => c.IsDeleted == false))
                .ThenInclude(c => c.LayerItem)
                .Include(c => c.DisplayItems!.Where(c => c.IsDeleted == false))
                .OrderByDescending(c => c.ActiveHour)
                .FirstOrDefaultAsync()! ?? new Display();

            // If null then mean there's no display that have activeHour < recent hour
            if(display.Template == null)
            {
                display = await _unitOfWork.DisplayRepository
                    .EnableQuery()
                    .Where(c => c.StoreDeviceId == device.StoreDeviceId && c.IsDeleted == false && c.ActiveHour > floatHour)
                    .Include(c => c.Menu!)
                    .Include(c => c.Collection!)
                    .Include(c => c.Template!)
                    .ThenInclude(c => c.Layers!)
                    .ThenInclude(c => c.LayerItem)
                    .Include(c => c.DisplayItems)
                    .OrderByDescending(c => c.ActiveHour)
                    .FirstOrDefaultAsync()! ?? throw new Exception("Fail to get display");
            }

            var result = await GetImageByIdAsync(display, tempPath);
            return result;
        }
        public async Task<string> GetImageByIdAsync(Display display, string tempPath)
        {
            string imgPath = await InitializeImageV2Async(display, tempPath);
            return imgPath;
        }
        public async Task<string> GetImageByIdV2Async(int displayId, string tempPath)
        {
            try
            {
                Display display = await _unitOfWork.DisplayRepository.EnableQuery()
                        .Where(c => c.DisplayId == displayId && c.IsDeleted == false)
                        .Include(c => c.Menu!)
                        .Include(c => c.Collection!)
                        .Include(c => c.Template!)
                        .ThenInclude(c => c.Layers!)
                        .ThenInclude(c => c.LayerItem)
                        .Include(c => c.DisplayItems)
                        .FirstOrDefaultAsync()
                        ?? throw new Exception("Display not found or deleted");

                string imgPath = await InitializeImageV2Async(display, tempPath);

                return imgPath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToStringDemystified());
            }
        }

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

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }
         public async Task<Display> AddDisplayV4Async(DisplayCreateDTO displayCreateDTO, string tempPath)
        {
                if (displayCreateDTO.MenuId == 0) displayCreateDTO.MenuId = null;
                if (displayCreateDTO.CollectionId == 0) displayCreateDTO.CollectionId = null;

                var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
                var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

                var menu = new Menu();
                if (displayCreateDTO.MenuId != null)
                {
                    menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
                }
                else { menu = null; }

                var collection = new Collection();
                if (displayCreateDTO.CollectionId != null)
                {
                    collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
                }
                else { collection = null; }

                if (storeDevice == null) throw new Exception("StoreDevice not found or deleted");
                if (menu == null && collection == null) throw new Exception("Menu/Collection not found or deleted");
                //if (collection == null && displayCreateDTO.CollectionId != 0) return BadRequest("Collection not found or deleted");
                if (template == null) throw new Exception("Template not found or deleted");

                // Check if have enough layer to render productgroup
                var layerToCheck = _unitOfWork.LayerRepository
                    .Find(c => c.TemplateId == template.TemplateId && c.LayerType == LayerType.RenderLayer && c.IsDeleted == false)
                    .Count();
                int productGroupToCheck = 0;
                if (menu != null)
                {
                    productGroupToCheck = _unitOfWork.ProductGroupRepository
                        .Find(c => c.MenuId == menu.MenuId && c.IsDeleted == false)
                        .Count();
                }
                else if (collection != null)
                {
                    productGroupToCheck = _unitOfWork.ProductGroupRepository
                        .Find(c => c.CollectionId == collection.CollectionId && c.IsDeleted == false)
                        .Count();
                }

                if (layerToCheck < productGroupToCheck)
                {
                    if (menu != null)
                    {
                        throw new Exception($"Template ID {template.TemplateId} doesn't have enough render layer to render product group from menu ID: {menu.MenuId}");
                    }
                    if (collection != null)
                    {
                        throw new Exception($"Template ID {template.TemplateId} doesn't have enough render layer to render product group from collection ID: {collection.CollectionId}");
                    }

                }

                //

                var data = _mapper.Map<Display>(displayCreateDTO);
                _unitOfWork.DisplayRepository.Add(data);
                _unitOfWork.Save();

                AddDisplayItem(template, menu, collection, data);
                await InitializeImageV2Async(data, tempPath);
                return data;

        }    
        private void AddDisplayItem(Template? template, Menu? menu, Collection? collection, Display data)
        {
            try
            {
                // Add new displayItem
                // Adding display items
                var productGroups = new List<ProductGroup>();
                var boxes = new List<Box>();
                var layers = new List<SmartMenu.Domain.Models.Layer>();
                var templateWithLayer = new Template();

                // GET ProductGroup List from Menu or Collection if not null
                if (menu != null)
                {
                    productGroups = _unitOfWork.ProductGroupRepository.Find(c => c.MenuId == menu.MenuId && c.IsDeleted == false).ToList();
                }

                if (collection != null)
                {
                    productGroups = _unitOfWork.ProductGroupRepository.Find(c => c.CollectionId == collection.CollectionId && c.IsDeleted == false).ToList();
                }

                // GET Box List from display's template
                templateWithLayer = _unitOfWork.TemplateRepository.GetTemplateWithLayersAndBoxes(template!.TemplateId);

                if (templateWithLayer.Layers != null)
                {
                    layers.AddRange(templateWithLayer.Layers);

                    foreach (var layer in layers)
                    {
                        if (layer.Boxes != null)
                        {
                            boxes.AddRange(layer.Boxes);
                        }
                    }

                    // Query exact box in needed
                    boxes = boxes.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay && c.IsDeleted == false).ToList();
                }

                // Get display items list from product groups and boxes
                int productGroupCount = productGroups.Count;
                int boxCount = boxes.Count;

                if (boxCount < productGroupCount)
                {
                    _unitOfWork.DisplayRepository.Remove(data);
                    _unitOfWork.Save();

                    throw new Exception("Not enough boxes for rendering product groups.");
                }

                // Adding display items to database
                for (int i = 0; i < productGroupCount; i++)
                {
                    DisplayItemCreateDTO item = new()
                    {
                        DisplayID = data.DisplayId,
                        BoxID = boxes[i].BoxId,
                        ProductGroupID = productGroups[i].ProductGroupId
                    };

                    var itemData = _mapper.Map<DisplayItem>(item);
                    _unitOfWork.DisplayItemRepository.Add(itemData);
                    _unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToStringDemystified());
            }
        }
        private async Task<string> InitializeImageV2Async(Display data, string tempPath)
        {

                #region 0. Initialize Template
                // 1. Get template resolutions
                var templateResolution = await _unitOfWork.TemplateRepository
                    .EnableQuery()
                    .Include(c => c.Layers!)
                    .ThenInclude(c => c.LayerItem)
                    .Include(c => c.Layers!)
                    .ThenInclude(c => c.Boxes!)
                    .ThenInclude(c => c.BoxItems!)
                    .ThenInclude(c => c.Font)
                    .Where(c => c.TemplateId == data.TemplateId && c.IsDeleted == false)
                    .FirstOrDefaultAsync()
                    ?? throw new Exception("Template not found or deleted");

                if (templateResolution.Layers == null) throw new Exception("Template has no layers");

                // 1.1 Define fontCache
                 Dictionary<string, Font> fontCache = new();

                // 2. Get device resolutions
                var storeDeviceResolution = await _unitOfWork.StoreDeviceRepository.FindObjectAsync(c => c.StoreDeviceId == data.StoreDeviceId && c.IsDeleted == false)
                ?? throw new Exception("Store device not found or deleted");

                // 2.1. Get store's products
                var device = await _unitOfWork.StoreDeviceRepository.FindObjectAsync(c => c.StoreDeviceId == data.StoreDeviceId && c.IsDeleted == false)
                    ?? throw new Exception("Device not found or deleted");

                var storeProductDict = await _unitOfWork.StoreRepository
                    .EnableQuery()
                    .SelectMany(s => s.StoreProducts)
                    .Where(c => c.StoreId == device.StoreId && c.IsDeleted == false && c.IsAvailable == true)
                    .ToDictionaryAsync(c => c.ProductId);

                // 2.2 Get store's productGroups
                //var storeProductGroupDict = _unitOfWork.ProductGroupRepository
                //    .EnableQuery()
                //    .SelectMany(spg => spg.ProductGroupItems!)
                //    .Where(c =>storeProductDict.ContainsKey(c.ProductId))
                //    .ToDictionary(c => c.ProductGroupId);

                // 3. Initialize template
                var initializeTemplate = GetInitializeTemplate(templateResolution, storeDeviceResolution);
                if (initializeTemplate.Layers == null) throw new Exception("Template has no layers");
                #endregion

                #region 1. Initialize image bitmap

                // 1. Get template from display

                Template template = await _unitOfWork.TemplateRepository.FindObjectAsync(c => c.TemplateId == data.TemplateId)
                    ?? throw new Exception("Template not found");


                // 2. Generate bitmap

                Bitmap b = new((int)initializeTemplate.TemplateWidth, (int)initializeTemplate.TemplateHeight);
                using Graphics g = Graphics.FromImage(b);

                #endregion

                #region 2. Draw image layer

                // 1. Initialize menu, collection
                var menu = (data.MenuId != 0)
                    ? await _unitOfWork.MenuRepository
                        .FindObjectAsync(c => c.MenuId == data.MenuId && !c.IsDeleted)
                    : null;

                var collection = (data.CollectionId != 0)
                    ? await _unitOfWork.CollectionRepository
                        .FindObjectAsync(c => c.CollectionId == data.CollectionId && !c.IsDeleted)
                        : null;

            // 2. Draw image from template layers

            await DrawImageLayerV2Async(b, g, initializeTemplate!.Layers!, tempPath);
            #endregion

                #region 3. Draw display box from display item
            // 1. Get display items from display

            List<DisplayItem> displayItems = _unitOfWork.DisplayItemRepository
                    .Find(c => c.DisplayId == data.DisplayId)
                    .ToList();
                if (displayItems.Count == 0) throw new Exception("Display items not found or null");


                // 2. Get boxes from display items
                List<Box> boxes = _unitOfWork.BoxRepository
                    .Find(c => displayItems.Select(d => d.BoxId).Contains(c.BoxId) && c.IsDeleted == false)
                    .ToList();
                if (boxes.Count == 0) throw new Exception("Box not found or null");


                // 3.  Initialize rectangle from boxes
                List<Rectangle> rects = boxes
                    .Select(c => new Rectangle() { X = (int)c.BoxPositionX, Y = (int)c.BoxPositionY, Width = (int)c.BoxWidth, Height = (int)c.BoxHeight })
                    .ToList();
                if (rects.Count == 0) throw new Exception("Rectangle fail to initialize");


                // 4. Draw rectangles from boxes
                foreach (var rect in rects)
                {
                    g.DrawRectangle(Pens.Red, rect);
                }
                #endregion

                #region 3.1. Draw static text
                
                // 0. Get box from layer type static
                var static_Text_Layer = initializeTemplate.Layers.FirstOrDefault(c => c.LayerType == LayerType.StaticTextLayer);
                
                // 1. If static_Text_Layer found
                if (static_Text_Layer != null)
                {
                    var static_Text_Box = static_Text_Layer.Boxes!.FirstOrDefault() ?? throw new Exception("static_Text_Box not found in static_Text_Layer");


                    // 2.  Initialize Pointf for static text
                    PointF static_Text_PointF = new((int)(static_Text_Box.BoxPositionX), (int)static_Text_Box.BoxPositionY);
                    Rectangle static_Text_Rect = new((int)static_Text_Box.BoxPositionX, (int)static_Text_Box.BoxPositionY, (int)static_Text_Box.BoxWidth, (int)static_Text_Box.BoxHeight);


                    // 3.  Initialize Fonts+, Colors for static text
                    var static_Text_FontDB = static_Text_Box.BoxItems!.FirstOrDefault()!.Font ?? throw new Exception("static_Text_FontDB not found in static_Text_Box");

                    Font static_Text_Font = InitializeFont(tempPath, static_Text_Box.BoxItems!.FirstOrDefault()!, fontCache);

                    ColorConverter static_Text_ColorConverter = new();
                    Color static_Text_Color = (Color)static_Text_ColorConverter.ConvertFromString(static_Text_Box.BoxItems!.FirstOrDefault()!.BoxColor)!;


                    // 4.  Draw static text from layer item
                    var static_Text_LayerItem = _unitOfWork.LayerItemRepository.Find(c => c.LayerId == static_Text_Layer.LayerId).FirstOrDefault()
                        ?? throw new Exception("static_Text_LayerItem not found in static_Text_Layer");

                    Rectangle rect = new(
                        (int)static_Text_Box.BoxPositionX,
                        (int)static_Text_Box.BoxPositionY,
                        (int)static_Text_Box.BoxWidth,
                        (int)static_Text_Box.BoxHeight
                        );

                    SizeF static_Text_StringSize = g.MeasureString(static_Text_LayerItem.LayerItemValue, static_Text_Font);

                    if (static_Text_PointF.Y < rect.Bottom - static_Text_StringSize.Height &&
                        static_Text_PointF.X + static_Text_StringSize.Width < rect.Right)
                    {
                        g.DrawString(static_Text_LayerItem.LayerItemValue,
                                static_Text_Font,
                                new SolidBrush(static_Text_Color),
                                static_Text_PointF
                                );
                        g.DrawRectangle(new Pen(Color.Red), static_Text_Rect);
                        static_Text_Font.Dispose();
                    }

                }
                #endregion

                #region 3.2 Draw menu/collection name
                SmartMenu.Domain.Models.Layer? menuCollectionNameLayer = initializeTemplate.Layers.Where(c => c.LayerType == LayerType.MenuCollectionNameLayer).FirstOrDefault();

                if (menuCollectionNameLayer != null)
                {
                    // 0. Initialize rect, box for menu/collection name
                    Box menu_collection_name_box = menuCollectionNameLayer.Boxes!.FirstOrDefault()!;

                    Rectangle menu_collection_rect =
                        new ((int)menu_collection_name_box.BoxPositionX, (int)menu_collection_name_box.BoxPositionY, (int)menu_collection_name_box.BoxWidth, (int)menu_collection_name_box.BoxHeight);

                    // 1.  Initialize PointF for menu/collection name
                    PointF menu_Collection_Name_Point = new((int)menu_collection_name_box.BoxPositionX, (int)menu_collection_name_box.BoxPositionY);

                    // 2. Initialize Fonts, Colors for menu/collection name
                    Font menu_Collection_Name_Font = InitializeFont(tempPath, menu_collection_name_box.BoxItems!.FirstOrDefault()!, fontCache);

                    ColorConverter menu_Collection_Name_colorConverter = new();
                    Color menu_Collection_Name_Color = (Color)menu_Collection_Name_colorConverter.ConvertFromString(menu_collection_name_box.BoxItems!.FirstOrDefault()!.BoxColor)!;

                    // 3.  Intialize rectangle and stringSize for measuaring
                    Rectangle rect = new(
                        (int)menu_collection_name_box.BoxPositionX, 
                        (int)menu_collection_name_box.BoxPositionY,
                        (int)menu_collection_name_box.BoxWidth,
                        (int)menu_collection_name_box.BoxHeight
                        );

                    SizeF menu_collection_stringsize = new();
                    if (menu != null)
                    {
                        menu_collection_stringsize = g.MeasureString(menu.MenuName, menu_Collection_Name_Font);
                    } 
                    if (collection != null)
                    {
                        menu_collection_stringsize = g.MeasureString(collection.CollectionName, menu_Collection_Name_Font);
                    }

                    // 4. Draw name from menu/collection
                    if (menu_Collection_Name_Point.Y < rect.Bottom - menu_collection_stringsize.Height &&
                        menu_Collection_Name_Point.X + menu_collection_stringsize.Width < rect.Right)
                    {

                        var textToDraw = (menu != null) ? menu.MenuName : collection!.CollectionName;
                        if (!string.IsNullOrEmpty(textToDraw))
                        {
                            g.DrawString(textToDraw,
                                         menu_Collection_Name_Font,
                                         new SolidBrush(menu_Collection_Name_Color),
                                         menu_Collection_Name_Point);

                            g.DrawRectangle(Pens.Red, menu_collection_rect);
                        }
                    }
                }
                #endregion

                #region 4. Draw header from productgroup

                // 1. Efficient Product Group Retrieval:

                //var productGroupDict = _unitOfWork.ProductGroupRepository
                //    .Find(c => displayItems.Select(item => item.ProductGroupId).Contains(c.ProductGroupId))
                //    .ToDictionary(pg => pg.ProductGroupId);

                //var productGroups = displayItems
                //    .Select(item => productGroupDict.TryGetValue(item.ProductGroupId, out var pg) ? pg : throw new Exception("Product group not found or null"))
                //    .ToList();

                var productGroups = await _unitOfWork.ProductGroupRepository
                    .EnableQuery()
                    .Include(c => c.ProductGroupItems!)
                    .ThenInclude(c => c.Product)
                    .Where(c => displayItems.Select(item => item.ProductGroupId).Contains(c.ProductGroupId))
                    .ToListAsync();


                // 2. Header Point Initialization =
                var headerPoints = boxes
                    .Select(item => new PointF((int)item.BoxPositionX, (int)item.BoxPositionY)) // Creating PointF objects
                    .ToList();

                if (headerPoints.Count == 0) throw new Exception("Header point fail to initialize");


                // 3. Combined BoxItem Retrieval and Filtering:
                var headerBoxItems = await _unitOfWork.BoxItemRepository
                    .EnableQuery()
                    .Include(c => c.Font)
                    .Where(c => boxes.Select(b => b.BoxId).Contains(c.BoxId) && c.BoxItemType == BoxItemType.Header)
                    .ToListAsync();

                if (headerBoxItems.Count == 0) throw new Exception("Box items not found or null");

                // 4. Streamlined Font and Color Initialization:
                var headerFonts = new List<Font>();
                var headerColors = new List<Color>();

                foreach (var item in headerBoxItems)
                {
                    
                    // Add font (assuming InitializeFont is already optimized)
                    headerFonts.Add(InitializeFont(tempPath, item, fontCache));

                    // Add color
                    headerColors.Add((Color)new ColorConverter().ConvertFromString(item.BoxColor)!); // Assume not null
                }


                // 5. Drawing Headers (Using the Initialized headerPoints):
                foreach (var item in headerBoxItems)
                {
                    
                    // Add font (assuming InitializeFont is already optimized)
                    headerFonts.Add(InitializeFont(tempPath, item, fontCache));

                    // Add color
                    headerColors.Add((Color)new ColorConverter().ConvertFromString(item.BoxColor)!); // Assume not null
                }

                for (int i = 0; i < productGroups.Count; i++)
                {
                    g.DrawString(productGroups[i].ProductGroupName,
                        headerFonts[i],
                        new SolidBrush(headerColors[i]),
                        headerPoints[i]);
                }

                b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png");
                #endregion

                #region 5. Draw product name from store's productgroups

                // 1. Get product from menu / collection in display

                // Initialize padding constants
                const int heightPadding = 70;

                //var products = productGroups
                //    .SelectMany(pg => _unitOfWork.ProductGroupItemRepository
                //        .Find(pgi => pgi.ProductGroupId == pg.ProductGroupId)
                //        .Select(pgi => _unitOfWork.ProductRepository
                //            .Find(p => p.ProductId == pgi.ProductId)
                //            .FirstOrDefault()
                //        )
                //    )
                //    .Where(p => p != null)
                //    .ToList();

                //if (products.Count == 0)
                //    throw new Exception("No products found in the selected product groups.");

                // 2. Initialize PointF for products

                var productPoints = boxes
                    .Select(box => new PointF((int)box.BoxPositionX, (int)box.BoxPositionY + heightPadding))
                    .ToList();

                if (productPoints.Count == 0)
                    throw new Exception("Product point initialization failed.");

                // 3. Initialize Fonts, Colors for products

                List<Font> bodyFonts = new();
                List<Color> bodyColors = new();
                var bodyFontsDictionary = new Dictionary<int, Font>();

                // Get box items from boxes from step 3
                List<BoxItem> bodyBoxItems = await _unitOfWork.BoxItemRepository
                    .EnableQuery()
                    .Include(c => c.Font)
                    .Where(c => boxes.Select(b => b.BoxId).Contains(c.BoxId) && c.BoxItemType == BoxItemType.Body)
                    .ToListAsync();

                // Get fonts from box items
                foreach (var item in bodyBoxItems)
                {
                    if (item.BoxItemType == BoxItemType.Body)
                    {
                        var boxItemFromDB = await _unitOfWork.BoxItemRepository
                            .EnableQuery()
                            .Include(c => c.Font)
                            .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception("Box item not found or deleted");

                        // Add Font
                        Font bodyFont = InitializeFont(tempPath, boxItemFromDB, fontCache);
                        bodyFonts.Add(bodyFont);
                        bodyFontsDictionary.Add(item.BoxId, bodyFont);

                        // Add color to list
                        ColorConverter colorConverter = new();
                        Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
                        bodyColors.Add(color);
                    }
                }
                if (bodyFonts.Count == 0) throw new Exception("BodyFont not found or null");
                if (bodyColors.Count == 0) throw new Exception("BodyColor not found or null");


                // 4. Get biggest string size from products

                //string biggestString = "";
                //SizeF biggestStringSize = new();
                Dictionary<string, SizeF> biggestStringSizes = new();
                foreach (var productGroup in productGroups)
                {
                    string biggestString = "";
                    SizeF biggestStringSize = new ();
                    float biggestStringWidth = 0f;

                    foreach (var productGroupItem in  productGroup.ProductGroupItems!)
                    {
                        var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName, bodyFonts[productGroups.IndexOf(productGroup)]).Width;

                        if (tempWidth > biggestStringWidth)
                        {
                            biggestString = productGroupItem.Product!.ProductName;
                            biggestStringWidth = tempWidth;
                            biggestStringSize = g.MeasureString(biggestString, bodyFonts[productGroups.IndexOf(productGroup)]);
                        }
                    }

                    biggestStringSizes.Add(productGroup.ProductGroupId.ToString(), biggestStringSize);
                }
                 

                // 5. Draw products within the display area

                foreach (var productGroup in productGroups)
                {
                    // Get the starting point for the product
                    var productPoint = productPoints[productGroups.IndexOf(productGroup)];

                    // Get the rectangle for the product group
                    var rect = rects[productGroups.IndexOf(productGroup)];

                    foreach (var productGroupItem in productGroup.ProductGroupItems!)
                    {



                        // Check if the product can fit in the remaining display area
                        if (productPoint.Y < rect.Bottom - biggestStringSizes[productGroup.ProductGroupId.ToString()].Height)
                        {
                            if (productPoint.X + biggestStringSizes[productGroup.ProductGroupId.ToString()].Width < rect.Right)
                            {
                                // Check if product is in store
                                if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                {
                                    var tempFont = bodyFonts[productGroups.IndexOf(productGroup)];
                                    var newFont = new Font(tempFont, FontStyle.Strikeout);

                                    // Draw the product name on the display
                                    g.DrawString(productGroupItem.Product!.ProductName,
                                        newFont,
                                        new SolidBrush(bodyColors[productGroups.IndexOf(productGroup)]),
                                        productPoint);
                                } else
                                {

                                // Draw the product name on the display
                                g.DrawString(productGroupItem.Product!.ProductName,
                                    bodyFonts[productGroups.IndexOf(productGroup)],
                                    new SolidBrush(bodyColors[productGroups.IndexOf(productGroup)]),
                                    productPoint);
                                }

                            }

                            // Update the Y position for the next product
                            productPoint.Y += biggestStringSizes[productGroup.ProductGroupId.ToString()].Height; // Borrow biggestStringSize from "Get biggest string size from products" region
                        }
                    }
                }

                #endregion

                #region 6. Draw product prices from product size prices

                // 1. Intialize padding constants
                const int widthPaddingS = 10;
                const int priceHeightPadding = 70;
                const int sizeHeightPadding = 40;

                // 2. Get display items
                List<DisplayItem> displayItemsFromDB = await _unitOfWork.DisplayItemRepository.EnableQuery()
                    .Where(c => c.DisplayId == data.DisplayId)
                    .Include(c => c.ProductGroup!)
                    .ThenInclude(c => c.ProductGroupItems!)
                    .ThenInclude(c => c.Product!)
                    .ThenInclude(c => c.ProductSizePrices)

                    .Include(c => c.Box!)
                    .ThenInclude(c => c.BoxItems!)
                    .ThenInclude(c => c.Font)
                    .ToListAsync();

                foreach (var displayItem in displayItemsFromDB)
                {
                    Box box = _unitOfWork.BoxRepository.Find(c => c.BoxId == displayItem.BoxId).FirstOrDefault()
                    ?? throw new Exception("Box not found or deleted");
                    // 3. Get the rectangle for the displayItem
                    var rect = rects[displayItems.IndexOf(displayItem)];
                    //

                    // 4. Find the biggest product string width
                    string BIGGEST_PRODUCT_STRING = "";
                    float BIGGEST_PRODUCT_STRING_WIDTH = 0f;
                    
                    BoxItem biggest_product_boxItem = displayItem.Box!.BoxItems!
                    .Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId)
                    .FirstOrDefault()!;

                    Font productFont = productFont = InitializeFont(tempPath, biggest_product_boxItem, fontCache);

                    foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                    {

                        var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName,
                        productFont).Width;
                        if (tempWidth >= g.MeasureString(BIGGEST_PRODUCT_STRING, productFont).Width)
                        {
                            BIGGEST_PRODUCT_STRING = productGroupItem.Product!.ProductName;
                            BIGGEST_PRODUCT_STRING_WIDTH = tempWidth;
                        }
                    }
                    if (BIGGEST_PRODUCT_STRING == "") throw new Exception("BIGGEST_PRODUCT_STRING fail to initialize");

                    //

                    // 5.  Find the biggest price string height, width
                    string BIGGEST_PRICE_STRING = "";
                    float BIGGEST_PRICE_STRING_HEIGHT = 0f;
                    float BIGGEST_PRICE_STRING_WIDTH = 0f;

                    bool flagInitProductPrice = false;
                    Font productPriceWidthFont = new (FontFamily.GenericSerif, 1);
                    foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                    {
                        BoxItem boxItem = displayItem.Box!.BoxItems!
                            .Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId && c.IsDeleted == false)
                            .FirstOrDefault()!;

                        //Get font for  productpriceFOnt
                        if (flagInitProductPrice == false)
                        {
                            productPriceWidthFont = InitializeFont(tempPath, boxItem, fontCache);
                            flagInitProductPrice = true;
                        }

                        foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                        {
                            var tempHeight = g.MeasureString(productSizePrice.Price.ToString(),
                                productPriceWidthFont);

                            if (tempHeight.Height >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceWidthFont).Height)
                            {
                                BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                                BIGGEST_PRICE_STRING_HEIGHT = tempHeight.Height;
                            }

                            if (tempHeight.Width >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceWidthFont).Width)
                            {
                                BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                                BIGGEST_PRICE_STRING_WIDTH = tempHeight.Width;
                            }
                        }
                    }

                    if (BIGGEST_PRICE_STRING == "") throw new Exception("BIGGEST_PRICE_STRING fail to initialize");
                    //

                    // 6. Initialize Pointf for product size, prices based on biggest product string width; product size
                    PointF pointPriceSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + priceHeightPadding);
                    PointF pointPriceSizeM = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + priceHeightPadding);
                    PointF pointPriceSizeL = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + priceHeightPadding);

                    PointF pointSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + sizeHeightPadding);
                    PointF pointSizeM = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + sizeHeightPadding);
                    PointF pointSizeL = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + sizeHeightPadding);
                    // 


                    // 7. Get the BoxItem for the product price
                    BoxItem boxItemForSize = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                    // 8. Convert the box color to a Color object
                    Color sizeColor = (Color)new ColorConverter().ConvertFromString(boxItemForSize.BoxColor)!;

                    // 9. Get the font for the product size
                    Font productSizeFont = InitializeFont(tempPath, boxItemForSize, fontCache);

                    // 10. Intialize flag for product size
                    bool isProductSizeSRendered = false;
                    bool isProductSizeMRendered = false;
                    bool isProductSizeLRendered = false;

                    bool flagInitProductSizePrice = false;
                    Font productPriceFont = new (FontFamily.GenericSerif, 1);
                    
                    
                    // 11. Draw product prices & size
                    foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                    {
                        //// 11.1. Check if product is in store
                        //if (!storeProductDict.ContainsKey(productGroupItem.ProductId)) continue;

                        // 11.2. Draw price if have product in store
                        foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                        {
                            // Get the BoxItem for the product price
                            BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                            // Convert the box color to a Color object
                            Color color = (Color)new ColorConverter().ConvertFromString(boxItem.BoxColor)!;

                            // Get the font for the product price
                            if (flagInitProductSizePrice == false)
                            {
                                productPriceFont = InitializeFont(tempPath, boxItem, fontCache);
                                flagInitProductSizePrice = true;
                            }


                            // Check if there is enough space to draw the product price
                            if (pointPriceSizeS.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                            {

                                // Draw price for product size Normal
                                if (productSizePrice.ProductSizeType == ProductSizeType.Normal && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                                {

                                    // Check if product is in store, if not make strikethrough text
                                    if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                    {
                                        var newFont = new Font(productPriceFont, FontStyle.Strikeout);
                                        var newFont2 = new Font(productPriceFont, FontStyle.Bold);

                                        // Draw the product price on the display
                                        g.DrawString(productSizePrice.Price.ToString(),
                                            newFont,
                                            new SolidBrush(color),
                                            pointPriceSizeS);

                                        g.DrawString(productSizePrice.Price.ToString(),
                                            newFont2,
                                            new SolidBrush(color),
                                            pointPriceSizeS);
                                    } else
                                    {
                                        // Draw the product price on the display
                                        g.DrawString(productSizePrice.Price.ToString(),
                                            productPriceFont,
                                            new SolidBrush(color),
                                            pointPriceSizeS);

                                    }

                                        pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;
                                }

                                // Draw price for product size S
                                if (productSizePrice.ProductSizeType == ProductSizeType.S && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                                {
                                    // Draw the product size title if not render
                                    if (isProductSizeSRendered == false)
                                    {
                                        // Make the text go between the price number
                                        pointSizeS.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                                        g.DrawString(ProductSizeType.S.ToString(),
                                            productSizeFont,
                                            new SolidBrush(sizeColor),
                                            pointSizeS
                                            );
                                        isProductSizeSRendered = true;

                                    }

                                    // Check if product is in store, if not make strikethrough text
                                    if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                    {
                                        var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                                        // Draw the product price on the display
                                        g.DrawString(productSizePrice.Price.ToString(),
                                            newFont,
                                            new SolidBrush(color),
                                            pointPriceSizeS);
                                    }
                                    else
                                    {
                                        // Draw the product price on the display

                                        g.DrawString(productSizePrice.Price.ToString(),
                                        productPriceFont,
                                        new SolidBrush(color),
                                        pointPriceSizeS);
                                    }

                                    pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;
                                }

                            }

                            if (pointPriceSizeM.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                            {
                                // Draw price for product size M
                                if (productSizePrice.ProductSizeType == ProductSizeType.M && pointPriceSizeM.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                                {
                                    // Draw the product size title if not render
                                    if (isProductSizeMRendered == false)
                                    {
                                        // Make the text go between the price number
                                        pointSizeM.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                                        g.DrawString(ProductSizeType.M.ToString(),
                                            productSizeFont,
                                            new SolidBrush(sizeColor),
                                            pointSizeM
                                            );
                                        isProductSizeMRendered = true;
                                    }

                                    // Check if product is in store, if not make strikethrough text
                                    if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                    {
                                        var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                                        // Draw the product price on the display
                                        g.DrawString(productSizePrice.Price.ToString(),
                                            newFont,
                                            new SolidBrush(color),
                                            pointPriceSizeM);
                                    }
                                    else
                                    {
                                        // Draw the product price on the display

                                        g.DrawString(productSizePrice.Price.ToString(),
                                        productPriceFont,
                                        new SolidBrush(color),
                                        pointPriceSizeM);
                                    }

                                    pointPriceSizeM.Y += BIGGEST_PRICE_STRING_HEIGHT;
                                }

                            }

                            if (pointPriceSizeL.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                            {
                                // Draw price for product size L
                                if (productSizePrice.ProductSizeType == ProductSizeType.L && pointPriceSizeL.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                                {
                                    // Draw the product size title if not render
                                    if (isProductSizeLRendered == false)
                                    {
                                        // Make the text go between the price number
                                        pointSizeL.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                                        g.DrawString(ProductSizeType.L.ToString(),
                                            productSizeFont,
                                            new SolidBrush(sizeColor),
                                            pointSizeL
                                            );
                                        isProductSizeLRendered = true;
                                    }

                                    // Check if product is in store, if not make strikethrough text
                                    if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                    {
                                        var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                                        // Draw the product price on the display
                                        g.DrawString(productSizePrice.Price.ToString(),
                                            newFont,
                                            new SolidBrush(color),
                                            pointPriceSizeL);
                                    }
                                    else
                                    {
                                        // Draw the product price on the display

                                        g.DrawString(productSizePrice.Price.ToString(),
                                        productPriceFont,
                                        new SolidBrush(color),
                                        pointPriceSizeL);
                                    }

                                    pointPriceSizeL.Y += BIGGEST_PRICE_STRING_HEIGHT;
                                }
                            }
                        }
                    }
                }

                #endregion Draw products price within the display area

                /*
                * FINALLY: Save image
                */
                b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png");

                g.Dispose();
                b.Dispose();
                DisposeFontCache(fontCache);

                return $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png";
        }
        private static async Task<Image> InitializeImage(string tempPath, LayerItem layerItem)
        {
            System.Drawing.Image tempImage;
            var tempFontPath = Path.Combine(tempPath, Guid.NewGuid() + ".png");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(layerItem.LayerItemValue, tempFontPath); // Asynchronous download
                client.Dispose();
            }

            if (!File.Exists(tempFontPath))
            {
                throw new FileNotFoundException();
            }

            var imageBytes = await File.ReadAllBytesAsync(tempFontPath); // Asynchronous read
            using (MemoryStream ms = new(imageBytes))
            {
                tempImage = System.Drawing.Image.FromStream(ms);
                await ms.DisposeAsync();
            }

            return tempImage;
        }

        private static Font InitializeFont(string tempPath, BoxItem boxItem, Dictionary<string, Font> fontCache)
        {
            // 0. Caching for Reuse
            if (fontCache.Count != 0)
                {
                    if (fontCache.TryGetValue(boxItem.Font!.FontPath, out var cachedFont))
                    {
                        return new Font(cachedFont.FontFamily, (float)boxItem.FontSize);
                    }
                }

            Font tempFont;
            // 1. Get temp font path
            var tempFontPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".ttf");

            // 2. Check if folder exist
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            // 3. Download and write file
            using (var client = new WebClient())
            {
                client.DownloadFile(boxItem.Font!.FontPath, tempFontPath);
                //client.Dispose();
            }

            // 4. Check if file exists
            if (!File.Exists(tempFontPath))
            {
                throw new FileNotFoundException();
            }

            // 5. Add Font
            using (PrivateFontCollection fontCollection = new())
            {
                var fontByte = File.ReadAllBytes(tempFontPath);
                var pinned = GCHandle.Alloc(fontByte, GCHandleType.Pinned);
                var pointer = pinned.AddrOfPinnedObject();
                fontCollection.AddMemoryFont(pointer, fontByte.Length);
                FontFamily fontFamily = fontCollection.Families[0];
                tempFont = new Font(fontFamily, (float)boxItem.FontSize);

            }

            fontCache[boxItem.Font!.FontPath] = tempFont;
            return tempFont;
        }
        private static async Task DrawImageLayerV2Async(Bitmap b, Graphics g, ICollection<SmartMenu.Domain.Models.Layer> layers, string tempPath)
        {
            foreach (var layer in layers)
            {
                if (layer.LayerType == LayerType.BackgroundImageLayer && !layer.IsDeleted)
                {
                    using var image = await InitializeImage(tempPath, layer.LayerItem!);
                    g.DrawImage(image, 0, 0, b.Width, b.Height);
                }

                else if (layer.LayerType == LayerType.ImageLayer && !layer.IsDeleted)
                {
                    var box = layer.Boxes!.FirstOrDefault()
                               ?? throw new Exception($"Layer ID: {layer.LayerId} have no box!");
                    var rect = new Rectangle((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);

                    //using var image = System.Drawing.Image.FromFile(layer.LayerItem!.LayerItemValue)
                    //                    ?? throw new Exception($"Image not found: {layer.LayerItem.LayerItemValue}");

                    using var image = await InitializeImage(tempPath, layer.LayerItem!);
                    //g.DrawRectangle(Pens.Black, rect);
                    g.DrawImage(image, rect);
                }
            }
        }

        //public Display AddDisplayV2(DisplayCreateDTO displayCreateDTO)
        //{
        //    if (displayCreateDTO.MenuId == 0) displayCreateDTO.MenuId = null;
        //    if (displayCreateDTO.CollectionId == 0) displayCreateDTO.CollectionId = null;

        //    var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
        //    var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

        //    var menu = new Menu();
        //    if (displayCreateDTO.MenuId != null)
        //    {
        //        menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
        //    }
        //    else { menu = null; }

        //    var collection = new Collection();
        //    if (displayCreateDTO.CollectionId != null)
        //    {
        //        collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
        //    }
        //    else { collection = null; }

        //    if (storeDevice == null) throw new Exception("StoreDevice not found or deleted");
        //    if (menu == null && collection == null) throw new Exception("Menu/Collection not found or deleted");
        //    //if (collection == null && displayCreateDTO.CollectionId != 0) return BadRequest("Collection not found or deleted");
        //    if (template == null) throw new Exception("Template not found or deleted");

        //    var data = _mapper.Map<Display>(displayCreateDTO);
        //    _unitOfWork.DisplayRepository.Add(data);
        //    _unitOfWork.Save();

        //    // Adding display items
        //    var productGroups = new List<ProductGroup>();
        //    var boxes = new List<Box>();
        //    var layers = new List<SmartMenu.Domain.Models.Layer>();
        //    var templateWithLayer = new Template();
        //    var displayItems = new List<DisplayItem>();

        //    // GET ProductGroup List from Menu or Collection if not null
        //    if (menu != null)
        //    {
        //        productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, menu.MenuId, null);
        //    }

        //    if (collection != null)
        //    {
        //        productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, null, collection.CollectionId);
        //    }

        //    // GET Box List from display's template
        //    templateWithLayer = _unitOfWork.TemplateRepository.GetTemplateWithLayersAndBoxes(template.TemplateId);

        //    if (templateWithLayer.Layers != null)
        //    {
        //        layers.AddRange(templateWithLayer.Layers);

        //        foreach (var layer in layers)
        //        {
        //            if (layer.Boxes != null)
        //            {
        //                boxes.AddRange(layer.Boxes);
        //            }
        //        }

        //        // Query exact box in needed
        //        boxes = boxes.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay).ToList();
        //    }

        //    // Get display items list from product groups and boxes
        //    int productGroupCount = productGroups.Count;
        //    int boxCount = boxes.Count;
        //    //var boxesToRender = boxes.Where(c => c.)

        //    if (boxCount < productGroupCount)
        //    {
        //        _unitOfWork.DisplayRepository.Remove(data);
        //        _unitOfWork.Save();

        //        throw new Exception("Not enough boxes for rendering product groups.");
        //    }

        //    // Adding display items to database
        //    for (int i = 0; i < productGroupCount; i++)
        //    {
        //        DisplayItemCreateDTO item = new()
        //        {
        //            DisplayID = data.DisplayId,
        //            BoxID = boxes[i].BoxId,
        //            ProductGroupID = productGroups[i].ProductGroupId
        //        };

        //        var itemData = _mapper.Map<DisplayItem>(item);
        //        _unitOfWork.DisplayItemRepository.Add(itemData);
        //        _unitOfWork.Save();
        //    }

        //    /*
        //     *
        //     * BEGIN RENDERING DATA
        //     *
        //     */

        //    /* Intialize real res */
        //    Template templateResolution;
        //    float templateWidth;
        //    float templateHeight;

        //    StoreDevice storeDeviceResolution;
        //    float deviceWidth;
        //    float deviceHeight;

        //    Template initializeDisplay;

        //    // Get template resolutions
        //    templateResolution = _unitOfWork.TemplateRepository
        //        .EnableQuery()
        //        .Include(c => c.Layers!)
        //        .ThenInclude(c => c.Boxes!)
        //        .ThenInclude(c => c.BoxItems!)
        //        .ThenInclude(c => c.Font)
        //        .Where(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false)
        //        .FirstOrDefault()
        //        ?? throw new Exception("Template not found or deleted");

        //    if (templateResolution.Layers == null) throw new Exception("Template has no layers");

        //    templateWidth = templateResolution.TemplateWidth;
        //    templateHeight = templateResolution.TemplateHeight;

        //    // Get device resolutions
        //    storeDeviceResolution = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault()
        //        ?? throw new Exception("Store device not found or deleted");

        //    deviceWidth = storeDeviceResolution.DeviceWidth;
        //    deviceHeight = storeDeviceResolution.DeviceHeight;

        //    // Initialize display
        //    initializeDisplay = GetInitializeTemplate(templateResolution, storeDeviceResolution);
        //    if (initializeDisplay.Layers == null) throw new Exception("Template has no layers");

        //    /* Start render */
        //    Bitmap b = new((int)initializeDisplay.TemplateWidth, (int)initializeDisplay.TemplateHeight);
        //    using Graphics g = Graphics.FromImage(b);

        //    /*
        //    * Draw image layer
        //    */
        //    foreach (var layer in initializeDisplay.Layers)
        //    {
        //        DrawImageLayer(menu, collection, b, g, layer);
        //    }

        //    /*
        //     * Draw render layer
        //     */

        //    // Get display
        //    Display displayRender = _unitOfWork.DisplayRepository
        //        .EnableQuery()
        //        .Include(c => c.DisplayItems!)
        //        .ThenInclude(c => c.ProductGroup!)
        //        .ThenInclude(c => c.ProductGroupItems!)
        //        .ThenInclude(c => c.Product)
        //        .ThenInclude(c => c!.ProductSizePrices)

        //        .Include(c => c.DisplayItems!)
        //        .ThenInclude(c => c.Box)
        //        .ThenInclude(c => c!.BoxItems!)
        //        .ThenInclude(c => c.Font)
        //        .Where(c => c.DisplayId == data.DisplayId).FirstOrDefault()
        //        ?? throw new Exception("Display not found in draw render layer");

        //    if (displayRender.DisplayItems == null) throw new Exception("Display item not found in draw render layer");

        //    /*
        //     *  Initialize displayitem (box and productgroup)
        //     */
        //    //Rectangle boxRectFather = new ();

        //    foreach (var displayItem in displayRender.DisplayItems)
        //    {
        //        // Config rectangle
        //        Rectangle boxRect = new((int)displayItem.Box!.BoxPositionX, (int)displayItem.Box.BoxPositionY, (int)displayItem.Box.BoxWidth, (int)displayItem.Box.BoxHeight);

        //        //Initialize Header position
        //        PointF headerPosition = new((int)displayItem.Box.BoxPositionX, (int)displayItem.Box.BoxPositionY);

        //        // Initialize Font
        //        FontFamily? headerFontFamily = null;
        //        FontFamily? bodyFontFamily = null;
        //        float headerSize = 0;
        //        float bodySize = 0;
        //        string headerColorHex = "";
        //        string bodyColorHex = "";

        //        // Config color
        //        ColorConverter converter = new();
        //        Color headerColor;
        //        Color bodyColor = new();

        //        /*
        //         *  Initialize productgroup into box and draw box header
        //         */
        //        foreach (var boxItem in displayItem.Box.BoxItems!)
        //        {
        //            /* Check if the box item is of type Header and Draw the Box Header*/
        //            if (boxItem.BoxItemType == BoxItemType.Header)
        //            {
        //                // Get the header size
        //                headerSize = (float)boxItem.FontSize;

        //                // Get the header text color
        //                headerColorHex = boxItem.BoxColor;

        //                // Initialize the header text color
        //                headerColor = (Color)converter.ConvertFromString(headerColorHex)!;

        //                // Get the header font
        //                PrivateFontCollection fontCollection = new();
        //                //var fontPath = $@"{boxItem.Font!.FontPath}";
        //                fontCollection.AddFontFile($@"{boxItem.Font!.FontPath}");

        //                headerFontFamily = new(fontCollection.Families.FirstOrDefault()!.Name, fontCollection);
        //                Font headerFont = new(headerFontFamily, headerSize, FontStyle.Bold);

        //                fontCollection.Dispose();

        //                // Draw the Box Header
        //                g.DrawString(displayItem.ProductGroup!.ProductGroupName.ToUpper()
        //                , headerFont
        //                , new SolidBrush(headerColor)
        //                , headerPosition);
        //            }

        //            /* Check if the box item is of type Body */
        //            if (boxItem.BoxItemType == BoxItemType.Body)
        //            {
        //                // Get the body size
        //                bodySize = (float)boxItem.FontSize;

        //                // Get the font size
        //                PrivateFontCollection fontCollection = new();
        //                fontCollection.AddFontFile($@"{boxItem.Font!.FontPath}");

        //                bodyFontFamily = new(fontCollection.Families.FirstOrDefault()!.Name, fontCollection);
        //                Font font = new(bodyFontFamily, bodySize, FontStyle.Bold);

        //                // Get the body text color
        //                bodyColorHex = boxItem.BoxColor;

        //                // Initialize the body text color
        //                bodyColor = (Color)converter.ConvertFromString(bodyColorHex)!;
        //            }
        //        }

        //        if (headerFontFamily == null) throw new Exception("Header font fail to initialized");
        //        if (bodyFontFamily == null) throw new Exception("Body font fail to initialized");

        //        //Color headerColor = (Color)converter.ConvertFromString(headerColorHex)!;

        //        /*
        //         *  Initialize productgroup into box and draw box body content
        //         */
        //        // Draw Box Content
        //        //var boxPaddingRect = boxRect;
        //        //var boxMoneyPaddingRect = boxRect;

        //        //boxPaddingRect.X += 30;
        //        //boxPaddingRect.Y += 50;
        //        //boxMoneyPaddingRect.X = boxPaddingRect.X + 450;
        //        //boxMoneyPaddingRect.Y += 50;

        //        //var boxMoneyPaddingXHolder = boxMoneyPaddingRect;

        //        // Initialize Font
        //        Font bodyFont = new(bodyFontFamily, bodySize);

        //        // Intialize Brush
        //        Brush brush = new SolidBrush(bodyColor);

        //        // Initialize the biggest string length
        //        var biggestString = "";
        //        var biggestStringWidth = 0f;

        //        foreach (var productHolder in displayItem.ProductGroup!.ProductGroupItems!)
        //        {
        //            // Get the biggest string and biggest string width
        //            if (g.MeasureString(productHolder.Product!.ProductName, bodyFont).Width > g.MeasureString(biggestString, bodyFont).Width)
        //            {
        //                biggestString = productHolder.Product!.ProductName;
        //                biggestStringWidth = g.MeasureString(productHolder.Product!.ProductName, bodyFont).Width;
        //            }
        //        }

        //        var tempPricePadding = 10;
        //        boxRect.Height += 100;
        //        PointF bodyPosition = new(boxRect.X, boxRect.Y + 100); // + 100 to padding from header
        //        PointF pricePosition = new(boxRect.X + biggestStringWidth + tempPricePadding, boxRect.Y + 100); // + 100 to padding from header

        //        var tempPriceWidth = 0f;
        //        //g.DrawRectangle(Pens.Black, boxRect2);
        //        foreach (var productHolder in displayItem.ProductGroup!.ProductGroupItems!)
        //        {
        //            foreach (var productPrice in productHolder.Product!.ProductSizePrices!)
        //            {
        //                SizeF priceSize = g.MeasureString(productPrice.Price.ToString(), bodyFont);
        //                if (priceSize.Width > tempPriceWidth) tempPriceWidth = priceSize.Width;
        //            }
        //        }

        //        // Get price position 2 and 3
        //        PointF pricePosition2 = new(pricePosition.X + tempPriceWidth, pricePosition.Y);
        //        PointF pricePosition3 = new(pricePosition.X + tempPriceWidth * 2, pricePosition.Y);

        //        foreach (var productHolder in displayItem.ProductGroup!.ProductGroupItems!)
        //        {
        //            SizeF bodyTextSize = g.MeasureString(productHolder.Product!.ProductName, bodyFont);

        //            if (bodyPosition.Y <= boxRect.Bottom - bodyTextSize.Height)
        //            {
        //                g.DrawString(productHolder.Product!.ProductName.TrimStart()
        //                     , bodyFont
        //                    , brush
        //                    , bodyPosition
        //                );

        //                bodyPosition.Y += bodyTextSize.Height;
        //                boxRect.Height += (int)bodyPosition.Y;
        //                g.DrawRectangle(new Pen(Color.Red), boxRect);
        //            }

        //            foreach (var productPrice in productHolder.Product.ProductSizePrices!)
        //            {
        //                if (pricePosition.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.Normal)
        //                {
        //                    g.DrawString(productPrice.Price.ToString(),
        //                        bodyFont,
        //                        brush,
        //                        pricePosition);
        //                    pricePosition.Y += bodyTextSize.Height;
        //                }

        //                if (pricePosition.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.S)
        //                {
        //                    g.DrawString(productPrice.Price.ToString(),
        //                        bodyFont,
        //                        brush,
        //                        pricePosition);
        //                    pricePosition.Y += bodyTextSize.Height;
        //                }

        //                if (pricePosition2.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.M)
        //                {
        //                    g.DrawString(productPrice.Price.ToString(),
        //                        bodyFont,
        //                        brush,
        //                        pricePosition2);
        //                    pricePosition2.Y += bodyTextSize.Height;
        //                }

        //                if (pricePosition3.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.L)
        //                {
        //                    g.DrawString(productPrice.Price.ToString(),
        //                        bodyFont,
        //                        brush,
        //                        pricePosition3);
        //                    pricePosition3.Y += bodyTextSize.Height;
        //                }
        //            }
        //        }
        //    }

        //    // Save image
        //    b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\text.png");
        //    b.Dispose();

        //    return data;
        //}

        public async Task<string> GetImageByIdV3Async(int displayId, string tempPath)
        {
            try
            {
                Display display = await _unitOfWork.DisplayRepository.EnableQuery()
                        .Where(c => c.DisplayId == displayId && c.IsDeleted == false)
                        .Include(c => c.Menu!)
                        .Include(c => c.Collection!)
                        .Include(c => c.Template!)
                        .ThenInclude(c => c.Layers!)
                        .ThenInclude(c => c.LayerItem)
                        .Include(c => c.DisplayItems)
                        .FirstOrDefaultAsync()
                        ?? throw new Exception("Display not found or deleted");

                string imgPath = await InitializeImageV3Async(display, tempPath);

                return imgPath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToStringDemystified());
            }
        }

        private async Task<string> InitializeImageV3Async(Display data, string tempPath)
        {

            #region 0. Initialize Template
            // 1. Get template resolutions
            var templateResolution = await _unitOfWork.TemplateRepository
                .EnableQuery()
                .Include(c => c.Layers!)
                .ThenInclude(c => c.LayerItem)
                .Include(c => c.Layers!)
                .ThenInclude(c => c.Boxes!)
                .ThenInclude(c => c.BoxItems!)
                .ThenInclude(c => c.Font)
                .Where(c => c.TemplateId == data.TemplateId && c.IsDeleted == false)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Template not found or deleted");

            if (templateResolution.Layers == null) throw new Exception("Template has no layers");

            // 1.1 Define fontCache
            Dictionary<string, Font> fontCache = new();

            // 2. Get device resolutions
            var storeDeviceResolution = await _unitOfWork.StoreDeviceRepository.FindObjectAsync(c => c.StoreDeviceId == data.StoreDeviceId && c.IsDeleted == false)
            ?? throw new Exception("Store device not found or deleted");

            // 2.1. Get store's products
            var device = await _unitOfWork.StoreDeviceRepository.FindObjectAsync(c => c.StoreDeviceId == data.StoreDeviceId && c.IsDeleted == false)
                ?? throw new Exception("Device not found or deleted");

            var storeProductDict = await _unitOfWork.StoreRepository
                .EnableQuery()
                .SelectMany(s => s.StoreProducts)
                .Where(c => c.StoreId == device.StoreId && c.IsDeleted == false && c.IsAvailable == true)
                .ToDictionaryAsync(c => c.ProductId);

            // 2.2 Get store's productGroups
            //var storeProductGroupDict = _unitOfWork.ProductGroupRepository
            //    .EnableQuery()
            //    .SelectMany(spg => spg.ProductGroupItems!)
            //    .Where(c =>storeProductDict.ContainsKey(c.ProductId))
            //    .ToDictionary(c => c.ProductGroupId);

            // 3. Initialize template
            var initializeTemplate = GetInitializeTemplate(templateResolution, storeDeviceResolution);
            if (initializeTemplate.Layers == null) throw new Exception("Template has no layers");
            #endregion

            #region 1. Initialize image bitmap

            // 1. Get template from display

            Template template = await _unitOfWork.TemplateRepository.FindObjectAsync(c => c.TemplateId == data.TemplateId)
                ?? throw new Exception("Template not found");


            // 2. Generate bitmap

            Bitmap b = new((int)initializeTemplate.TemplateWidth, (int)initializeTemplate.TemplateHeight);
            using Graphics g = Graphics.FromImage(b);

            #endregion

            #region 2. Draw image layer

            // 1. Initialize menu, collection
            var menu = (data.MenuId != 0)
                ? await _unitOfWork.MenuRepository
                    .FindObjectAsync(c => c.MenuId == data.MenuId && !c.IsDeleted)
                : null;

            var collection = (data.CollectionId != 0)
                ? await _unitOfWork.CollectionRepository
                    .FindObjectAsync(c => c.CollectionId == data.CollectionId && !c.IsDeleted)
                    : null;

            // 2. Draw image from template layers

            await DrawImageLayerV2Async(b, g, initializeTemplate!.Layers!, tempPath);
            #endregion

            #region 3. Draw display box from display item
            // 1. Get display items from display

            List<DisplayItem> displayItems = _unitOfWork.DisplayItemRepository
                    .Find(c => c.DisplayId == data.DisplayId)
                    .ToList();
            if (displayItems.Count == 0) throw new Exception("Display items not found or null");


            // 2. Get boxes from display items
            List<Box> boxes = _unitOfWork.BoxRepository
                .Find(c => displayItems.Select(d => d.BoxId).Contains(c.BoxId) && c.IsDeleted == false)
                .ToList();
            if (boxes.Count == 0) throw new Exception("Box not found or null");


            // 3.  Initialize rectangle from boxes
            List<Rectangle> rects = boxes
                .Select(c => new Rectangle() { X = (int)c.BoxPositionX, Y = (int)c.BoxPositionY, Width = (int)c.BoxWidth, Height = (int)c.BoxHeight })
                .ToList();
            if (rects.Count == 0) throw new Exception("Rectangle fail to initialize");


            // 4. Draw rectangles from boxes
            foreach (var rect in rects)
            {
                Bitmap b1 = new (rect.Width, rect.Height);
                using Graphics g1 = Graphics.FromImage(b);
                g1.DrawRectangle(Pens.Red, rect);
                g.DrawImage(b1, rect);

                g1.Dispose();
                b1.Dispose();
            }
            #endregion

            #region 3.1. Draw static text

            // 0. Get box from layer type static
            var static_Text_Layer = initializeTemplate.Layers.FirstOrDefault(c => c.LayerType == LayerType.StaticTextLayer);

            // 1. If static_Text_Layer found
            if (static_Text_Layer != null)
            {
                var static_Text_Box = static_Text_Layer.Boxes!.FirstOrDefault() ?? throw new Exception("static_Text_Box not found in static_Text_Layer");


                // 2.  Initialize Pointf for static text
                PointF static_Text_PointF = new((int)(static_Text_Box.BoxPositionX), (int)static_Text_Box.BoxPositionY);
                Rectangle static_Text_Rect = new((int)static_Text_Box.BoxPositionX, (int)static_Text_Box.BoxPositionY, (int)static_Text_Box.BoxWidth, (int)static_Text_Box.BoxHeight);


                // 3.  Initialize Fonts+, Colors for static text
                var static_Text_FontDB = static_Text_Box.BoxItems!.FirstOrDefault()!.Font ?? throw new Exception("static_Text_FontDB not found in static_Text_Box");

                Font static_Text_Font = InitializeFont(tempPath, static_Text_Box.BoxItems!.FirstOrDefault()!, fontCache);

                ColorConverter static_Text_ColorConverter = new();
                Color static_Text_Color = (Color)static_Text_ColorConverter.ConvertFromString(static_Text_Box.BoxItems!.FirstOrDefault()!.BoxColor)!;


                // 4.  Draw static text from layer item
                var static_Text_LayerItem = _unitOfWork.LayerItemRepository.Find(c => c.LayerId == static_Text_Layer.LayerId).FirstOrDefault()
                    ?? throw new Exception("static_Text_LayerItem not found in static_Text_Layer");

                Rectangle rect = new(
                    (int)static_Text_Box.BoxPositionX,
                    (int)static_Text_Box.BoxPositionY,
                    (int)static_Text_Box.BoxWidth,
                    (int)static_Text_Box.BoxHeight
                    );

                SizeF static_Text_StringSize = g.MeasureString(static_Text_LayerItem.LayerItemValue, static_Text_Font);

                //if (static_Text_PointF.Y < rect.Bottom - static_Text_StringSize.Height &&
                //    static_Text_PointF.X + static_Text_StringSize.Width < rect.Right)
                //{

                //}
                using(Bitmap b1 = new ((int)static_Text_PointF.X, (int)static_Text_PointF.Y))
                {
                    Graphics g1 = Graphics.FromImage(b1);
                    g1.DrawString(static_Text_LayerItem.LayerItemValue,
                        static_Text_Font,
                        new SolidBrush(static_Text_Color),
                        static_Text_PointF
                        );
                    g1.DrawRectangle(new Pen(Color.Red), static_Text_Rect);
                    g.DrawImage(b1, static_Text_PointF);
                    
                    static_Text_Font.Dispose();
                    g1.Dispose();
                    b1.Dispose();
                }


            }
            #endregion

            #region 3.2 Draw menu/collection name
            SmartMenu.Domain.Models.Layer? menuCollectionNameLayer = initializeTemplate.Layers.Where(c => c.LayerType == LayerType.MenuCollectionNameLayer).FirstOrDefault();

            if (menuCollectionNameLayer != null)
            {
                // 0. Initialize rect, box for menu/collection name
                Box menu_collection_name_box = menuCollectionNameLayer.Boxes!.FirstOrDefault()!;

                Rectangle menu_collection_rect =
                    new((int)menu_collection_name_box.BoxPositionX, (int)menu_collection_name_box.BoxPositionY, (int)menu_collection_name_box.BoxWidth, (int)menu_collection_name_box.BoxHeight);

                // 1.  Initialize PointF for menu/collection name
                PointF menu_Collection_Name_Point = new((int)menu_collection_name_box.BoxPositionX, (int)menu_collection_name_box.BoxPositionY);

                // 2. Initialize Fonts, Colors for menu/collection name
                Font menu_Collection_Name_Font = InitializeFont(tempPath, menu_collection_name_box.BoxItems!.FirstOrDefault()!, fontCache);

                ColorConverter menu_Collection_Name_colorConverter = new();
                Color menu_Collection_Name_Color = (Color)menu_Collection_Name_colorConverter.ConvertFromString(menu_collection_name_box.BoxItems!.FirstOrDefault()!.BoxColor)!;

                // 3.  Intialize rectangle and stringSize for measuaring
                Rectangle rect = new(
                    (int)menu_collection_name_box.BoxPositionX,
                    (int)menu_collection_name_box.BoxPositionY,
                    (int)menu_collection_name_box.BoxWidth,
                    (int)menu_collection_name_box.BoxHeight
                    );

                SizeF menu_collection_stringsize = new();
                if (menu != null)
                {
                    menu_collection_stringsize = g.MeasureString(menu.MenuName, menu_Collection_Name_Font);
                }
                if (collection != null)
                {
                    menu_collection_stringsize = g.MeasureString(collection.CollectionName, menu_Collection_Name_Font);
                }

                // 4. Draw name from menu/collection
                //if (menu_Collection_Name_Point.Y < rect.Bottom - menu_collection_stringsize.Height &&
                //    menu_Collection_Name_Point.X + menu_collection_stringsize.Width < rect.Right)
                //{


                //}

                using(Bitmap b1 = new Bitmap((int)rect.Width, (int)rect.Height))
                {
                    Graphics g1 = Graphics.FromImage(b1);

                    var textToDraw = (menu != null) ? menu.MenuName : collection!.CollectionName;
                    if (!string.IsNullOrEmpty(textToDraw))
                    {
                        g1.DrawString(textToDraw,
                                     menu_Collection_Name_Font,
                                     new SolidBrush(menu_Collection_Name_Color),
                                     menu_Collection_Name_Point);

                        g1.DrawRectangle(Pens.Red, menu_collection_rect);
                        g.DrawImage(b1, menu_Collection_Name_Point);
                    }

                    g1.Dispose();
                    b1.Dispose();
                }


            }
            #endregion

            #region 4. Draw header from productgroup

            // 1. Efficient Product Group Retrieval:

            //var productGroupDict = _unitOfWork.ProductGroupRepository
            //    .Find(c => displayItems.Select(item => item.ProductGroupId).Contains(c.ProductGroupId))
            //    .ToDictionary(pg => pg.ProductGroupId);

            //var productGroups = displayItems
            //    .Select(item => productGroupDict.TryGetValue(item.ProductGroupId, out var pg) ? pg : throw new Exception("Product group not found or null"))
            //    .ToList();

            var productGroups = await _unitOfWork.ProductGroupRepository
                .EnableQuery()
                .Include(c => c.ProductGroupItems!)
                .ThenInclude(c => c.Product)
                .Where(c => displayItems.Select(item => item.ProductGroupId).Contains(c.ProductGroupId))
                .ToListAsync();


            // 2. Header Point Initialization =
            var headerPoints = boxes
                .Select(item => new PointF((int)item.BoxPositionX, (int)item.BoxPositionY)) // Creating PointF objects
                .ToList();

            if (headerPoints.Count == 0) throw new Exception("Header point fail to initialize");

            var headerRects = boxes
                .Select(item => new Rectangle((int)item.BoxPositionX, (int)item.BoxPositionY, (int)item.BoxWidth, (int)item.BoxHeight)) // Creating PointF objects
                .ToList();

            if (headerRects.Count == 0) throw new Exception("Header rect fail to initialize");


            // 3. Combined BoxItem Retrieval and Filtering:
            var headerBoxItems = await _unitOfWork.BoxItemRepository
                .EnableQuery()
                .Include(c => c.Font)
                .Where(c => boxes.Select(b => b.BoxId).Contains(c.BoxId) && c.BoxItemType == BoxItemType.Header)
                .ToListAsync();

            if (headerBoxItems.Count == 0) throw new Exception("Box items not found or null");

            // 4. Streamlined Font and Color Initialization:
            var headerFonts = new List<Font>();
            var headerColors = new List<Color>();

            foreach (var item in headerBoxItems)
            {

                // Add font (assuming InitializeFont is already optimized)
                headerFonts.Add(InitializeFont(tempPath, item, fontCache));

                // Add color
                headerColors.Add((Color)new ColorConverter().ConvertFromString(item.BoxColor)!); // Assume not null
            }


            // 5. Drawing Headers (Using the Initialized headerPoints):
            foreach (var item in headerBoxItems)
            {

                // Add font (assuming InitializeFont is already optimized)
                headerFonts.Add(InitializeFont(tempPath, item, fontCache));

                // Add color
                headerColors.Add((Color)new ColorConverter().ConvertFromString(item.BoxColor)!); // Assume not null
            }

            for (int i = 0; i < productGroups.Count; i++)
            {
                using(Bitmap b1 = new Bitmap((int)headerRects[i].Width, (int)headerRects[i].Height))
                {
                    using Graphics g1 = Graphics.FromImage(b1);
                    g1.DrawString(productGroups[i].ProductGroupName,
                        headerFonts[i],
                        new SolidBrush(headerColors[i]),
                        new PointF(0, 0));
                    g.DrawImage(b1, headerPoints[i]);

                }
            }

            b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png");
            #endregion

            #region 5. Draw product name from store's productgroups

            // 1. Get product from menu / collection in display

            // Initialize padding constants
            const int heightPadding = 70;

            //var products = productGroups
            //    .SelectMany(pg => _unitOfWork.ProductGroupItemRepository
            //        .Find(pgi => pgi.ProductGroupId == pg.ProductGroupId)
            //        .Select(pgi => _unitOfWork.ProductRepository
            //            .Find(p => p.ProductId == pgi.ProductId)
            //            .FirstOrDefault()
            //        )
            //    )
            //    .Where(p => p != null)
            //    .ToList();

            //if (products.Count == 0)
            //    throw new Exception("No products found in the selected product groups.");

            // 2. Initialize PointF for products

            var productRects = boxes
                .Select(box => new Rectangle((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight))
                .ToList();

            var productPoints = boxes
                .Select(box => new PointF(x: 0,y: heightPadding))
                .ToList();

            if (productPoints.Count == 0)
                throw new Exception("Product point initialization failed.");

            // 3. Initialize Fonts, Colors for products

            List<Font> bodyFonts = new();
            List<Color> bodyColors = new();
            var bodyFontsDictionary = new Dictionary<int, Font>();

            // Get box items from boxes from step 3
            List<BoxItem> bodyBoxItems = await _unitOfWork.BoxItemRepository
                .EnableQuery()
                .Include(c => c.Font)
                .Where(c => boxes.Select(b => b.BoxId).Contains(c.BoxId) && c.BoxItemType == BoxItemType.Body)
                .ToListAsync();

            // Get fonts from box items
            foreach (var item in bodyBoxItems)
            {
                if (item.BoxItemType == BoxItemType.Body)
                {
                    var boxItemFromDB = await _unitOfWork.BoxItemRepository
                        .EnableQuery()
                        .Include(c => c.Font)
                        .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
                        .FirstOrDefaultAsync()
                        ?? throw new Exception("Box item not found or deleted");

                    // Add Font
                    Font bodyFont = InitializeFont(tempPath, boxItemFromDB, fontCache);
                    bodyFonts.Add(bodyFont);
                    bodyFontsDictionary.Add(item.BoxId, bodyFont);

                    // Add color to list
                    ColorConverter colorConverter = new();
                    Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
                    bodyColors.Add(color);
                }
            }
            if (bodyFonts.Count == 0) throw new Exception("BodyFont not found or null");
            if (bodyColors.Count == 0) throw new Exception("BodyColor not found or null");


            // 4. Get biggest string size from products

            //string biggestString = "";
            //SizeF biggestStringSize = new();
            Dictionary<string, SizeF> biggestStringSizes = new();
            foreach (var productGroup in productGroups)
            {
                string biggestString = "";
                SizeF biggestStringSize = new();
                float biggestStringWidth = 0f;

                foreach (var productGroupItem in productGroup.ProductGroupItems!)
                {
                    var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName, bodyFonts[productGroups.IndexOf(productGroup)]).Width;

                    if (tempWidth > biggestStringWidth)
                    {
                        biggestString = productGroupItem.Product!.ProductName;
                        biggestStringWidth = tempWidth;
                        biggestStringSize = g.MeasureString(biggestString, bodyFonts[productGroups.IndexOf(productGroup)]);
                    }
                }

                biggestStringSizes.Add(productGroup.ProductGroupId.ToString(), biggestStringSize);
            }


            // 5. Draw products within the display area

            foreach (var productGroup in productGroups)
            {
                // Get the starting point for the product
                var productPoint = productPoints[productGroups.IndexOf(productGroup)];

                // Get the rectangle for the product group
                var rect = rects[productGroups.IndexOf(productGroup)];

                foreach (var productGroupItem in productGroup.ProductGroupItems!)
                {



                    //// Check if the product can fit in the remaining display area
                    //if (productPoint.Y < rect.Bottom - biggestStringSizes[productGroup.ProductGroupId.ToString()].Height)
                    //{
                    //    if (productPoint.X + biggestStringSizes[productGroup.ProductGroupId.ToString()].Width < rect.Right)
                    //    {


                    //    }

                    //}

                    // Check if product is in store
                    using(Bitmap b1 = new (rect.Width, rect.Height))
                    {
                        Graphics g1 = Graphics.FromImage(b1);
                        if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                        {
                            var tempFont = bodyFonts[productGroups.IndexOf(productGroup)];
                            var newFont = new Font(tempFont, FontStyle.Strikeout);

                            // Draw the product name on the display
                            g1.DrawString(productGroupItem.Product!.ProductName,
                                newFont,
                                new SolidBrush(bodyColors[productGroups.IndexOf(productGroup)]),
                                productPoint);
                            g.DrawImage(b1, rect);
                        }
                        else
                        {

                            // Draw the product name on the display
                            g1.DrawString(productGroupItem.Product!.ProductName,
                                bodyFonts[productGroups.IndexOf(productGroup)],
                                new SolidBrush(bodyColors[productGroups.IndexOf(productGroup)]),
                                productPoint);
                            g.DrawImage(b1, rect);
                        }

                        // Update the Y position for the next product
                        productPoint.Y += biggestStringSizes[productGroup.ProductGroupId.ToString()].Height; // Borrow biggestStringSize from "Get biggest string size from products" region
                    }
                    
                }
            }

            #endregion

            #region 6. Draw product prices from product size prices

            // 1. Intialize padding constants
            const int widthPaddingS = 10;
            const int priceHeightPadding = 70;
            const int sizeHeightPadding = 40;

            // 2. Get display items
            List<DisplayItem> displayItemsFromDB = await _unitOfWork.DisplayItemRepository.EnableQuery()
                .Where(c => c.DisplayId == data.DisplayId)
                .Include(c => c.ProductGroup!)
                .ThenInclude(c => c.ProductGroupItems!)
                .ThenInclude(c => c.Product!)
                .ThenInclude(c => c.ProductSizePrices)

                .Include(c => c.Box!)
                .ThenInclude(c => c.BoxItems!)
                .ThenInclude(c => c.Font)
                .ToListAsync();

            foreach (var displayItem in displayItemsFromDB)
            {
                Box box = _unitOfWork.BoxRepository.Find(c => c.BoxId == displayItem.BoxId).FirstOrDefault()
                ?? throw new Exception("Box not found or deleted");
                // 3. Get the rectangle for the displayItem
                var rect = rects[displayItems.IndexOf(displayItem)];
                //

                // 4. Find the biggest product string width
                string BIGGEST_PRODUCT_STRING = "";
                float BIGGEST_PRODUCT_STRING_WIDTH = 0f;

                BoxItem biggest_product_boxItem = displayItem.Box!.BoxItems!
                .Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId)
                .FirstOrDefault()!;

                Font productFont = productFont = InitializeFont(tempPath, biggest_product_boxItem, fontCache);

                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {

                    var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName,
                    productFont).Width;
                    if (tempWidth >= g.MeasureString(BIGGEST_PRODUCT_STRING, productFont).Width)
                    {
                        BIGGEST_PRODUCT_STRING = productGroupItem.Product!.ProductName;
                        BIGGEST_PRODUCT_STRING_WIDTH = tempWidth;
                    }
                }
                if (BIGGEST_PRODUCT_STRING == "") throw new Exception("BIGGEST_PRODUCT_STRING fail to initialize");

                //

                // 5.  Find the biggest price string height, width
                string BIGGEST_PRICE_STRING = "";
                float BIGGEST_PRICE_STRING_HEIGHT = 0f;
                float BIGGEST_PRICE_STRING_WIDTH = 0f;

                bool flagInitProductPrice = false;
                Font productPriceWidthFont = new(FontFamily.GenericSerif, 1);
                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    BoxItem boxItem = displayItem.Box!.BoxItems!
                        .Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId && c.IsDeleted == false)
                        .FirstOrDefault()!;

                    //Get font for  productpriceFOnt
                    if (flagInitProductPrice == false)
                    {
                        productPriceWidthFont = InitializeFont(tempPath, boxItem, fontCache);
                        flagInitProductPrice = true;
                    }

                    foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                    {
                        var tempHeight = g.MeasureString(productSizePrice.Price.ToString(),
                            productPriceWidthFont);

                        if (tempHeight.Height >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceWidthFont).Height)
                        {
                            BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                            BIGGEST_PRICE_STRING_HEIGHT = tempHeight.Height;
                        }

                        if (tempHeight.Width >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceWidthFont).Width)
                        {
                            BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                            BIGGEST_PRICE_STRING_WIDTH = tempHeight.Width;
                        }
                    }
                }

                if (BIGGEST_PRICE_STRING == "") throw new Exception("BIGGEST_PRICE_STRING fail to initialize");
                //

                // 6. Initialize Pointf for product size, prices based on biggest product string width; product size
                PointF pointPriceSizeS = new( BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, priceHeightPadding);
                PointF pointPriceSizeM = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH, priceHeightPadding);
                PointF pointPriceSizeL = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, priceHeightPadding);

                PointF pointSizeS = new(BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, sizeHeightPadding);
                PointF pointSizeM = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH, sizeHeightPadding);
                PointF pointSizeL = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, sizeHeightPadding);

                Rectangle rectPrice = new ((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);
                // 


                // 7. Get the BoxItem for the product price
                BoxItem boxItemForSize = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                // 8. Convert the box color to a Color object
                Color sizeColor = (Color)new ColorConverter().ConvertFromString(boxItemForSize.BoxColor)!;

                // 9. Get the font for the product size
                Font productSizeFont = InitializeFont(tempPath, boxItemForSize, fontCache);

                // 10. Intialize flag for product size
                bool isProductSizeSRendered = false;
                bool isProductSizeMRendered = false;
                bool isProductSizeLRendered = false;

                bool flagInitProductSizePrice = false;
                Font productPriceFont = new(FontFamily.GenericSerif, 1);


                // 11. Draw product prices & size
                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    //// 11.1. Check if product is in store
                    //if (!storeProductDict.ContainsKey(productGroupItem.ProductId)) continue;

                    // 11.2. Draw price if have product in store
                    foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                    {
                        // Get the BoxItem for the product price
                        BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                        // Convert the box color to a Color object
                        Color color = (Color)new ColorConverter().ConvertFromString(boxItem.BoxColor)!;

                        // Get the font for the product price
                        if (flagInitProductSizePrice == false)
                        {
                            productPriceFont = InitializeFont(tempPath, boxItem, fontCache);
                            flagInitProductSizePrice = true;
                        }

                        /*
                         * ************************************
                         */

                        if(productSizePrice.ProductSizeType == ProductSizeType.Normal) {
                            using (Bitmap b1 = new (rectPrice.Width, rectPrice.Height))
                            {
                                Graphics g1 = Graphics.FromImage(b1);

                                // Check if product is in store, if not make strikethrough text
                                if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                {
                                    var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                                    // Draw the product price on the display
                                    g1.DrawString(productSizePrice.Price.ToString(),
                                        newFont,
                                        new SolidBrush(color),
                                        pointPriceSizeS);
                                    g.DrawImage(b1, rectPrice);
                                }
                                else
                                {
                                    // Draw the product price on the display
                                    g1.DrawString(productSizePrice.Price.ToString(),
                                        productPriceFont,
                                        new SolidBrush(color),
                                        pointPriceSizeS);
                                    g.DrawImage(b1, rectPrice);
                                }

                                pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;

                                g1.Dispose();
                                b1.Dispose();
                            }

                        }

                        if (productSizePrice.ProductSizeType == ProductSizeType.S)
                        {
                            using (Bitmap b1 = new(rectPrice.Width, rectPrice.Height))
                            {
                                Graphics g1 = Graphics.FromImage(b1);

                                // Draw the product size title if not render
                                if (isProductSizeSRendered == false)
                                {
                                    // Make the text go between the price number
                                    pointSizeS.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                                    g1.DrawString(ProductSizeType.S.ToString(),
                                        productSizeFont,
                                        new SolidBrush(sizeColor),
                                        pointSizeS
                                        );
                                    isProductSizeSRendered = true;

                                    g.DrawImage(b1, rectPrice);
                                }

                                // Check if product is in store, if not make strikethrough text
                                if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                {
                                    var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                                    // Draw the product price on the display
                                    g1.DrawString(productSizePrice.Price.ToString(),
                                        newFont,
                                        new SolidBrush(color),
                                        pointPriceSizeS);

                                    g.DrawImage(b1 , rectPrice);
                                }
                                else
                                {
                                    // Draw the product price on the display

                                    g1.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeS);

                                    g.DrawImage(b1, rectPrice);
                                }

                                pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;

                                g1.Dispose();
                                b1.Dispose();
                            }

                        }

                        if (productSizePrice.ProductSizeType == ProductSizeType.M)
                        {
                            using (Bitmap b1 = new(rectPrice.Width, rectPrice.Height))
                            {
                                Graphics g1 = Graphics.FromImage(b1);

                                // Draw the product size title if not render
                                if (isProductSizeMRendered == false)
                                {
                                    // Make the text go between the price number
                                    pointSizeM.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                                    g1.DrawString(ProductSizeType.M.ToString(),
                                        productSizeFont,
                                        new SolidBrush(sizeColor),
                                        pointSizeM
                                        );
                                    isProductSizeMRendered = true;

                                    g.DrawImage(b1, rectPrice);
                                }

                                // Check if product is in store, if not make strikethrough text
                                if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                {
                                    var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                                    // Draw the product price on the display
                                    g1.DrawString(productSizePrice.Price.ToString(),
                                        newFont,
                                        new SolidBrush(color),
                                        pointPriceSizeM);

                                    g.DrawImage(b1, rectPrice);
                                }
                                else
                                {
                                    // Draw the product price on the display

                                    g1.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeM);

                                    g.DrawImage(b1, rectPrice);
                                }

                                pointPriceSizeM.Y += BIGGEST_PRICE_STRING_HEIGHT;

                                g1.Dispose();
                                b1.Dispose();
                            }

                        }

                        if (productSizePrice.ProductSizeType == ProductSizeType.L)
                        {
                            using (Bitmap b1 = new(rectPrice.Width, rectPrice.Height))
                            {
                                Graphics g1 = Graphics.FromImage(b1);

                                // Draw the product size title if not render
                                if (isProductSizeLRendered == false)
                                {
                                    // Make the text go between the price number
                                    pointSizeL.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                                    g1.DrawString(ProductSizeType.L.ToString(),
                                        productSizeFont,
                                        new SolidBrush(sizeColor),
                                        pointSizeL
                                        );
                                    isProductSizeLRendered = true;

                                    g.DrawImage(b1, rectPrice);
                                }

                                // Check if product is in store, if not make strikethrough text
                                if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                                {
                                    var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                                    // Draw the product price on the display
                                    g1.DrawString(productSizePrice.Price.ToString(),
                                        newFont,
                                        new SolidBrush(color),
                                        pointPriceSizeL);

                                    g.DrawImage(b1, rectPrice);
                                }
                                else
                                {
                                    // Draw the product price on the display

                                    g1.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeL);

                                    g.DrawImage(b1, rectPrice);
                                }

                                pointPriceSizeL.Y += BIGGEST_PRICE_STRING_HEIGHT;

                                g1.Dispose();
                                b1.Dispose();
                            }

                        }
                        /*
                         * *****************************************
                         */



                        //// Check if there is enough space to draw the product price
                        //if (pointPriceSizeS.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                        //{

                        //    // Draw price for product size Normal
                        //    if (productSizePrice.ProductSizeType == ProductSizeType.Normal && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                        //    {


                        //    }

                        //    // Draw price for product size S
                        //    if (productSizePrice.ProductSizeType == ProductSizeType.S && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                        //    {

                        //    }

                        //}

                        //if (pointPriceSizeM.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                        //{
                        //    // Draw price for product size M
                        //    if (productSizePrice.ProductSizeType == ProductSizeType.M && pointPriceSizeM.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                        //    {
                        //        // Draw the product size title if not render
                        //        if (isProductSizeMRendered == false)
                        //        {
                        //            // Make the text go between the price number
                        //            pointSizeM.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                        //            g.DrawString(ProductSizeType.M.ToString(),
                        //                productSizeFont,
                        //                new SolidBrush(sizeColor),
                        //                pointSizeM
                        //                );
                        //            isProductSizeMRendered = true;
                        //        }

                        //        // Check if product is in store, if not make strikethrough text
                        //        if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                        //        {
                        //            var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                        //            // Draw the product price on the display
                        //            g.DrawString(productSizePrice.Price.ToString(),
                        //                newFont,
                        //                new SolidBrush(color),
                        //                pointPriceSizeM);
                        //        }
                        //        else
                        //        {
                        //            // Draw the product price on the display

                        //            g.DrawString(productSizePrice.Price.ToString(),
                        //            productPriceFont,
                        //            new SolidBrush(color),
                        //            pointPriceSizeM);
                        //        }

                        //        pointPriceSizeM.Y += BIGGEST_PRICE_STRING_HEIGHT;
                        //    }

                        //}

                        //if (pointPriceSizeL.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                        //{
                        //    // Draw price for product size L
                        //    if (productSizePrice.ProductSizeType == ProductSizeType.L && pointPriceSizeL.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                        //    {
                        //        // Draw the product size title if not render
                        //        if (isProductSizeLRendered == false)
                        //        {
                        //            // Make the text go between the price number
                        //            pointSizeL.X += BIGGEST_PRICE_STRING_WIDTH / 4;

                        //            g.DrawString(ProductSizeType.L.ToString(),
                        //                productSizeFont,
                        //                new SolidBrush(sizeColor),
                        //                pointSizeL
                        //                );
                        //            isProductSizeLRendered = true;
                        //        }

                        //        // Check if product is in store, if not make strikethrough text
                        //        if (!storeProductDict.ContainsKey(productGroupItem.ProductId))
                        //        {
                        //            var newFont = new Font(productPriceFont, FontStyle.Strikeout);

                        //            // Draw the product price on the display
                        //            g.DrawString(productSizePrice.Price.ToString(),
                        //                newFont,
                        //                new SolidBrush(color),
                        //                pointPriceSizeL);
                        //        }
                        //        else
                        //        {
                        //            // Draw the product price on the display

                        //            g.DrawString(productSizePrice.Price.ToString(),
                        //            productPriceFont,
                        //            new SolidBrush(color),
                        //            pointPriceSizeL);
                        //        }

                        //        pointPriceSizeL.Y += BIGGEST_PRICE_STRING_HEIGHT;
                        //    }
                        //}
                    }
                }
            }

            #endregion Draw products price within the display area

            /*
            * FINALLY: Save image
            */
            b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png");

            g.Dispose();
            b.Dispose();
            DisposeFontCache(fontCache);

            return $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png";
        }

        public async Task<string> UpdateContainImageAsync(int displayId, DisplayUpdateDTO displayUpdateDTO, string tempPath)
        {
            Display updateDisplay = Update(displayId, displayUpdateDTO) 
                ?? throw new Exception("Display fail to update");

            Display display = await _unitOfWork.DisplayRepository.EnableQuery()
                .Where(c => c.DisplayId == displayId && c.IsDeleted == false)
                .Include(c => c.Menu!)
                .Include(c => c.Collection!)
                .Include(c => c.Template!)
                .ThenInclude(c => c.Layers!)
                .ThenInclude(c => c.LayerItem)
                .Include(c => c.DisplayItems)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Display not found or deleted");

            string imgPath = await InitializeImageV2Async(display, tempPath);
            return imgPath;

        }

        public Display Update(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            if (displayUpdateDTO.MenuId == 0) displayUpdateDTO.MenuId = null;
            if (displayUpdateDTO.CollectionId == 0) displayUpdateDTO.CollectionId = null;

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

            AddDisplayItem(template, menu, collection, data);

            return data;
        }

        public void Delete(int displayId)
        {
            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Display not found or deleted");

            _unitOfWork.DisplayRepository.Remove(data);
            _unitOfWork.Save();
        }

        //private static void DrawImageLayer(Menu? menu, Collection? collection, Bitmap b, Graphics g, SmartMenu.Domain.Models.Layer layer)
        //{
        //    // Draw background
        //    if (layer.LayerType == LayerType.BackgroundImageLayer)
        //    {
        //        var image = System.Drawing.Image.FromFile(layer.LayerItem!.LayerItemValue);
        //        g.DrawImage(image, 0, 0, b.Width, b.Height);
        //    }

        //    // Draw image
        //    //if (layer.LayerType == LayerType.ImageLayer)
        //    //{
        //    //    foreach (var box in layer.Boxes!)
        //    //    {
        //    //        var rectf = new RectangleF(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
        //    //        var rect = new Rectangle((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);

        //    //        // Lưu ý lỗi file hình : Out of memory tên phía sau bắt đầu "....-10212.jpg"
        //    //        var image = Image.FromFile(layer.LayerItem!.LayerItemValue)
        //    //            ?? throw new Exception($"Image not found: {layer.LayerItem.LayerItemValue}");

        //    //        g.DrawRectangle(Pens.Black, rect);
        //    //        g.DrawImage(image, rectf);
        //    //    }
        //    //}

        //    // Draw title
        //    if (layer.LayerType == LayerType.MenuCollectionNameLayer)
        //    {
        //        var box = layer.Boxes!.FirstOrDefault() ?? throw new Exception("Box not found in draw title layer");

        //        RectangleF boxRectTitleF = new(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
        //        Rectangle boxRectTitle = new((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);
        //        // Get name
        //        string name = "";
        //        if (menu != null) name = menu.MenuName;
        //        if (collection != null) name = collection.CollectionName;

        //        // Get BoxItem
        //        BoxItem boxItem = box.BoxItems!.FirstOrDefault()
        //            ?? throw new Exception("Box Item not found in draw title layer");

        //        // Get Font
        //        Domain.Models.Font font = boxItem.Font
        //            ?? throw new Exception("Font not found in draw title layer");

        //        // Config font
        //        PrivateFontCollection fontCollection = new();
        //        //fontCollection.AddFontFile(@"C:\Users\tekat\OneDrive\Desktop\Lilita_One\LilitaOne.ttf");
        //        var fontPath = $@"{font.FontPath}";
        //        fontCollection.AddFontFile($@"{font.FontPath}");
        //        FontFamily fontFamily = new(fontCollection.Families.FirstOrDefault()!.Name, fontCollection);

        //        // Config color
        //        ColorConverter converter = new();
        //        Color color = (Color)converter.ConvertFromString(boxItem.BoxColor)!;

        //        // Draw menu name
        //        //g.DrawString(name
        //        //    , new System.Drawing.Font(fontFamily, (float)box.BoxItems!.First().FontSize, FontStyle.Bold)
        //        //    , new SolidBrush(Color.Black)
        //        //    , boxRectTitleF
        //        //    , new StringFormat() { Alignment = boxItem.TextFormat }
        //        //    );

        //        //g.DrawRectangle(Pens.Black, boxRectTitle);
        //    }
        //}

        //public Display AddDisplayV3(DisplayCreateDTO displayCreateDTO)
        //{
        //    if (displayCreateDTO.MenuId == 0) displayCreateDTO.MenuId = null;
        //    if (displayCreateDTO.CollectionId == 0) displayCreateDTO.CollectionId = null;

        //    var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
        //    var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

        //    var menu = new Menu();
        //    if (displayCreateDTO.MenuId != null)
        //    {
        //        menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
        //    }
        //    else { menu = null; }

        //    var collection = new Collection();
        //    if (displayCreateDTO.CollectionId != null)
        //    {
        //        collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
        //    }
        //    else { collection = null; }

        //    if (storeDevice == null) throw new Exception("StoreDevice not found or deleted");
        //    if (menu == null && collection == null) throw new Exception("Menu/Collection not found or deleted");
        //    //if (collection == null && displayCreateDTO.CollectionId != 0) return BadRequest("Collection not found or deleted");
        //    if (template == null) throw new Exception("Template not found or deleted");

        //    var data = _mapper.Map<Display>(displayCreateDTO);
        //    _unitOfWork.DisplayRepository.Add(data);
        //    _unitOfWork.Save();

        //    // Adding display items
        //    var productGroups = new List<ProductGroup>();
        //    var boxes = new List<Box>();
        //    var layers = new List<SmartMenu.Domain.Models.Layer>();
        //    var templateWithLayer = new Template();
        //    var displayItems = new List<DisplayItem>();

        //    // GET ProductGroup List from Menu or Collection if not null
        //    if (menu != null)
        //    {
        //        productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, menu.MenuId, null);
        //    }

        //    if (collection != null)
        //    {
        //        productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, null, collection.CollectionId);
        //    }

        //    // GET Box List from display's template
        //    templateWithLayer = _unitOfWork.TemplateRepository.GetTemplateWithLayersAndBoxes(template.TemplateId);

        //    if (templateWithLayer.Layers != null)
        //    {
        //        layers.AddRange(templateWithLayer.Layers);

        //        foreach (var layer in layers)
        //        {
        //            if (layer.Boxes != null)
        //            {
        //                boxes.AddRange(layer.Boxes);
        //            }
        //        }

        //        // Query exact box in needed
        //        boxes = boxes.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay).ToList();
        //    }

        //    // Get display items list from product groups and boxes
        //    int productGroupCount = productGroups.Count;
        //    int boxCount = boxes.Count;
        //    //var boxesToRender = boxes.Where(c => c.)

        //    if (boxCount < productGroupCount)
        //    {
        //        _unitOfWork.DisplayRepository.Remove(data);
        //        _unitOfWork.Save();

        //        throw new Exception("Not enough boxes for rendering product groups.");
        //    }

        //    // Adding display items to database
        //    for (int i = 0; i < productGroupCount; i++)
        //    {
        //        DisplayItemCreateDTO item = new()
        //        {
        //            DisplayID = data.DisplayId,
        //            BoxID = boxes[i].BoxId,
        //            ProductGroupID = productGroups[i].ProductGroupId
        //        };

        //        var itemData = _mapper.Map<DisplayItem>(item);
        //        _unitOfWork.DisplayItemRepository.Add(itemData);
        //        _unitOfWork.Save();
        //    }

        //    InitializeImage(data);
        //    return data;
        //}

        //private string InitializeImage(Display data)
        //{
        //    /*
        //     * 0. Initialize display
        //     */

        //    #region Initialize Template
        //    // Get template resolutions
        //    var templateResolution = _unitOfWork.TemplateRepository
        //        .EnableQuery()
        //        .Include(c => c.Layers!)
        //        .ThenInclude(c => c.Boxes!)
        //        .ThenInclude(c => c.BoxItems!)
        //        .ThenInclude(c => c.Font)
        //        .Where(c => c.TemplateId == data.TemplateId && c.IsDeleted == false)
        //        .FirstOrDefault()
        //        ?? throw new Exception("Template not found or deleted");

        //    if (templateResolution.Layers == null) throw new Exception("Template has no layers");

        //    // Get device resolutions
        //    var storeDeviceResolution = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == data.StoreDeviceId && c.IsDeleted == false).FirstOrDefault()
        //        ?? throw new Exception("Store device not found or deleted");

        //    // Initialize template
        //    var initializeTemplate = GetInitializeTemplate(templateResolution, storeDeviceResolution);
        //    if (initializeTemplate.Layers == null) throw new Exception("Template has no layers");
        //    #endregion

        //    /*
        //     * 1. Initialize image bitmap
        //     */

        //    #region Get template from display

        //    Template template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == data.TemplateId).FirstOrDefault()
        //        ?? throw new Exception("Template not found");

        //    #endregion Get template from display

        //    #region Generate bitmap

        //    Bitmap b = new((int)initializeTemplate.TemplateWidth, (int)initializeTemplate.TemplateHeight);
        //    using Graphics g = Graphics.FromImage(b);

        //    #endregion Generate bitmap

        //    /*
        //     * 2. Draw image layer
        //     */

        //    #region Initialize menu, collection
        //    var menu = new Menu();
        //    if (data.MenuId != null)
        //    {
        //        menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == data.MenuId && c.IsDeleted == false).FirstOrDefault();
        //    }
        //    else { menu = null; }

        //    var collection = new Collection();
        //    if (data.CollectionId != null)
        //    {
        //        collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == data.CollectionId && c.IsDeleted == false).FirstOrDefault();
        //    }
        //    else { collection = null; }
        //    #endregion

        //    #region Draw image from template layers
        //    foreach (var layer in initializeTemplate!.Layers!)
        //    {
        //        DrawImageLayer(menu, collection, b, g, layer);
        //    }
        //    #endregion

        //    /*
        //     * 3. Draw display box from display item
        //     */

        //    #region Get display items from display

        //    List<DisplayItem> displayItems = _unitOfWork.DisplayItemRepository
        //        .Find(c => c.DisplayId == data.DisplayId)
        //        .ToList();
        //    if (displayItems.Count == 0) throw new Exception("Display items not found or null");

        //    #endregion Get display items from display

        //    #region Get boxes from display items
        //    // CONTINUE HERE -> IMAGE RENDER
        //    List<Box> boxes = new();
        //    foreach (var item in displayItems)
        //    {
        //        boxes.Add(_unitOfWork.BoxRepository
        //            .Find(c => c.BoxId == item.BoxId)
        //            .FirstOrDefault()
        //            ?? throw new Exception("Box not found or null")
        //        );
        //    }
        //    if (boxes.Count == 0) throw new Exception("Box not found or null");

        //    #endregion Get boxes from display items

        //    #region Initialize rectangle from boxes

        //    List<Rectangle> rects = new();
        //    foreach (var item in boxes)
        //    {
        //        Rectangle rect = new((int)item.BoxPositionX, (int)item.BoxPositionY, (int)item.BoxWidth, (int)item.BoxHeight);
        //        rects.Add(rect);
        //    }
        //    if (rects.Count == 0) throw new Exception("Rectangle fail to initialize");

        //    #endregion Initialize rectangle from boxes

        //    #region Draw rectangles from boxes

        //    foreach (var rect in rects)
        //    {
        //        g.DrawRectangle(Pens.Red, rect);
        //    }

        //    #endregion Draw rectangles from boxes

        //    /*
        //     * 4. Draw header from productgroup
        //     */

        //    #region Get product group from display item

        //    List<ProductGroup> productGroups = new();

        //    foreach (var item in displayItems)
        //    {
        //        productGroups.Add(_unitOfWork.ProductGroupRepository
        //            .Find(c => c.ProductGroupId == item.ProductGroupId)
        //            .FirstOrDefault()
        //            ?? throw new Exception("Product group not found or null")
        //        );
        //    }
        //    if (productGroups.Count == 0) throw new Exception("Product group not found or null");

        //    #endregion Get product group from display item

        //    #region Initialize PointF for headers

        //    List<PointF> headerPoints = new();

        //    // Get postion x,y from boxes in step 2
        //    foreach (var item in boxes)
        //    {
        //        PointF point = new((int)item.BoxPositionX, (int)item.BoxPositionY);
        //        headerPoints.Add(point);
        //    }
        //    if (headerPoints.Count == 0) throw new Exception("Header point fail to initialize");

        //    #endregion Initialize PointF for headers

        //    #region Initialize Fonts, Colors for headers

        //    List<Font> headerFonts = new();
        //    List<Color> headerColors = new();

        //    // Get box items from boxes
        //    List<BoxItem> boxItems = new();

        //    foreach (var item in boxes)
        //    {
        //        boxItems.AddRange(_unitOfWork.BoxItemRepository
        //            .Find(c => c.BoxId == item.BoxId)
        //            .ToList()
        //        );
        //    }
        //    if (boxItems.Count == 0) throw new Exception("Box items not found or null");

        //    // Get fonts from box items
        //    foreach (var item in boxItems)
        //    {
        //        if (item.BoxItemType == BoxItemType.Header)
        //        {
        //            var boxItemFromDB = _unitOfWork.BoxItemRepository
        //                .EnableQuery()
        //                .Include(c => c.Font)
        //                .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
        //                .FirstOrDefault()
        //                ?? throw new Exception("Box item not found or deleted");

        //            // Add font to list
        //            PrivateFontCollection fontCollection = new();
        //            fontCollection.AddFontFile(boxItemFromDB.Font!.FontPath);
        //            headerFonts.Add(new Font(fontCollection.Families[0], (int)boxItemFromDB.FontSize));
        //            fontCollection.Dispose();

        //            // Add color to list
        //            ColorConverter colorConverter = new();
        //            Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
        //            headerColors.Add(color);
        //        }
        //    }
        //    if (headerFonts.Count == 0) throw new Exception("Font not found or null");
        //    if (headerColors.Count == 0) throw new Exception("Color not found or null");

        //    #endregion Initialize Fonts, Colors for headers

        //    #region Draw header from product group

        //    foreach (var productGroup in productGroups)
        //    {
        //        g.DrawString(productGroup.ProductGroupName,
        //            headerFonts[productGroups.IndexOf(productGroup)],
        //            new SolidBrush(headerColors[productGroups.IndexOf(productGroup)]),
        //            headerPoints[productGroups.IndexOf(productGroup)]
        //            );
        //    }

        //    #endregion Draw header from product group

        //    /*
        //     * 5. Draw product name from productgroup
        //     */

        //    #region Get product from menu / collection in display

        //    // Initialize padding constants
        //    const int heightPadding = 100;

        //    // Initialize product group item
        //    List<ProductGroupItem> productGroupItems = new();

        //    // Get product group items from product groups
        //    foreach (var productGroup in productGroups)
        //    {
        //        productGroupItems.AddRange(_unitOfWork.ProductGroupItemRepository
        //            .Find(c => c.ProductGroupId == productGroup.ProductGroupId)
        //            .ToList()
        //        );
        //    }
        //    if (productGroupItems.Count == 0) throw new Exception("Product group item not found or null");

        //    // Get product from product group items
        //    List<Product> products = new();
        //    foreach (var productGroupItem in productGroupItems)
        //    {
        //        products.Add(_unitOfWork.ProductRepository
        //            .Find(c => c.ProductId == productGroupItem.ProductId)
        //            .FirstOrDefault()
        //            ?? throw new Exception("Product not found or null")
        //        );
        //    }
        //    if (products.Count == 0) throw new Exception("Product not found or null");

        //    #endregion Get product from menu / collection in display

        //    #region Initialize PointF for products

        //    List<PointF> productPoints = new();

        //    // Get postion x,y from boxes in step 2
        //    foreach (var item in boxes)
        //    {
        //        PointF point = new((int)item.BoxPositionX, (int)item.BoxPositionY + heightPadding);
        //        productPoints.Add(point);
        //    }
        //    if (productPoints.Count == 0) throw new Exception("Product point fail to initialize");

        //    #endregion Initialize PointF for products

        //    #region Initialize Fonts, Colors for products

        //    List<Font> bodyFonts = new();
        //    List<Color> bodyColors = new();

        //    // Get box items from boxes from step 3
        //    List<BoxItem> bodyBoxItems = new();
        //    bodyBoxItems = boxItems;

        //    // Get fonts from box items
        //    foreach (var item in bodyBoxItems)
        //    {
        //        if (item.BoxItemType == BoxItemType.Body)
        //        {
        //            var boxItemFromDB = _unitOfWork.BoxItemRepository
        //                .EnableQuery()
        //                .Include(c => c.Font)
        //                .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
        //                .FirstOrDefault()
        //                ?? throw new Exception("Box item not found or deleted");

        //            // Add font to list
        //            PrivateFontCollection fontCollection = new();
        //            fontCollection.AddFontFile(boxItemFromDB.Font!.FontPath);
        //            bodyFonts.Add(new Font(fontCollection.Families[0], (int)boxItemFromDB.FontSize));


        //            // Add color to list
        //            ColorConverter colorConverter = new();
        //            Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
        //            bodyColors.Add(color);
        //        }
        //    }
        //    if (bodyFonts.Count == 0) throw new Exception("Font not found or null");
        //    if (bodyColors.Count == 0) throw new Exception("Color not found or null");

        //    #endregion Initialize Fonts, Colors for products

        //    #region Get biggest string size from products

        //    string biggestString = "";
        //    float biggestStringWidth = 0f;
        //    SizeF biggestStringSize = new();

        //    foreach (var productGroup in productGroups)
        //    {
        //        foreach (var productGroupItem in productGroup.ProductGroupItems!)
        //        {
        //            var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName, bodyFonts[productGroups.IndexOf(productGroup)]).Width;

        //            if (tempWidth > biggestStringWidth)
        //            {
        //                biggestString = productGroupItem.Product!.ProductName;
        //                biggestStringWidth = tempWidth;
        //                biggestStringSize = g.MeasureString(biggestString, bodyFonts[productGroups.IndexOf(productGroup)]);
        //            }
        //        }
        //    }

        //    #endregion Get biggest string size from products

        //    #region Draw products within the display area

        //    foreach (var productGroup in productGroups)
        //    {
        //        // Get the starting point for the product
        //        var productPoint = productPoints[productGroups.IndexOf(productGroup)];

        //        // Get the rectangle for the product group
        //        var rect = rects[productGroups.IndexOf(productGroup)];

        //        foreach (var productGroupItem in productGroup.ProductGroupItems!)
        //        {
        //            // Check if the product can fit in the remaining display area
        //            if (productPoint.Y < rect.Bottom - biggestStringSize.Height)
        //            {
        //                if (productPoint.X + biggestStringSize.Width < rect.Right)
        //                {
        //                    // Draw the product name on the display
        //                    g.DrawString(productGroupItem.Product!.ProductName,
        //                        bodyFonts[productGroups.IndexOf(productGroup)],
        //                        new SolidBrush(bodyColors[productGroups.IndexOf(productGroup)]),
        //                        productPoint);
        //                }

        //                // Update the Y position for the next product
        //                productPoint.Y += biggestStringSize.Height; // Borrow biggestStringSize from "Get biggest string size from products" region
        //            }
        //        }
        //    }

        //    #endregion Draw products within the display area



        //    /*
        //     * 6. Draw product prices from product size prices
        //     */

        //    #region Draw products price within the display area

        //    // Intialize padding constants
        //    const int widthPaddingS = 10;
        //    const int priceHeightPadding = 100;
        //    const int sizeHeightPadding = 60;

        //    // get display items
        //    List<DisplayItem> displayItemsFromDB  = _unitOfWork.DisplayItemRepository.EnableQuery()
        //        .Where(c => c.DisplayId == data.DisplayId)
        //        .Include(c => c.ProductGroup!)
        //        .ThenInclude(c => c.ProductGroupItems!)
        //        .ThenInclude(c => c.Product!)
        //        .ThenInclude(c => c.ProductSizePrices)

        //        .Include(c => c.Box!)
        //        .ThenInclude(c => c.BoxItems!)
        //        .ThenInclude(c => c.Font)
        //        .ToList();

        //    foreach (var displayItem in displayItemsFromDB)
        //    {
        //        Box box = _unitOfWork.BoxRepository.Find(c => c.BoxId == displayItem.BoxId).FirstOrDefault()
        //            ?? throw new Exception("Box not found or deleted");

        //        // Get the rectangle for the displayItem
        //        var rect = rects[displayItems.IndexOf(displayItem)];
        //        //



        //        // Find the biggest product string width
        //        string BIGGEST_PRODUCT_STRING = "";
        //        float BIGGEST_PRODUCT_STRING_WIDTH = 0f;

        //        foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
        //        {
        //            BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

        //            var productFont = bodyFonts
        //                .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
        //                .FirstOrDefault();

        //            var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName,
        //                productFont!).Width;

        //            if (tempWidth >= g.MeasureString(BIGGEST_PRODUCT_STRING, productFont!).Width)
        //            {
        //                BIGGEST_PRODUCT_STRING = productGroupItem.Product!.ProductName;
        //                BIGGEST_PRODUCT_STRING_WIDTH = tempWidth;
        //            }
        //        }

        //        if (BIGGEST_PRODUCT_STRING == "") throw new Exception("BIGGEST_PRODUCT_STRING fail to initialize");
        //        //

        //        // Find the biggest price string height, width
        //        string BIGGEST_PRICE_STRING = "";
        //        float BIGGEST_PRICE_STRING_HEIGHT = 0f;
        //        float BIGGEST_PRICE_STRING_WIDTH = 0f;

        //        foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
        //        {
        //            BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

        //            var productPriceFont = bodyFonts
        //                .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
        //                .FirstOrDefault();

        //            foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
        //            {
        //                var tempHeight = g.MeasureString(productSizePrice.Price.ToString(),
        //                    productPriceFont!);

        //                if (tempHeight.Height >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceFont!).Height)
        //                {
        //                    BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
        //                    BIGGEST_PRICE_STRING_HEIGHT = tempHeight.Height;
        //                }

        //                if (tempHeight.Width >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceFont!).Width)
        //                {
        //                    BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
        //                    BIGGEST_PRICE_STRING_WIDTH = tempHeight.Width;
        //                }
        //            }
        //        }

        //        if (BIGGEST_PRICE_STRING == "") throw new Exception("BIGGEST_PRICE_STRING fail to initialize");
        //        //

        //        // Initialize Pointf for product size, prices based on biggest product string width; product size
        //        PointF pointPriceSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + priceHeightPadding);
        //        PointF pointPriceSizeM = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + priceHeightPadding);
        //        PointF pointPriceSizeL = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + priceHeightPadding);

        //        PointF pointSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + sizeHeightPadding);
        //        PointF pointSizeM = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + sizeHeightPadding);
        //        PointF pointSizeL = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + sizeHeightPadding);
        //        // 

        //        // Draw size of product 

        //        // Get the BoxItem for the product price
        //        BoxItem boxItemForSize = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

        //        // Convert the box color to a Color object
        //        boxItemForSize.BoxColor = boxItemForSize.BoxColor.Split("#").Last();
        //        Color sizeColor = bodyColors.Where(c => c.Name.Contains(boxItemForSize.BoxColor)).FirstOrDefault()!;

        //        // Get the font for the product price
        //        var productSizeFont = bodyFonts
        //                    .Where(c => c.Name == boxItemForSize.Font!.FontName || c.Size == boxItemForSize.FontSize)
        //                    .FirstOrDefault();

        //        // Intialize flag for product size
        //        bool isProductSizeSRendered = false;
        //        bool isProductSizeMRendered = false;
        //        bool isProductSizeLRendered = false;

        //        // Draw product prices & size
        //        foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
        //        {
        //            foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
        //            {
        //                // Get the BoxItem for the product price
        //                BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

        //                // Convert the box color to a Color object
        //                boxItem.BoxColor = boxItem.BoxColor.Split("#").Last();
        //                Color color = bodyColors.Where(c => c.Name.Contains(boxItem.BoxColor)).FirstOrDefault()!;

        //                // Get the font for the product price
        //                var productPriceFont = bodyFonts
        //                            .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
        //                            .FirstOrDefault();

        //                // Check if there is enough space to draw the product price
        //                if (pointPriceSizeS.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
        //                {

        //                    // Draw price for product size Normal
        //                    if (productSizePrice.ProductSizeType == ProductSizeType.Normal && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
        //                    {

        //                        // Draw the product price on the display
        //                        g.DrawString(productSizePrice.Price.ToString(),
        //                            productPriceFont!,
        //                            new SolidBrush(color),
        //                            pointPriceSizeS);

        //                        pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;
        //                    }

        //                    // Draw price for product size S
        //                    if (productSizePrice.ProductSizeType == ProductSizeType.S && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
        //                    {
        //                        // Draw the product size title if not render
        //                        if (isProductSizeSRendered == false)
        //                        {
        //                            // Make the text go between the price number
        //                            pointSizeS.X += BIGGEST_PRICE_STRING_WIDTH /4;

        //                            g.DrawString(ProductSizeType.S.ToString(),
        //                                productSizeFont!,
        //                                new SolidBrush(sizeColor),
        //                                pointSizeS
        //                                );
        //                            isProductSizeSRendered = true;


        //                        }

        //                        // Draw the product price on the display
        //                        g.DrawString(productSizePrice.Price.ToString(),
        //                            productPriceFont!,
        //                            new SolidBrush(color),
        //                            pointPriceSizeS);

        //                        pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;
        //                    }

        //                    // Draw price for product size M
        //                    if (productSizePrice.ProductSizeType == ProductSizeType.M && pointPriceSizeM.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
        //                    {
        //                        // Draw the product size title if not render
        //                        if (isProductSizeMRendered == false)
        //                        {
        //                            // Make the text go between the price number
        //                            pointSizeM.X += BIGGEST_PRICE_STRING_WIDTH / 4;

        //                            g.DrawString(ProductSizeType.M.ToString(),
        //                                productSizeFont!,
        //                                new SolidBrush(sizeColor),
        //                                pointSizeM
        //                                );
        //                            isProductSizeMRendered = true;
        //                        }

        //                        // Draw the product price on the display
        //                        g.DrawString(productSizePrice.Price.ToString(),
        //                            productPriceFont!,
        //                            new SolidBrush(color),
        //                            pointPriceSizeM);

        //                        pointPriceSizeM.Y += BIGGEST_PRICE_STRING_HEIGHT;
        //                    }

        //                    // Draw price for product size L
        //                    if (productSizePrice.ProductSizeType == ProductSizeType.L && pointPriceSizeL.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
        //                    {
        //                        // Draw the product size title if not render
        //                        if (isProductSizeLRendered == false)
        //                        {
        //                            // Make the text go between the price number
        //                            pointSizeL.X += BIGGEST_PRICE_STRING_WIDTH / 4;

        //                            g.DrawString(ProductSizeType.L.ToString(),
        //                                productSizeFont!,
        //                                new SolidBrush(sizeColor),
        //                                pointSizeL
        //                                );
        //                            isProductSizeLRendered = true;
        //                        }

        //                        // Draw the product price on the display
        //                        g.DrawString(productSizePrice.Price.ToString(),
        //                            productPriceFont!,
        //                            new SolidBrush(color),
        //                            pointPriceSizeL);

        //                        pointPriceSizeL.Y += BIGGEST_PRICE_STRING_HEIGHT;
        //                    }
        //                }
        //                //
        //            }
        //        }
        //    }

        //    #endregion Draw products price within the display area



        //    /*
        //    * FINALLY: Save image
        //    */
        //    b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png");
        //    b.Dispose();

        //    return $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png";
        //}








        //private static void DrawImageLayerV2(Bitmap b, Graphics g, SmartMenu.Domain.Models.Layer layer, string tempPath)
        //{
        //    try
        //    {
        //        // 1. Draw background
        //        if (layer.LayerType == LayerType.BackgroundImageLayer)
        //        {
        //            //var image = System.Drawing.Image.FromFile(layer.LayerItem!.LayerItemValue);
        //            var image = InitializeImage(tempPath, layer.LayerItem!);
        //            g.DrawImage(image, 0, 0, b.Width, b.Height);
        //        }


        //        // 2. Draw image
        //        if (layer.LayerType == LayerType.ImageLayer)
        //        {
        //            var image = System.Drawing.Image.FromFile(layer.LayerItem!.LayerItemValue)
        //                         ?? throw new Exception($"Image not found: {layer.LayerItem.LayerItemValue}");

        //            var box = layer.Boxes!.FirstOrDefault() ?? throw new Exception($"Layer ID: {layer.LayerId} have no box!");
        //            var rect = new Rectangle((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);

        //            g.DrawRectangle(Pens.Black, rect);
        //            g.DrawImage(image, rect);
                    
        //            image.Dispose(); 
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToStringDemystified());
        //    }
        //}

        public void DeleteTempFile(string tempPath)
        {
            if (!Directory.Exists(tempPath))
            {
                throw new DirectoryNotFoundException($"Temporary directory not found: {tempPath}");
            }

            var files = Directory.EnumerateFiles(tempPath); // More efficient for large directories

            foreach (var file in files)
            {
                try
                {
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

        private static void DisposeFontCache(Dictionary<string, Font> fontCache)
        {
            foreach (var font in fontCache.Values)
            {
                font.Dispose();
            }

            fontCache.Clear(); // Optional, but good practice to release references
        }
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

    }
}

#pragma warning restore CA1416 // Validate platform compatibility