using AspShop.Data;
using AspShop.Data.Entities;
using AspShop.Models.Rest;
using AspShop.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpPost("feedback/{id}")]
        public RestResponse AddFeedback(String id, int? rate, String? comment)
        {
            RestResponse restResponse = new()
            {
                Meta = new()
                {
                    Manipulations = ["POST"],
                    Cache = 0,
                    Service = "Shop API: product feedback",
                    DataType = "null",
                    Opt = {
             { "id", id },
             { "rate", rate ?? -1 },
             { "comment", comment ?? "" },
            },
                },
                Data = null
            };
            Guid? userId = null;
            //Визначаємо чи є авторизований користувач
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                userId = Guid.Parse(HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid)
                    .Value);
            }
            //Перевіряємо чи існує товар
            var product = _dataAccessor.GetProductBySlug(id);
            if (product == null)
            {
                restResponse.Status = RestStatus.Status404;
                return restResponse;
            }
            _dataContext.Feedbacks.Add(new()
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                UserId = userId,
                Rate = rate,
                Comment = comment,
                CreatedAt = DateTime.Now
            });
            _dataContext.SaveChanges();
            return restResponse;
        }


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
