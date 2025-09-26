using AspShop.Data;
using AspShop.Data.Entities;
using AspShop.Models.Home;
using AspShop.Models.Rest;
using AspShop.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspShop.Controllers.Api
{
    [Route("api/group")]
    [ApiController]
    public class GroupController(IStorageService storageService, DataAccessor dataAccessor, DataContext dataContext) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataContext _dataContext = dataContext;
        private readonly DataAccessor _dataAccessor = dataAccessor;

        //[HttpGet]
        //public IEnumerable<ProductGroup> AllGroups()
        //{
        //    return _dataAccessor.GetProductGroups();

        //}

        [HttpGet]
        public RestResponse AllGroups()
        {
            return new()
            {
                Meta = new()
                {
                    Manipulations = [ "GET", "POST" ],
                    Cache = 24 * 60 * 60,
                    Service = "Shop API: product groups",
                    DataType = "json/array"
                },
                Data = _dataAccessor.GetProductGroups()
            };
        }

        [HttpGet("{id}")]
        public RestResponse GroupBySlug(String id)
        {
            var pg = _dataAccessor.GetProductGroupBySlug(id);
            return new()
            {
                Status = pg == null ? RestStatus.Status404 : RestStatus.Status200,
                Meta = new()
                {
                    Manipulations = ["GET"],
                    Cache = 60 * 60,
                    Service = "Shop API: products of group by slug",
                    DataType = pg == null ? "null" : "json/object"
                },
                Data = pg
            };
        }

        [HttpPost]
        public object AddGroup(AdminGroupFormModel model)
        {
            //Валідація дз
            #region Dz Validation

            if (string.IsNullOrWhiteSpace(model.Name))
                return new { status = "Invalid Name", code = 400 };

            if (string.IsNullOrWhiteSpace(model.Slug))
                return new { status = "Invalid Slug", code = 400 };

            if (_dataContext.ProductGroups.Any(g => g.Slug == model.Slug))
                return new { status = "Slug already exists", code = 400 };

            string? imageUrl = null;
            if (model.Image != null)
            {
                try
                {
                    imageUrl = _storageService.Save(model.Image);
                }
                catch
                {
                    return new { status = "Invalid Image", code = 400 };
                }
            }

            #endregion
            _dataContext.ProductGroups.Add(new Data.Entities.ProductGroup()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                ImageUrl = _storageService.Save(model.Image)
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
