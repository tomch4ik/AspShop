using AspShop.Data.Entities;
using AspShop.Services.Kdf;
using Microsoft.EntityFrameworkCore;

namespace AspShop.Data
{
    public class DataAccessor(DataContext dataContext, IKdfService kdfService)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService = kdfService;

        public void AddToCart(String userId, String productId)
        {
            Guid userGuid = Guid.Parse(userId);
            Guid productGuid = Guid.Parse(productId);
            // Перевіряємо чи достатньо товару у наявності
            // Перевіряємо чи користувач має відкритий кошик
            // Якщо не має, то відкриваємо новий
            // Перевіряємо чи є у кошику даний товар
            // якщо ні, то створюємо нову позицію
            // якщо є, то збільшуємо кількість у наявній позиції
            Cart? cart = GetActiveCart(userId);
            if (cart == null)
            {
                cart = new Cart()
                {
                    Id = Guid.NewGuid(),
                    UserId = userGuid,
                    CreatedAt = DateTime.Now,
                };
                _dataContext.Carts.Add(cart);
            }
            CartItem? cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productGuid);
            if (cartItem == null)
            {
                var product = _dataContext.Products.Find(productGuid)!;
                cartItem = new CartItem()
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = productGuid,
                    Quantity = 1,
                    Product = product,
                };
                _dataContext.CartItems.Add(cartItem);
                //cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += 1;
            }
            // Перераховуємо вартість кошика
            CalcPrice(cart);
            _dataContext.SaveChanges();
        }
        private void CalcPrice(Cart cart)
        {
            double total = 0.0;
            foreach (CartItem cartItem in cart.CartItems)
            {
                // if (cartItem.DiscountID != null) ...
                cartItem.Price = cartItem.Product.Price * cartItem.Quantity;
                total += cartItem.Price;
            }
            // if (cart.DiscountID != null) ...
            cart.Price = total;
        }

        public Cart? GetActiveCart(String userId)
        {
            Guid userGuid = Guid.Parse(userId);
            return _dataContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c =>
                    c.UserId == userGuid &&
                    c.PaidAt == null &&
                    c.DeletedAt == null);
        }

        public void AddFeedback(Product product, Guid? userId, int? rate, string? comment)
        {
            _dataContext.Feedbacks.Add(new Feedback()
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                UserId = userId,
                Rate = rate,
                Comment = comment,
                CreatedAt = DateTime.Now
            });
            _dataContext.SaveChanges();
        }

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
            .ThenInclude(p => p.Feedbacks)
            .AsNoTracking()
            .FirstOrDefault(g => g.Slug == slug && g.DeletedAt == null);
            return group == null ? null : group with
            {
                Products = group.Products
                .Select(p => p with
                {
                ImageUrl =
                $"/Storage/Item/{p.ImageUrl ?? "no-image.jpg"}",
                    FeedbacksCount = p.Feedbacks.Count()
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

