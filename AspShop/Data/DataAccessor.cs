using AspShop.Data.Entities;
using AspShop.Services.Kdf;
using Microsoft.EntityFrameworkCore;

namespace AspShop.Data
{
    public class DataAccessor(DataContext dataContext, IKdfService kdfService)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService = kdfService;

        public UserAccess? Authenticate(String login, String password)
        {
            var userAccess = GetUserAccessByLogin(login);
            if (userAccess == null)
            {
                return null;
            }
            String dk = _kdfService.Dk(password, userAccess.Salt);
            if (dk != userAccess.Dk)
            {
                return null;
            }
            return userAccess;
        }

        public UserAccess? GetUserAccessByLogin(string login)
        {
            return _dataContext
                .UserAccesses
                .AsNoTracking()
                .Include(ua => ua.User)
                .Include(ua => ua.Role)
                .FirstOrDefault(ua => ua.Login == login);
        }
        public Product? GetProductBySlug(String slug)
        {
            var product = _dataContext
                .Products
                .AsNoTracking()
                .FirstOrDefault(p => (p.Slug == slug || p.Id.ToString() == slug) && p.DeletedAt == null);

            return product == null ? null : product with
            {
                ImageUrl =
                    $"/Storage/Item/{product.ImageUrl ?? "no-image.jpg"}"
            };
        }

        public ProductGroup? GetProductGroupBySlug(string slug)
        {
            var group = _dataContext
            .ProductGroups
            .Include(g => g.Products.Where(p => p.DeletedAt == null))
            .AsNoTracking()
            .FirstOrDefault(g => g.Slug == slug && g.DeletedAt == null);
            return group == null ? null : group with
            {
                Products = group.Products
                .Select(p => p with
                {
                ImageUrl =
                $"/Storage/Item/{p.ImageUrl ?? "no-image.jpg"}"
                })
                .ToList()
            };
        }

        public IEnumerable<ProductGroup> GetProductGroups()
        {
            return _dataContext
                .ProductGroups
                .Where(g => g.DeletedAt == null)
                .AsEnumerable();
        }
    }
}

