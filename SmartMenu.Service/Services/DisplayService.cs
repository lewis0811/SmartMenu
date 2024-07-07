using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System.Drawing;
using System.Drawing.Text;
using System.Net;
using Font = System.Drawing.Font;

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

        public Display AddDisplayV2(DisplayCreateDTO displayCreateDTO)
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

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            // Adding display items
            var productGroups = new List<ProductGroup>();
            var boxes = new List<Box>();
            var layers = new List<Layer>();
            var templateWithLayer = new Template();
            var displayItems = new List<DisplayItem>();

            // GET ProductGroup List from Menu or Collection if not null
            if (menu != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, menu.MenuId, null);
            }

            if (collection != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, null, collection.CollectionId);
            }

            // GET Box List from display's template
            templateWithLayer = _unitOfWork.TemplateRepository.GetTemplateWithLayersAndBoxes(template.TemplateId);

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
                boxes = boxes.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay).ToList();
            }

            // Get display items list from product groups and boxes
            int productGroupCount = productGroups.Count;
            int boxCount = boxes.Count;
            //var boxesToRender = boxes.Where(c => c.)

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

            /*
             *
             * BEGIN RENDERING DATA
             *
             */

            /* Intialize real res */
            Template templateResolution;
            float templateWidth;
            float templateHeight;

            StoreDevice storeDeviceResolution;
            float deviceWidth;
            float deviceHeight;

            Template initializeDisplay;

            // Get template resolutions
            templateResolution = _unitOfWork.TemplateRepository
                .EnableQuery()
                .Include(c => c.Layers!)
                .ThenInclude(c => c.Boxes!)
                .ThenInclude(c => c.BoxItems!)
                .ThenInclude(c => c.Font)
                .Where(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Template not found or deleted");

            if (templateResolution.Layers == null) throw new Exception("Template has no layers");

            templateWidth = templateResolution.TemplateWidth;
            templateHeight = templateResolution.TemplateHeight;

            // Get device resolutions
            storeDeviceResolution = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store device not found or deleted");

            deviceWidth = storeDeviceResolution.DeviceWidth;
            deviceHeight = storeDeviceResolution.DeviceHeight;

            // Initialize display
            initializeDisplay = GetInitializeTemplate(templateResolution, storeDeviceResolution);
            if (initializeDisplay.Layers == null) throw new Exception("Template has no layers");

            /* Start render */
            Bitmap b = new((int)initializeDisplay.TemplateWidth, (int)initializeDisplay.TemplateHeight);
            using Graphics g = Graphics.FromImage(b);

            /*
            * Draw image layer
            */
            foreach (var layer in initializeDisplay.Layers)
            {
                DrawImageLayer(menu, collection, b, g, layer);
            }

            /*
             * Draw render layer
             */

            // Get display
            Display displayRender = _unitOfWork.DisplayRepository
                .EnableQuery()
                .Include(c => c.DisplayItems!)
                .ThenInclude(c => c.ProductGroup!)
                .ThenInclude(c => c.ProductGroupItems!)
                .ThenInclude(c => c.Product)
                .ThenInclude(c => c!.ProductSizePrices)

                .Include(c => c.DisplayItems!)
                .ThenInclude(c => c.Box)
                .ThenInclude(c => c!.BoxItems!)
                .ThenInclude(c => c.Font)
                .Where(c => c.DisplayId == data.DisplayId).FirstOrDefault()
                ?? throw new Exception("Display not found in draw render layer");

            if (displayRender.DisplayItems == null) throw new Exception("Display item not found in draw render layer");

            /*
             *  Initialize displayitem (box and productgroup)
             */
            //Rectangle boxRectFather = new ();

            foreach (var displayItem in displayRender.DisplayItems)
            {
                // Config rectangle
                Rectangle boxRect = new((int)displayItem.Box!.BoxPositionX, (int)displayItem.Box.BoxPositionY, (int)displayItem.Box.BoxWidth, (int)displayItem.Box.BoxHeight);

                //Initialize Header position
                PointF headerPosition = new((int)displayItem.Box.BoxPositionX, (int)displayItem.Box.BoxPositionY);

                // Initialize Font
                FontFamily? headerFontFamily = null;
                FontFamily? bodyFontFamily = null;
                float headerSize = 0;
                float bodySize = 0;
                string headerColorHex = "";
                string bodyColorHex = "";

                // Config color
                ColorConverter converter = new();
                Color headerColor;
                Color bodyColor = new();

                /*
                 *  Initialize productgroup into box and draw box header
                 */
                foreach (var boxItem in displayItem.Box.BoxItems!)
                {
                    /* Check if the box item is of type Header and Draw the Box Header*/
                    if (boxItem.BoxItemType == BoxItemType.Header)
                    {
                        // Get the header size
                        headerSize = (float)boxItem.FontSize;

                        // Get the header text color
                        headerColorHex = boxItem.BoxColor;

                        // Initialize the header text color
                        headerColor = (Color)converter.ConvertFromString(headerColorHex)!;

                        // Get the header font
                        PrivateFontCollection fontCollection = new();
                        //var fontPath = $@"{boxItem.Font!.FontPath}";
                        fontCollection.AddFontFile($@"{boxItem.Font!.FontPath}");

                        headerFontFamily = new(fontCollection.Families.FirstOrDefault()!.Name, fontCollection);
                        Font headerFont = new(headerFontFamily, headerSize, FontStyle.Bold);

                        fontCollection.Dispose();

                        // Draw the Box Header
                        g.DrawString(displayItem.ProductGroup!.ProductGroupName.ToUpper()
                        , headerFont
                        , new SolidBrush(headerColor)
                        , headerPosition
                        , new StringFormat() { Alignment = boxItem.TextFormat }
                        );
                    }

                    /* Check if the box item is of type Body */
                    if (boxItem.BoxItemType == BoxItemType.Body)
                    {
                        // Get the body size
                        bodySize = (float)boxItem.FontSize;

                        // Get the font size
                        PrivateFontCollection fontCollection = new();
                        fontCollection.AddFontFile($@"{boxItem.Font!.FontPath}");

                        bodyFontFamily = new(fontCollection.Families.FirstOrDefault()!.Name, fontCollection);
                        Font font = new(bodyFontFamily, bodySize, FontStyle.Bold);

                        // Get the body text color
                        bodyColorHex = boxItem.BoxColor;

                        // Initialize the body text color
                        bodyColor = (Color)converter.ConvertFromString(bodyColorHex)!;
                    }
                }

                if (headerFontFamily == null) throw new Exception("Header font fail to initialized");
                if (bodyFontFamily == null) throw new Exception("Body font fail to initialized");

                //Color headerColor = (Color)converter.ConvertFromString(headerColorHex)!;

                /*
                 *  Initialize productgroup into box and draw box body content
                 */
                // Draw Box Content
                //var boxPaddingRect = boxRect;
                //var boxMoneyPaddingRect = boxRect;

                //boxPaddingRect.X += 30;
                //boxPaddingRect.Y += 50;
                //boxMoneyPaddingRect.X = boxPaddingRect.X + 450;
                //boxMoneyPaddingRect.Y += 50;

                //var boxMoneyPaddingXHolder = boxMoneyPaddingRect;

                // Initialize Font
                Font bodyFont = new(bodyFontFamily, bodySize);

                // Intialize Brush
                Brush brush = new SolidBrush(bodyColor);

                // Initialize the biggest string length
                var biggestString = "";
                var biggestStringWidth = 0f;

                foreach (var productHolder in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    // Get the biggest string and biggest string width
                    if (g.MeasureString(productHolder.Product!.ProductName, bodyFont).Width > g.MeasureString(biggestString, bodyFont).Width)
                    {
                        biggestString = productHolder.Product!.ProductName;
                        biggestStringWidth = g.MeasureString(productHolder.Product!.ProductName, bodyFont).Width;
                    }
                }

                var tempPricePadding = 10;
                boxRect.Height += 100;
                PointF bodyPosition = new(boxRect.X, boxRect.Y + 100); // + 100 to padding from header
                PointF pricePosition = new(boxRect.X + biggestStringWidth + tempPricePadding, boxRect.Y + 100); // + 100 to padding from header

                var tempPriceWidth = 0f;
                //g.DrawRectangle(Pens.Black, boxRect2);
                foreach (var productHolder in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    foreach (var productPrice in productHolder.Product!.ProductSizePrices!)
                    {
                        SizeF priceSize = g.MeasureString(productPrice.Price.ToString(), bodyFont);
                        if (priceSize.Width > tempPriceWidth) tempPriceWidth = priceSize.Width;
                    }
                }

                // Get price position 2 and 3
                PointF pricePosition2 = new(pricePosition.X + tempPriceWidth, pricePosition.Y);
                PointF pricePosition3 = new(pricePosition.X + tempPriceWidth * 2, pricePosition.Y);

                foreach (var productHolder in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    SizeF bodyTextSize = g.MeasureString(productHolder.Product!.ProductName, bodyFont);

                    if (bodyPosition.Y <= boxRect.Bottom - bodyTextSize.Height)
                    {
                        g.DrawString(productHolder.Product!.ProductName.TrimStart()
                             , bodyFont
                            , brush
                            , bodyPosition
                        );

                        bodyPosition.Y += bodyTextSize.Height;
                        boxRect.Height += (int)bodyPosition.Y;
                        g.DrawRectangle(new Pen(Color.Red), boxRect);
                    }

                    foreach (var productPrice in productHolder.Product.ProductSizePrices!)
                    {
                        if (pricePosition.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.Normal)
                        {
                            g.DrawString(productPrice.Price.ToString(),
                                bodyFont,
                                brush,
                                pricePosition);
                            pricePosition.Y += bodyTextSize.Height;
                        }

                        if (pricePosition.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.S)
                        {
                            g.DrawString(productPrice.Price.ToString(),
                                bodyFont,
                                brush,
                                pricePosition);
                            pricePosition.Y += bodyTextSize.Height;
                        }

                        if (pricePosition2.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.M)
                        {
                            g.DrawString(productPrice.Price.ToString(),
                                bodyFont,
                                brush,
                                pricePosition2);
                            pricePosition2.Y += bodyTextSize.Height;
                        }

                        if (pricePosition3.X + tempPriceWidth <= boxRect.Right && productPrice.ProductSizeType == ProductSizeType.L)
                        {
                            g.DrawString(productPrice.Price.ToString(),
                                bodyFont,
                                brush,
                                pricePosition3);
                            pricePosition3.Y += bodyTextSize.Height;
                        }
                    }
                }
            }

            // Save image
            b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\text.png");
            b.Dispose();

            return data;
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

            return data;
        }

        public void Delete(int displayId)
        {
            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Display not found or deleted");

            _unitOfWork.DisplayRepository.Remove(data);
            _unitOfWork.Save();
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

        private static Template GetInitializeTemplate(Template templateResolution, StoreDevice storeDeviceResolution)
        {
            var templateWidth = templateResolution.TemplateWidth;
            var templateHeight = templateResolution.TemplateHeight;
            var deviceWidth = storeDeviceResolution.DeviceWidth;
            var deviceHeight = storeDeviceResolution.DeviceHeight;

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

        private static void DrawImageLayer(Menu? menu, Collection? collection, Bitmap b, Graphics g, Layer layer)
        {
            // Draw background
            if (layer.LayerType == LayerType.BackgroundImageLayer)
            {
                var image = System.Drawing.Image.FromFile(layer.LayerItem!.LayerItemValue);
                g.DrawImage(image, 0, 0, b.Width, b.Height);
            }

            // Draw image
            //if (layer.LayerType == LayerType.ImageLayer)
            //{
            //    foreach (var box in layer.Boxes!)
            //    {
            //        var rectf = new RectangleF(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
            //        var rect = new Rectangle((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);

            //        // Lưu ý lỗi file hình : Out of memory tên phía sau bắt đầu "....-10212.jpg"
            //        var image = Image.FromFile(layer.LayerItem!.LayerItemValue)
            //            ?? throw new Exception($"Image not found: {layer.LayerItem.LayerItemValue}");

            //        g.DrawRectangle(Pens.Black, rect);
            //        g.DrawImage(image, rectf);
            //    }
            //}

            // Draw title
            if (layer.LayerType == LayerType.MenuCollectionNameLayer)
            {
                var box = layer.Boxes!.FirstOrDefault() ?? throw new Exception("Box not found in draw title layer");

                RectangleF boxRectTitleF = new(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
                Rectangle boxRectTitle = new((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);
                // Get name
                string name = "";
                if (menu != null) name = menu.MenuName;
                if (collection != null) name = collection.CollectionName;

                // Get BoxItem
                BoxItem boxItem = box.BoxItems!.FirstOrDefault()
                    ?? throw new Exception("Box Item not found in draw title layer");

                // Get Font
                Domain.Models.Font font = boxItem.Font
                    ?? throw new Exception("Font not found in draw title layer");

                // Config font
                PrivateFontCollection fontCollection = new();
                //fontCollection.AddFontFile(@"C:\Users\tekat\OneDrive\Desktop\Lilita_One\LilitaOne.ttf");
                var fontPath = $@"{font.FontPath}";
                fontCollection.AddFontFile($@"{font.FontPath}");
                FontFamily fontFamily = new(fontCollection.Families.FirstOrDefault()!.Name, fontCollection);

                // Config color
                ColorConverter converter = new();
                Color color = (Color)converter.ConvertFromString(boxItem.BoxColor)!;

                // Draw menu name
                //g.DrawString(name
                //    , new System.Drawing.Font(fontFamily, (float)box.BoxItems!.First().FontSize, FontStyle.Bold)
                //    , new SolidBrush(Color.Black)
                //    , boxRectTitleF
                //    , new StringFormat() { Alignment = boxItem.TextFormat }
                //    );

                //g.DrawRectangle(Pens.Black, boxRectTitle);
            }
        }

        public Display AddDisplayV3(DisplayCreateDTO displayCreateDTO)
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

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            // Adding display items
            var productGroups = new List<ProductGroup>();
            var boxes = new List<Box>();
            var layers = new List<Layer>();
            var templateWithLayer = new Template();
            var displayItems = new List<DisplayItem>();

            // GET ProductGroup List from Menu or Collection if not null
            if (menu != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, menu.MenuId, null);
            }

            if (collection != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, null, collection.CollectionId);
            }

            // GET Box List from display's template
            templateWithLayer = _unitOfWork.TemplateRepository.GetTemplateWithLayersAndBoxes(template.TemplateId);

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
                boxes = boxes.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay).ToList();
            }

            // Get display items list from product groups and boxes
            int productGroupCount = productGroups.Count;
            int boxCount = boxes.Count;
            //var boxesToRender = boxes.Where(c => c.)

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

            InitializeImage(data);
            return data;
        }

        private string InitializeImage(Display data)
        {
            /*
             * 0. Initialize display
             */

            #region Initialize Template
            // Get template resolutions
            var templateResolution = _unitOfWork.TemplateRepository
                .EnableQuery()
                .Include(c => c.Layers!)
                .ThenInclude(c => c.Boxes!)
                .ThenInclude(c => c.BoxItems!)
                .ThenInclude(c => c.Font)
                .Where(c => c.TemplateId == data.TemplateId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Template not found or deleted");

            if (templateResolution.Layers == null) throw new Exception("Template has no layers");

            // Get device resolutions
            var storeDeviceResolution = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == data.StoreDeviceId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store device not found or deleted");

            // Initialize template
            var initializeTemplate = GetInitializeTemplate(templateResolution, storeDeviceResolution);
            if (initializeTemplate.Layers == null) throw new Exception("Template has no layers");
            #endregion

            /*
             * 1. Initialize image bitmap
             */

            #region Get template from display

            Template template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == data.TemplateId).FirstOrDefault()
                ?? throw new Exception("Template not found");

            #endregion Get template from display

            #region Generate bitmap

            Bitmap b = new((int)initializeTemplate.TemplateWidth, (int)initializeTemplate.TemplateHeight);
            using Graphics g = Graphics.FromImage(b);

            #endregion Generate bitmap

            /*
             * 2. Draw image layer
             */

            #region Initialize menu, collection
            var menu = new Menu();
            if (data.MenuId != null)
            {
                menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == data.MenuId && c.IsDeleted == false).FirstOrDefault();
            }
            else { menu = null; }

            var collection = new Collection();
            if (data.CollectionId != null)
            {
                collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == data.CollectionId && c.IsDeleted == false).FirstOrDefault();
            }
            else { collection = null; }
            #endregion

            #region Draw image from template layers
            foreach (var layer in initializeTemplate!.Layers!)
            {
                DrawImageLayer(menu, collection, b, g, layer);
            }
            #endregion

            /*
             * 3. Draw display box from display item
             */

            #region Get display items from display

            List<DisplayItem> displayItems = _unitOfWork.DisplayItemRepository
                .Find(c => c.DisplayId == data.DisplayId)
                .ToList();
            if (displayItems.Count == 0) throw new Exception("Display items not found or null");

            #endregion Get display items from display

            #region Get boxes from display items
            // CONTINUE HERE -> IMAGE RENDER
            List<Box> boxes = new();
            foreach (var item in displayItems)
            {
                boxes.Add(_unitOfWork.BoxRepository
                    .Find(c => c.BoxId == item.BoxId)
                    .FirstOrDefault()
                    ?? throw new Exception("Box not found or null")
                );
            }
            if (boxes.Count == 0) throw new Exception("Box not found or null");

            #endregion Get boxes from display items

            #region Initialize rectangle from boxes

            List<Rectangle> rects = new();
            foreach (var item in boxes)
            {
                Rectangle rect = new((int)item.BoxPositionX, (int)item.BoxPositionY, (int)item.BoxWidth, (int)item.BoxHeight);
                rects.Add(rect);
            }
            if (rects.Count == 0) throw new Exception("Rectangle fail to initialize");

            #endregion Initialize rectangle from boxes

            #region Draw rectangles from boxes

            foreach (var rect in rects)
            {
                g.DrawRectangle(Pens.Red, rect);
            }

            #endregion Draw rectangles from boxes

            /*
             * 4. Draw header from productgroup
             */

            #region Get product group from display item

            List<ProductGroup> productGroups = new();

            foreach (var item in displayItems)
            {
                productGroups.Add(_unitOfWork.ProductGroupRepository
                    .Find(c => c.ProductGroupId == item.ProductGroupId)
                    .FirstOrDefault()
                    ?? throw new Exception("Product group not found or null")
                );
            }
            if (productGroups.Count == 0) throw new Exception("Product group not found or null");

            #endregion Get product group from display item

            #region Initialize PointF for headers

            List<PointF> headerPoints = new();

            // Get postion x,y from boxes in step 2
            foreach (var item in boxes)
            {
                PointF point = new((int)item.BoxPositionX, (int)item.BoxPositionY);
                headerPoints.Add(point);
            }
            if (headerPoints.Count == 0) throw new Exception("Header point fail to initialize");

            #endregion Initialize PointF for headers

            #region Initialize Fonts, Colors for headers

            List<Font> headerFonts = new();
            List<Color> headerColors = new();

            // Get box items from boxes
            List<BoxItem> boxItems = new();

            foreach (var item in boxes)
            {
                boxItems.AddRange(_unitOfWork.BoxItemRepository
                    .Find(c => c.BoxId == item.BoxId)
                    .ToList()
                );
            }
            if (boxItems.Count == 0) throw new Exception("Box items not found or null");

            // Get fonts from box items
            foreach (var item in boxItems)
            {
                if (item.BoxItemType == BoxItemType.Header)
                {
                    var boxItemFromDB = _unitOfWork.BoxItemRepository
                        .EnableQuery()
                        .Include(c => c.Font)
                        .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
                        .FirstOrDefault()
                        ?? throw new Exception("Box item not found or deleted");

                    // Add font to list
                    PrivateFontCollection fontCollection = new();
                    fontCollection.AddFontFile(boxItemFromDB.Font!.FontPath);
                    headerFonts.Add(new Font(fontCollection.Families[0], (int)boxItemFromDB.FontSize));


                    // Add color to list
                    ColorConverter colorConverter = new();
                    Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
                    headerColors.Add(color);
                }
            }
            if (headerFonts.Count == 0) throw new Exception("Font not found or null");
            if (headerColors.Count == 0) throw new Exception("Color not found or null");

            #endregion Initialize Fonts, Colors for headers

            #region Draw header from product group

            foreach (var productGroup in productGroups)
            {
                g.DrawString(productGroup.ProductGroupName,
                    headerFonts[productGroups.IndexOf(productGroup)],
                    new SolidBrush(headerColors[productGroups.IndexOf(productGroup)]),
                    headerPoints[productGroups.IndexOf(productGroup)]
                    );
            }

            #endregion Draw header from product group

            /*
             * 5. Draw product name from productgroup
             */

            #region Get product from menu / collection in display

            // Initialize padding constants
            const int heightPadding = 100;

            // Initialize product group item
            List<ProductGroupItem> productGroupItems = new();

            // Get product group items from product groups
            foreach (var productGroup in productGroups)
            {
                productGroupItems.AddRange(_unitOfWork.ProductGroupItemRepository
                    .Find(c => c.ProductGroupId == productGroup.ProductGroupId)
                    .ToList()
                );
            }
            if (productGroupItems.Count == 0) throw new Exception("Product group item not found or null");

            // Get product from product group items
            List<Product> products = new();
            foreach (var productGroupItem in productGroupItems)
            {
                products.Add(_unitOfWork.ProductRepository
                    .Find(c => c.ProductId == productGroupItem.ProductId)
                    .FirstOrDefault()
                    ?? throw new Exception("Product not found or null")
                );
            }
            if (products.Count == 0) throw new Exception("Product not found or null");

            #endregion Get product from menu / collection in display

            #region Initialize PointF for products

            List<PointF> productPoints = new();

            // Get postion x,y from boxes in step 2
            foreach (var item in boxes)
            {
                PointF point = new((int)item.BoxPositionX, (int)item.BoxPositionY + heightPadding);
                productPoints.Add(point);
            }
            if (productPoints.Count == 0) throw new Exception("Product point fail to initialize");

            #endregion Initialize PointF for products

            #region Initialize Fonts, Colors for products

            List<Font> bodyFonts = new();
            List<Color> bodyColors = new();

            // Get box items from boxes from step 3
            List<BoxItem> bodyBoxItems = new();
            bodyBoxItems = boxItems;

            // Get fonts from box items
            foreach (var item in bodyBoxItems)
            {
                if (item.BoxItemType == BoxItemType.Body)
                {
                    var boxItemFromDB = _unitOfWork.BoxItemRepository
                        .EnableQuery()
                        .Include(c => c.Font)
                        .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
                        .FirstOrDefault()
                        ?? throw new Exception("Box item not found or deleted");

                    // Add font to list
                    PrivateFontCollection fontCollection = new();
                    fontCollection.AddFontFile(boxItemFromDB.Font!.FontPath);
                    bodyFonts.Add(new Font(fontCollection.Families[0], (int)boxItemFromDB.FontSize));


                    // Add color to list
                    ColorConverter colorConverter = new();
                    Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
                    bodyColors.Add(color);
                }
            }
            if (bodyFonts.Count == 0) throw new Exception("Font not found or null");
            if (bodyColors.Count == 0) throw new Exception("Color not found or null");

            #endregion Initialize Fonts, Colors for products

            #region Get biggest string size from products

            string biggestString = "";
            float biggestStringWidth = 0f;
            SizeF biggestStringSize = new();

            foreach (var productGroup in productGroups)
            {
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
            }

            #endregion Get biggest string size from products

            #region Draw products within the display area

            foreach (var productGroup in productGroups)
            {
                // Get the starting point for the product
                var productPoint = productPoints[productGroups.IndexOf(productGroup)];

                // Get the rectangle for the product group
                var rect = rects[productGroups.IndexOf(productGroup)];

                foreach (var productGroupItem in productGroup.ProductGroupItems!)
                {
                    // Check if the product can fit in the remaining display area
                    if (productPoint.Y < rect.Bottom - biggestStringSize.Height)
                    {
                        if (productPoint.X + biggestStringSize.Width < rect.Right)
                        {
                            // Draw the product name on the display
                            g.DrawString(productGroupItem.Product!.ProductName,
                                bodyFonts[productGroups.IndexOf(productGroup)],
                                new SolidBrush(bodyColors[productGroups.IndexOf(productGroup)]),
                                productPoint);
                        }

                        // Update the Y position for the next product
                        productPoint.Y += biggestStringSize.Height; // Borrow biggestStringSize from "Get biggest string size from products" region
                    }
                }
            }

            #endregion Draw products within the display area



            /*
             * 6. Draw product prices from product size prices
             */

            #region Draw products price within the display area

            // Intialize padding constants
            const int widthPaddingS = 10;
            const int priceHeightPadding = 100;
            const int sizeHeightPadding = 60;

            // get display items
            List<DisplayItem> displayItemsFromDB  = _unitOfWork.DisplayItemRepository.EnableQuery()
                .Where(c => c.DisplayId == data.DisplayId)
                .Include(c => c.ProductGroup!)
                .ThenInclude(c => c.ProductGroupItems!)
                .ThenInclude(c => c.Product!)
                .ThenInclude(c => c.ProductSizePrices)

                .Include(c => c.Box!)
                .ThenInclude(c => c.BoxItems!)
                .ThenInclude(c => c.Font)
                .ToList();

            foreach (var displayItem in displayItemsFromDB)
            {
                Box box = _unitOfWork.BoxRepository.Find(c => c.BoxId == displayItem.BoxId).FirstOrDefault()
                    ?? throw new Exception("Box not found or deleted");

                // Get the rectangle for the displayItem
                var rect = rects[displayItems.IndexOf(displayItem)];
                //



                // Find the biggest product string width
                string BIGGEST_PRODUCT_STRING = "";
                float BIGGEST_PRODUCT_STRING_WIDTH = 0f;

                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                    var productFont = bodyFonts
                        .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
                        .FirstOrDefault();

                    var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName,
                        productFont).Width;

                    if (tempWidth >= g.MeasureString(BIGGEST_PRODUCT_STRING, productFont).Width)
                    {
                        BIGGEST_PRODUCT_STRING = productGroupItem.Product!.ProductName;
                        BIGGEST_PRODUCT_STRING_WIDTH = tempWidth;
                    }
                }

                if (BIGGEST_PRODUCT_STRING == "") throw new Exception("BIGGEST_PRODUCT_STRING fail to initialize");
                //

                // Find the biggest price string height, width
                string BIGGEST_PRICE_STRING = "";
                float BIGGEST_PRICE_STRING_HEIGHT = 0f;
                float BIGGEST_PRICE_STRING_WIDTH = 0f;

                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                    var productPriceFont = bodyFonts
                        .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
                        .FirstOrDefault();

                    foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                    {
                        var tempHeight = g.MeasureString(productSizePrice.Price.ToString(),
                            productPriceFont);

                        if (tempHeight.Height >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceFont).Height)
                        {
                            BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                            BIGGEST_PRICE_STRING_HEIGHT = tempHeight.Height;
                        }

                        if (tempHeight.Width >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceFont).Width)
                        {
                            BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                            BIGGEST_PRICE_STRING_WIDTH = tempHeight.Width;
                        }
                    }
                }

                if (BIGGEST_PRICE_STRING == "") throw new Exception("BIGGEST_PRICE_STRING fail to initialize");
                //

                // Initialize Pointf for product size, prices based on biggest product string width; product size
                PointF pointPriceSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + priceHeightPadding);
                PointF pointPriceSizeM = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + priceHeightPadding);
                PointF pointPriceSizeL = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + priceHeightPadding);

                PointF pointSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + sizeHeightPadding);
                PointF pointSizeM = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + sizeHeightPadding);
                PointF pointSizeL = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + sizeHeightPadding);
                // 

                // Draw size of product 

                // Get the BoxItem for the product price
                BoxItem boxItemForSize = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                // Convert the box color to a Color object
                boxItemForSize.BoxColor = boxItemForSize.BoxColor.Split("#").Last();
                Color sizeColor = bodyColors.Where(c => c.Name.Contains(boxItemForSize.BoxColor)).FirstOrDefault()!;

                // Get the font for the product price
                var productSizeFont = bodyFonts
                            .Where(c => c.Name == boxItemForSize.Font!.FontName || c.Size == boxItemForSize.FontSize)
                            .FirstOrDefault();

                // Intialize flag for product size
                bool isProductSizeSRendered = false;
                bool isProductSizeMRendered = false;
                bool isProductSizeLRendered = false;

                // Draw product prices & size
                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                    {
                        // Get the BoxItem for the product price
                        BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                        // Convert the box color to a Color object
                        boxItem.BoxColor = boxItem.BoxColor.Split("#").Last();
                        Color color = bodyColors.Where(c => c.Name.Contains(boxItem.BoxColor)).FirstOrDefault()!;

                        // Get the font for the product price
                        var productPriceFont = bodyFonts
                                    .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
                                    .FirstOrDefault();

                        // Check if there is enough space to draw the product price
                        if (pointPriceSizeS.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                        {

                            // Draw price for product size Normal
                            if (productSizePrice.ProductSizeType == ProductSizeType.Normal && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                            {

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeS);

                                pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;
                            }

                            // Draw price for product size S
                            if (productSizePrice.ProductSizeType == ProductSizeType.S && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                            {
                                // Draw the product size title if not render
                                if (isProductSizeSRendered == false)
                                {
                                    // Make the text go between the price number
                                    pointSizeS.X += BIGGEST_PRICE_STRING_WIDTH /4;

                                    g.DrawString(ProductSizeType.S.ToString(),
                                        productSizeFont,
                                        new SolidBrush(sizeColor),
                                        pointSizeS
                                        );
                                    isProductSizeSRendered = true;


                                }

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeS);

                                pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;
                            }

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

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeM);

                                pointPriceSizeM.Y += BIGGEST_PRICE_STRING_HEIGHT;
                            }

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

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeL);

                                pointPriceSizeL.Y += BIGGEST_PRICE_STRING_HEIGHT;
                            }
                        }
                        //
                    }
                }
            }

            #endregion Draw products price within the display area



            /*
            * FINALLY: Save image
            */
            b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png");
            b.Dispose();

            return $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png";
        }

        public string GetImageById(int displayId)
        {
            Display display = _unitOfWork.DisplayRepository.EnableQuery()
                .Where(c => c.DisplayId == displayId && c.IsDeleted == false)
                .Include(c => c.Menu!)
                .Include(c => c.Collection!)
                .Include(c =>c.Template!)
                .ThenInclude(c => c.Layers!)
                .ThenInclude(c => c.LayerItem)
                .Include(c => c.DisplayItems)
                .FirstOrDefault() 
                ?? throw new Exception("Display not found or deleted");

            string imgPath = InitializeImage(display);

            return imgPath;

        }
        public string UpdateContainImage(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            Display updateDisplay = Update(displayId, displayUpdateDTO) 
                ?? throw new Exception("Display fail to update");

            Display display = _unitOfWork.DisplayRepository.EnableQuery()
                .Where(c => c.DisplayId == displayId && c.IsDeleted == false)
                .Include(c => c.Menu!)
                .Include(c => c.Collection!)
                .Include(c => c.Template!)
                .ThenInclude(c => c.Layers!)
                .ThenInclude(c => c.LayerItem)
                .Include(c => c.DisplayItems)
                .FirstOrDefault()
                ?? throw new Exception("Display not found or deleted");

            string imgPath = InitializeImage(display);
            return imgPath;

        }

        public Display AddDisplayV4(DisplayCreateDTO displayCreateDTO, string tempPath)
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

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            // Adding display items
            var productGroups = new List<ProductGroup>();
            var boxes = new List<Box>();
            var layers = new List<Layer>();
            var templateWithLayer = new Template();
            var displayItems = new List<DisplayItem>();

            // GET ProductGroup List from Menu or Collection if not null
            if (menu != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, menu.MenuId, null);
            }

            if (collection != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, null, collection.CollectionId);
            }

            // GET Box List from display's template
            templateWithLayer = _unitOfWork.TemplateRepository.GetTemplateWithLayersAndBoxes(template.TemplateId);

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
                boxes = boxes.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay).ToList();
            }

            // Get display items list from product groups and boxes
            int productGroupCount = productGroups.Count;
            int boxCount = boxes.Count;
            //var boxesToRender = boxes.Where(c => c.)

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

            InitializeImageV2(data, tempPath);
            return data;
        }

        private string InitializeImageV2(Display data, string tempPath)
        {
            /*
             * 0. Initialize display
             */

            #region Initialize Template
            // Get template resolutions
            var templateResolution = _unitOfWork.TemplateRepository
                .EnableQuery()
                .Include(c => c.Layers!)
                .ThenInclude(c => c.Boxes!)
                .ThenInclude(c => c.BoxItems!)
                .ThenInclude(c => c.Font)
                .Where(c => c.TemplateId == data.TemplateId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Template not found or deleted");

            if (templateResolution.Layers == null) throw new Exception("Template has no layers");

            // Get device resolutions
            var storeDeviceResolution = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == data.StoreDeviceId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store device not found or deleted");

            // Initialize template
            var initializeTemplate = GetInitializeTemplate(templateResolution, storeDeviceResolution);
            if (initializeTemplate.Layers == null) throw new Exception("Template has no layers");
            #endregion

            /*
             * 1. Initialize image bitmap
             */

            #region Get template from display

            Template template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == data.TemplateId).FirstOrDefault()
                ?? throw new Exception("Template not found");

            #endregion Get template from display

            #region Generate bitmap

            Bitmap b = new((int)initializeTemplate.TemplateWidth, (int)initializeTemplate.TemplateHeight);
            using Graphics g = Graphics.FromImage(b);

            #endregion Generate bitmap

            /*
             * 2. Draw image layer
             */

            #region Initialize menu, collection
            var menu = new Menu();
            if (data.MenuId != null)
            {
                menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == data.MenuId && c.IsDeleted == false).FirstOrDefault();
            }
            else { menu = null; }

            var collection = new Collection();
            if (data.CollectionId != null)
            {
                collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == data.CollectionId && c.IsDeleted == false).FirstOrDefault();
            }
            else { collection = null; }
            #endregion

            #region Draw image from template layers
            foreach (var layer in initializeTemplate!.Layers!)
            {
                DrawImageLayerV2(menu, collection, b, g, layer, tempPath);
            }
            #endregion

            /*
             * 3. Draw display box from display item
             */

            #region Get display items from display

            List<DisplayItem> displayItems = _unitOfWork.DisplayItemRepository
                .Find(c => c.DisplayId == data.DisplayId)
                .ToList();
            if (displayItems.Count == 0) throw new Exception("Display items not found or null");

            #endregion Get display items from display

            #region Get boxes from display items
            // CONTINUE HERE -> IMAGE RENDER
            List<Box> boxes = new();
            foreach (var item in displayItems)
            {
                boxes.Add(_unitOfWork.BoxRepository
                    .Find(c => c.BoxId == item.BoxId)
                    .FirstOrDefault()
                    ?? throw new Exception("Box not found or null")
                );
            }
            if (boxes.Count == 0) throw new Exception("Box not found or null");

            #endregion Get boxes from display items

            #region Initialize rectangle from boxes

            List<Rectangle> rects = new();
            foreach (var item in boxes)
            {
                Rectangle rect = new((int)item.BoxPositionX, (int)item.BoxPositionY, (int)item.BoxWidth, (int)item.BoxHeight);
                rects.Add(rect);
            }
            if (rects.Count == 0) throw new Exception("Rectangle fail to initialize");

            #endregion Initialize rectangle from boxes

            #region Draw rectangles from boxes

            foreach (var rect in rects)
            {
                g.DrawRectangle(Pens.Red, rect);
            }

            #endregion Draw rectangles from boxes

            /*
             * 4. Draw header from productgroup
             */

            #region Get product group from display item

            List<ProductGroup> productGroups = new();

            foreach (var item in displayItems)
            {
                productGroups.Add(_unitOfWork.ProductGroupRepository
                    .Find(c => c.ProductGroupId == item.ProductGroupId)
                    .FirstOrDefault()
                    ?? throw new Exception("Product group not found or null")
                );
            }
            if (productGroups.Count == 0) throw new Exception("Product group not found or null");

            #endregion Get product group from display item

            #region Initialize PointF for headers

            List<PointF> headerPoints = new();

            // Get postion x,y from boxes in step 2
            foreach (var item in boxes)
            {
                PointF point = new((int)item.BoxPositionX, (int)item.BoxPositionY);
                headerPoints.Add(point);
            }
            if (headerPoints.Count == 0) throw new Exception("Header point fail to initialize");

            #endregion Initialize PointF for headers

            #region Initialize Fonts, Colors for headers

            List<Font> headerFonts = new();
            List<Color> headerColors = new();

            // Get box items from boxes
            List<BoxItem> boxItems = new();

            foreach (var item in boxes)
            {
                boxItems.AddRange(_unitOfWork.BoxItemRepository
                    .Find(c => c.BoxId == item.BoxId)
                    .ToList()
                );
            }
            if (boxItems.Count == 0) throw new Exception("Box items not found or null");

            // Get fonts from box items
            foreach (var item in boxItems)
            {
                if (item.BoxItemType == BoxItemType.Header)
                {
                    var boxItemFromDB = _unitOfWork.BoxItemRepository
                        .EnableQuery()
                        .Include(c => c.Font)
                        .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
                        .FirstOrDefault()
                        ?? throw new Exception("Box item not found or deleted");

                    // Get temp font path
                    var tempFontPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".ttf");

                    // Download and write file
                    //using (var client = new HttpClient())
                    //{
                    //    var response =  client.GetAsync(boxItemFromDB.Font!.FontPath).Result;
                    //    if (response.IsSuccessStatusCode)
                    //    {
                    //        var content = response.Content.ReadAsByteArrayAsync().Result;
                    //        File.WriteAllBytes(tempFontPath, content);
                    //    }
                    //    client.Dispose();
                    //}

                    using (var client = new WebClient())
                    {
                        client.DownloadFile(boxItemFromDB.Font!.FontPath, tempFontPath);
                        client.Dispose();
                    }

                    // Check if file exists
                    if (!File.Exists(tempFontPath))
                    {
                        throw new FileNotFoundException();
                    } 

                    // Add font to list
                    PrivateFontCollection fontCollection = new();
                    fontCollection.AddFontFile(tempFontPath);
                    Font font = new Font(fontCollection.Families[0], (int)boxItemFromDB.FontSize);
                    headerFonts.Add(new Font(fontCollection.Families[0], (int)boxItemFromDB.FontSize));
                    File.Delete(tempFontPath);

                    // Add color to list
                    ColorConverter colorConverter = new();
                    Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
                    headerColors.Add(color);
                }
            }
            if (headerFonts.Count == 0) throw new Exception("Font not found or null");
            if (headerColors.Count == 0) throw new Exception("Color not found or null");

            #endregion Initialize Fonts, Colors for headers

            #region Draw header from product group

            foreach (var productGroup in productGroups)
            {
                g.DrawString(productGroup.ProductGroupName,
                    headerFonts[productGroups.IndexOf(productGroup)],
                    new SolidBrush(headerColors[productGroups.IndexOf(productGroup)]),
                    headerPoints[productGroups.IndexOf(productGroup)]
                    );
            }

            #endregion Draw header from product group

            /*
             * 5. Draw product name from productgroup
             */

            #region Get product from menu / collection in display

            // Initialize padding constants
            const int heightPadding = 100;

            // Initialize product group item
            List<ProductGroupItem> productGroupItems = new();

            // Get product group items from product groups
            foreach (var productGroup in productGroups)
            {
                productGroupItems.AddRange(_unitOfWork.ProductGroupItemRepository
                    .Find(c => c.ProductGroupId == productGroup.ProductGroupId)
                    .ToList()
                );
            }
            if (productGroupItems.Count == 0) throw new Exception("Product group item not found or null");

            // Get product from product group items
            List<Product> products = new();
            foreach (var productGroupItem in productGroupItems)
            {
                products.Add(_unitOfWork.ProductRepository
                    .Find(c => c.ProductId == productGroupItem.ProductId)
                    .FirstOrDefault()
                    ?? throw new Exception("Product not found or null")
                );
            }
            if (products.Count == 0) throw new Exception("Product not found or null");

            #endregion Get product from menu / collection in display

            #region Initialize PointF for products

            List<PointF> productPoints = new();

            // Get postion x,y from boxes in step 2
            foreach (var item in boxes)
            {
                PointF point = new((int)item.BoxPositionX, (int)item.BoxPositionY + heightPadding);
                productPoints.Add(point);
            }
            if (productPoints.Count == 0) throw new Exception("Product point fail to initialize");

            #endregion Initialize PointF for products

            #region Initialize Fonts, Colors for products

            List<Font> bodyFonts = new();
            List<Color> bodyColors = new();

            // Get box items from boxes from step 3
            List<BoxItem> bodyBoxItems = new();
            bodyBoxItems = boxItems;

            // Get fonts from box items
            foreach (var item in bodyBoxItems)
            {
                if (item.BoxItemType == BoxItemType.Body)
                {
                    var boxItemFromDB = _unitOfWork.BoxItemRepository
                        .EnableQuery()
                        .Include(c => c.Font)
                        .Where(c => c.BoxId == item.BoxId && c.BoxItemType == item.BoxItemType)
                        .FirstOrDefault()
                        ?? throw new Exception("Box item not found or deleted");

                    // Get temp font path
                    var tempFontPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".ttf");

                    // Download and write file
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(boxItemFromDB.Font!.FontPath, tempFontPath);
                        client.Dispose();
                    }

                    // Check if file exists
                    if (!File.Exists(tempFontPath))
                    {
                        throw new FileNotFoundException();
                    }
                    

                    // Add font to list
                    PrivateFontCollection fontCollection = new();
                    fontCollection.AddFontFile(tempFontPath);
                    bodyFonts.Add(new Font(fontCollection.Families[0], (int)boxItemFromDB.FontSize));
                    File.Delete(tempFontPath);

                    // Add color to list
                    ColorConverter colorConverter = new();
                    Color color = (Color)colorConverter.ConvertFromString(boxItemFromDB.BoxColor)!;
                    bodyColors.Add(color);
                }
            }
            if (bodyFonts.Count == 0) throw new Exception("Font not found or null");
            if (bodyColors.Count == 0) throw new Exception("Color not found or null");

            #endregion Initialize Fonts, Colors for products

            #region Get biggest string size from products

            string biggestString = "";
            float biggestStringWidth = 0f;
            SizeF biggestStringSize = new();

            foreach (var productGroup in productGroups)
            {
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
            }

            #endregion Get biggest string size from products

            #region Draw products within the display area

            foreach (var productGroup in productGroups)
            {
                // Get the starting point for the product
                var productPoint = productPoints[productGroups.IndexOf(productGroup)];

                // Get the rectangle for the product group
                var rect = rects[productGroups.IndexOf(productGroup)];

                foreach (var productGroupItem in productGroup.ProductGroupItems!)
                {
                    // Check if the product can fit in the remaining display area
                    if (productPoint.Y < rect.Bottom - biggestStringSize.Height)
                    {
                        if (productPoint.X + biggestStringSize.Width < rect.Right)
                        {
                            // Draw the product name on the display
                            g.DrawString(productGroupItem.Product!.ProductName,
                                bodyFonts[productGroups.IndexOf(productGroup)],
                                new SolidBrush(bodyColors[productGroups.IndexOf(productGroup)]),
                                productPoint);
                        }

                        // Update the Y position for the next product
                        productPoint.Y += biggestStringSize.Height; // Borrow biggestStringSize from "Get biggest string size from products" region
                    }
                }
            }

            #endregion Draw products within the display area



            /*
             * 6. Draw product prices from product size prices
             */

            #region Draw products price within the display area

            // Intialize padding constants
            const int widthPaddingS = 10;
            const int priceHeightPadding = 100;
            const int sizeHeightPadding = 60;

            // get display items
            List<DisplayItem> displayItemsFromDB = _unitOfWork.DisplayItemRepository.EnableQuery()
                .Where(c => c.DisplayId == data.DisplayId)
                .Include(c => c.ProductGroup!)
                .ThenInclude(c => c.ProductGroupItems!)
                .ThenInclude(c => c.Product!)
                .ThenInclude(c => c.ProductSizePrices)

                .Include(c => c.Box!)
                .ThenInclude(c => c.BoxItems!)
                .ThenInclude(c => c.Font)
                .ToList();

            foreach (var displayItem in displayItemsFromDB)
            {
                Box box = _unitOfWork.BoxRepository.Find(c => c.BoxId == displayItem.BoxId).FirstOrDefault()
                    ?? throw new Exception("Box not found or deleted");

                // Get the rectangle for the displayItem
                var rect = rects[displayItems.IndexOf(displayItem)];
                //



                // Find the biggest product string width
                string BIGGEST_PRODUCT_STRING = "";
                float BIGGEST_PRODUCT_STRING_WIDTH = 0f;

                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                    var productFont = bodyFonts
                        .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
                        .FirstOrDefault();

                    var tempWidth = g.MeasureString(productGroupItem.Product!.ProductName,
                        productFont).Width;

                    if (tempWidth >= g.MeasureString(BIGGEST_PRODUCT_STRING, productFont).Width)
                    {
                        BIGGEST_PRODUCT_STRING = productGroupItem.Product!.ProductName;
                        BIGGEST_PRODUCT_STRING_WIDTH = tempWidth;
                    }
                }

                if (BIGGEST_PRODUCT_STRING == "") throw new Exception("BIGGEST_PRODUCT_STRING fail to initialize");
                //

                // Find the biggest price string height, width
                string BIGGEST_PRICE_STRING = "";
                float BIGGEST_PRICE_STRING_HEIGHT = 0f;
                float BIGGEST_PRICE_STRING_WIDTH = 0f;

                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                    var productPriceFont = bodyFonts
                        .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
                        .FirstOrDefault();

                    foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                    {
                        var tempHeight = g.MeasureString(productSizePrice.Price.ToString(),
                            productPriceFont);

                        if (tempHeight.Height >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceFont).Height)
                        {
                            BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                            BIGGEST_PRICE_STRING_HEIGHT = tempHeight.Height;
                        }

                        if (tempHeight.Width >= g.MeasureString(BIGGEST_PRICE_STRING, productPriceFont).Width)
                        {
                            BIGGEST_PRICE_STRING = productSizePrice.Price.ToString();
                            BIGGEST_PRICE_STRING_WIDTH = tempHeight.Width;
                        }
                    }
                }

                if (BIGGEST_PRICE_STRING == "") throw new Exception("BIGGEST_PRICE_STRING fail to initialize");
                //

                // Initialize Pointf for product size, prices based on biggest product string width; product size
                PointF pointPriceSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + priceHeightPadding);
                PointF pointPriceSizeM = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + priceHeightPadding);
                PointF pointPriceSizeL = new(pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + priceHeightPadding);

                PointF pointSizeS = new(box.BoxPositionX + BIGGEST_PRODUCT_STRING_WIDTH + widthPaddingS, box.BoxPositionY + sizeHeightPadding);
                PointF pointSizeM = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH, box.BoxPositionY + sizeHeightPadding);
                PointF pointSizeL = new(pointSizeS.X + BIGGEST_PRICE_STRING_WIDTH * 2, box.BoxPositionY + sizeHeightPadding);
                // 

                // Draw size of product 

                // Get the BoxItem for the product price
                BoxItem boxItemForSize = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                // Convert the box color to a Color object
                boxItemForSize.BoxColor = boxItemForSize.BoxColor.Split("#").Last();
                Color sizeColor = bodyColors.Where(c => c.Name.Contains(boxItemForSize.BoxColor)).FirstOrDefault()!;

                // Get the font for the product price
                var productSizeFont = bodyFonts
                            .Where(c => c.Name == boxItemForSize.Font!.FontName || c.Size == boxItemForSize.FontSize)
                            .FirstOrDefault();

                // Intialize flag for product size
                bool isProductSizeSRendered = false;
                bool isProductSizeMRendered = false;
                bool isProductSizeLRendered = false;

                // Draw product prices & size
                foreach (var productGroupItem in displayItem.ProductGroup!.ProductGroupItems!)
                {
                    foreach (var productSizePrice in productGroupItem.Product!.ProductSizePrices!)
                    {
                        // Get the BoxItem for the product price
                        BoxItem boxItem = displayItem.Box!.BoxItems!.Where(c => c.BoxItemType == BoxItemType.Body && c.BoxId == box.BoxId).FirstOrDefault()!;

                        // Convert the box color to a Color object
                        boxItem.BoxColor = boxItem.BoxColor.Split("#").Last();
                        Color color = bodyColors.Where(c => c.Name.Contains(boxItem.BoxColor)).FirstOrDefault()!;

                        // Get the font for the product price
                        var productPriceFont = bodyFonts
                                    .Where(c => c.Name == boxItem.Font!.FontName || c.Size == boxItem.FontSize)
                                    .FirstOrDefault();

                        // Check if there is enough space to draw the product price
                        if (pointPriceSizeS.Y < rect.Bottom - BIGGEST_PRICE_STRING_HEIGHT)
                        {

                            // Draw price for product size Normal
                            if (productSizePrice.ProductSizeType == ProductSizeType.Normal && pointPriceSizeS.X + BIGGEST_PRICE_STRING_WIDTH < rect.Right)
                            {

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeS);

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

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeS);

                                pointPriceSizeS.Y += BIGGEST_PRICE_STRING_HEIGHT;
                            }

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

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeM);

                                pointPriceSizeM.Y += BIGGEST_PRICE_STRING_HEIGHT;
                            }

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

                                // Draw the product price on the display
                                g.DrawString(productSizePrice.Price.ToString(),
                                    productPriceFont,
                                    new SolidBrush(color),
                                    pointPriceSizeL);

                                pointPriceSizeL.Y += BIGGEST_PRICE_STRING_HEIGHT;
                            }
                        }
                        //
                    }
                }
            }

            #endregion Draw products price within the display area



            /*
            * FINALLY: Save image
            */
            b.Save($"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png");
            b.Dispose();

            return $"{Directory.GetCurrentDirectory()}" + @"\wwwroot\images\test2.png";
        }

        private static void DrawImageLayerV2(Menu? menu, Collection? collection, Bitmap b, Graphics g, Layer layer, string tempPath)
        {
            // Draw background
            if (layer.LayerType == LayerType.BackgroundImageLayer)
            {
                var image = System.Drawing.Image.FromFile(layer.LayerItem!.LayerItemValue);
                g.DrawImage(image, 0, 0, b.Width, b.Height);
            }

            // Draw image
            //if (layer.LayerType == LayerType.ImageLayer)
            //{
            //    foreach (var box in layer.Boxes!)
            //    {
            //        var rectf = new RectangleF(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
            //        var rect = new Rectangle((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);

            //        // Lưu ý lỗi file hình : Out of memory tên phía sau bắt đầu "....-10212.jpg"
            //        var image = Image.FromFile(layer.LayerItem!.LayerItemValue)
            //            ?? throw new Exception($"Image not found: {layer.LayerItem.LayerItemValue}");

            //        g.DrawRectangle(Pens.Black, rect);
            //        g.DrawImage(image, rectf);
            //    }
            //}

            // Draw title
            if (layer.LayerType == LayerType.MenuCollectionNameLayer)
            {
                var box = layer.Boxes!.FirstOrDefault() ?? throw new Exception("Box not found in draw title layer");

                RectangleF boxRectTitleF = new(box.BoxPositionX, box.BoxPositionY, box.BoxWidth, box.BoxHeight);
                Rectangle boxRectTitle = new((int)box.BoxPositionX, (int)box.BoxPositionY, (int)box.BoxWidth, (int)box.BoxHeight);
                // Get name
                string name = "";
                if (menu != null) name = menu.MenuName;
                if (collection != null) name = collection.CollectionName;

                // Get BoxItem
                BoxItem boxItem = box.BoxItems!.FirstOrDefault()
                    ?? throw new Exception("Box Item not found in draw title layer");

                // Get Font
                Domain.Models.Font font = boxItem.Font
                    ?? throw new Exception("Font not found in draw title layer");

                // Get temp font path
                var tempFontPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".ttf");

                // Download and write file
                using (var client = new WebClient())
                {
                    client.DownloadFile(font!.FontPath, tempFontPath);
                    client.Dispose();
                }

                // Check if file exists
                if (!File.Exists(tempFontPath))
                {
                    throw new FileNotFoundException();
                }
                

                // Config font
                PrivateFontCollection fontCollection = new();
                //fontCollection.AddFontFile(@"C:\Users\tekat\OneDrive\Desktop\Lilita_One\LilitaOne.ttf");
                var fontPath = $@"{font.FontPath}";
                fontCollection.AddFontFile(tempFontPath);
                FontFamily fontFamily = new(fontCollection.Families.FirstOrDefault()!.Name, fontCollection);
                File.Delete(tempFontPath);

                // Config color
                ColorConverter converter = new();
                Color color = (Color)converter.ConvertFromString(boxItem.BoxColor)!;

                // Draw menu name
                //g.DrawString(name
                //    , new System.Drawing.Font(fontFamily, (float)box.BoxItems!.First().FontSize, FontStyle.Bold)
                //    , new SolidBrush(Color.Black)
                //    , boxRectTitleF
                //    , new StringFormat() { Alignment = boxItem.TextFormat }
                //    );

                //g.DrawRectangle(Pens.Black, boxRectTitle);

                // Delete temp font file
                System.IO.File.Delete(tempFontPath);
            }
        }

        public string GetImageByIdV2(int displayId, string tempPath)
        {
            Display display = _unitOfWork.DisplayRepository.EnableQuery()
                .Where(c => c.DisplayId == displayId && c.IsDeleted == false)
                .Include(c => c.Menu!)
                .Include(c => c.Collection!)
                .Include(c => c.Template!)
                .ThenInclude(c => c.Layers!)
                .ThenInclude(c => c.LayerItem)
                .Include(c => c.DisplayItems)
                .FirstOrDefault()
                ?? throw new Exception("Display not found or deleted");

            string imgPath = InitializeImageV2(display, tempPath);

            return imgPath;
        }
    }
}