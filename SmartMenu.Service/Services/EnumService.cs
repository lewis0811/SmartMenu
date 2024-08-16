using SmartMenu.Domain.Models.Enum;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class EnumService : IEnumService
    {
        public object GetBoxItemType()
        {
            List<object> boxItemTypes = new();

            foreach (BoxItemType item in Enum.GetValues(typeof(BoxItemType)))
            {
                boxItemTypes.Add(new { value = item, name = item.ToString() });
            }

            return boxItemTypes;
        }

        public object GetBoxType()
        {
            List<object> boxTypes = new();

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

        public object GetPaymentStatus()
        {
            List<object> paymentStatuses = new();

            foreach (Payment_Status item in Enum.GetValues(typeof(Payment_Status)))
            {
                paymentStatuses.Add(new { value = item, name = item.ToString() });
            }

            return paymentStatuses;
        }

        public object GetProductPriceCurrency()
        {
            List<object> productPriceCurrency = new();

            foreach (ProductPriceCurrency item in Enum.GetValues(typeof(ProductPriceCurrency)))
            {
                productPriceCurrency.Add(new { value = item, name = item.ToString() });
            }

            return productPriceCurrency;
        }

        public object GetPayType()
        {
            List<object> payType = new();

            foreach (PayType item in Enum.GetValues(typeof(PayType)))
            {
                payType.Add(new { value = item, name = item.ToString() });
            }

            return payType;
        }

        public object GetSubscriptionStatus()
        {
            List<object> subscriptionStatuses = new();

            foreach (SubscriptionStatus item in Enum.GetValues(typeof(SubscriptionStatus)))
            {
                subscriptionStatuses.Add(new { value = item, name = item.ToString() });
            }

            return subscriptionStatuses;
        }
    }
}