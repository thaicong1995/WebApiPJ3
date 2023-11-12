using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Dto;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Sevice.Service;
using WebApi.TokenConfig;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly CartItemService _cartItemService;

        private readonly Token _token;

        public CartItemController(CartItemService cartItemService, Token token)
        {
            _cartItemService = cartItemService;
            _token = token;
        }

        [Authorize]
        [HttpPost("Add-To-Cart")]
        public IActionResult AddToCart([FromBody] CartItemRequest cartItemRequest)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
                    var cartItem = _cartItemService.AddToCart(cartItemRequest, userID);

                    return Ok(cartItem);
                }
                else
                {
                    return BadRequest("User ID is not available in the claims.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }


        [Authorize]
        [HttpPost("DecreaseQuantity")]
        public IActionResult DecreaseQuantity([FromBody] int productId)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
                    var cartItem = _cartItemService.DecreaseQuantity(productId, userID);

                    return Ok(cartItem);
                }
                else
                {
                    return BadRequest("User ID is not available in the claims.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPost("IncreaseQuantity")]
        public IActionResult IncreaseQuantity([FromBody] int productId)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
                    var cartItem = _cartItemService.IncreaseQuantity(productId, userID);

                    return Ok(cartItem);
                }
                else
                {
                    return BadRequest("User ID is not available in the claims.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet("My-Cart")]
        public IActionResult GetCartBYUser()
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
                    var cartItem = _cartItemService.GetCartByUerId( userID);

                    return Ok(cartItem);
                }
                else
                {
                    return BadRequest("User ID is not available in the claims.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }


        [Authorize]
        [HttpDelete("Delete")]
        public IActionResult deleteProductInCart([FromBody] int productId)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
                    if (_cartItemService.DeleteCart(productId, userID))
                    {
                        return Ok("Deleted");
                    }
                    else
                    {
                        return BadRequest("Not Found");
                    }
                }
                else
                {
                    return BadRequest("User ID is not available in the claims.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet("Cart")]
        public IActionResult GetListCart()
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
                    var ListCart = _cartItemService.GetListCart(userID);
                    return Ok( ListCart);
                }
                else
                {
                    return BadRequest("User ID is not available in the claims.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }
    }
}
