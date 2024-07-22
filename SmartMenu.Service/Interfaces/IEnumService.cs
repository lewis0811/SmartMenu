using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IEnumService
    {
        object GetBoxItemType();
        object GetBoxType();
        object GetLayerType();
        object GetProductSizeType();
        object GetRoleType();
        object GetTemplateType();
    }
}
