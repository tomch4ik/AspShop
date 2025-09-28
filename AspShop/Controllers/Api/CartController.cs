using AspShop.Data;
using AspShop.Models.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspShop.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        [HttpPost("{id}")]
        public RestResponse AddProduct(String id) //Product id
        {
            RestResponse restResponse = new()
            {
                Meta = new()
                {
                    Service = "Shop API 'User cart'. Add product to cart",
                    ServerTime = DateTime.Now.Ticks,
                }
            };
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                try
                {
                    String userId = HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid)
                    .Value;
                    _dataAccessor.AddToCart(userId, id);
                }
                catch (Exception ex) when (ex is ArgumentException)
                {
                    restResponse.Status = RestStatus.Status400;
                    restResponse.Data = ex.Message;
                }
                catch (Exception ex) when (ex is InvalidOperationException)
                {
                    restResponse.Status = RestStatus.Status401;
                    restResponse.Data = "Error user identification. Check JWT";
                }
                catch (Exception ex) when (ex is FormatException)
                {
                    restResponse.Status = RestStatus.Status400;
                    restResponse.Data = ex.Message;
                }
            }
            else
            {
                restResponse.Status = RestStatus.Status401;
            }
            return restResponse;
        }
    }
}
