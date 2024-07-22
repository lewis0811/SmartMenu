using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Services
{
    public class EnumService : IEnumService
    {

        public object GetBoxItemType()
        {
            List<object> boxItemTypes = new ();

            foreach (BoxItemType item in Enum.GetValues(typeof(BoxItemType)))
            {
                boxItemTypes.Add(new { value = item, name =item.ToString() });
            }

            return boxItemTypes;
        }

        public object GetBoxType()
        {
            List<object> boxTypes = new ();

            foreach (BoxType item in Enum.GetValues(typeof(BoxType)))
            {

                boxTypes.Add(new { value = item, name = item.ToString() });
            }

            return boxTypes;
        }

        public object GetLayerType()
        {
            List<object> layerTypes = new();

            foreach (LayerType item in Enum.GetValues(typeof(LayerType)))
            {

                layerTypes.Add(new { value = item, name = item.ToString() });
            }

            return layerTypes;
        }

        public object GetProductSizeType()
        {
            List<object> productSizeTypes = new();

            foreach (ProductSizeType item in Enum.GetValues(typeof(ProductSizeType)))
            {

                productSizeTypes.Add(new { value = item, name = item.ToString() });
            }

            return productSizeTypes;
        }

        public object GetRoleType()
        {
            List<object> roleTypes = new();

            foreach (Role item in Enum.GetValues(typeof(Role)))
            {

                roleTypes.Add(new { value = item, name = item.ToString() });
            }

            return roleTypes;
        }

        public object GetTemplateType()
        {
            List<object> templateTypes = new();

            foreach (TemplateType item in Enum.GetValues(typeof(TemplateType)))
            {

                templateTypes.Add(new { value = item, name = item.ToString() });
            }

            return templateTypes;
        }
    }
}
