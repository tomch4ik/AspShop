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
        private RestResponse restResponse = new()
        {
            Meta = new()
            {
                Service = "Shop API 'User cart'",
                ServerTime = DateTime.Now.Ticks,
            }
        };

        private String imgPath => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Storage/Item/";

        [HttpGet]
        public RestResponse GetActiveCart()
        {
            RestResponse restResponse = new()
            {
                Meta = new()
                {
                    Service = "Shop API 'User cart'. Get active cart",
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

                    var activeCart = _dataAccessor
                    .GetActiveCart(userId);

                    restResponse.Data = _dataAccessor == null ? null :
                        activeCart with
                        {
                            CartItems = activeCart
                                .CartItems
                                .Select(ci => ci with
                                {
                                    Product = ci.Product with
                                    {
                                        ImageUrl = imgPath + (ci.Product.ImageUrl ?? "no-image.jpg")
                                    }
                                }).ToList()
                        };
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

        [HttpPatch("{id}")]
        public RestResponse UpdateCartItem(String id, [FromQuery] int cnt) 
        {
            this.restResponse.Meta.Service += "Update Cart Item for " + cnt;
            ExecuteAuthorized(
                (userId) => this.restResponse.Data =
                    _dataAccessor.UpdateCartItem(userId, id, cnt)
            );
            return this.restResponse;
        }

        [HttpDelete("{id}")]
        public RestResponse DeleteCartItem(String id) // cart item id
        {
            this.restResponse.Meta.Service += "Delete Cart Item";
            ExecuteAuthorized(
                (userId) => this.restResponse.Data = _dataAccessor.DeleteCartItem(userId, id)
            );
            return this.restResponse;
        }

        [HttpDelete]
        public RestResponse DeleteCart()
        {
            return new();
        }

        private void ExecuteAuthorized(Action<String> action)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                try
                {
                    String userId = HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.PrimarySid).Value;

                    action(userId);
                }
                catch (Exception ex) when (ex is ArgumentNullException)
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
        }
    }
}
