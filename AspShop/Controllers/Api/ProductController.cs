using AspShop.Data;
using AspShop.Data.Entities;
using AspShop.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspShop.Models.Api
{
    [Route("api/product")]
    [ApiController]
    public class ProductController(
            IStorageService storageService,
            DataAccessor dataAccessor,
            DataContext dataContext) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataContext _dataContext = dataContext;
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpPost]
        public object AddProduct(ApiProductFormModel model)
        {
            #region Валідація моделі
            if (!ModelState.IsValid)
            {
                return new
                {
                    status = "Validation failed",
                    errors = ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            e => e.Key,
                            e => e.Value!.Errors.Select(er => er.ErrorMessage).ToArray()
                        ),
                    code = 400
                };
            }
            Guid groupGuid;
            try { groupGuid = Guid.Parse(model.GroupId); }
            catch { return new { status = "Invalid GroupId", code = 400 }; }
            #endregion
            _dataContext
                .Products
                .Add(new()
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupGuid,
                    Name = model.Name,
                    Description = model.Description,
                    Slug = model.Slug,
                    ImageUrl = model.Image == null ? null : _storageService.Save(model.Image),
                    Price = model.Price,
                    Stock = model.Stock
                });

            try
            {
                _dataContext.SaveChanges();
                return new { status = "OK", code = 200 };
            }
            catch (Exception ex)
            {
                return new { status = ex.Message, code = 500 };
            }
        }
    }
}
