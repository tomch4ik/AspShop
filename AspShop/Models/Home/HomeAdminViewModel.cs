using AspShop.Data.Entities;

namespace AspShop.Models.Home
{
    public class HomeAdminViewModel
    {
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
