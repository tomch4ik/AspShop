using AspShop.Data.Entities;
using System.Collections;

namespace AspShop.Models.Home
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
