﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X9;
using System.Security.Claims;
using WebApi.Dto;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Sevice.Interface;
using WebApi.Sevice.Service;
using WebApi.TokenConfig;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        private readonly Token _token;

        public OrderController(OrderService orderService, Token token, UserService userService)
        {
            _orderService = orderService;
            _token = token;
            _userService = userService;
        }

        [HttpPost("create")]
        public IActionResult CreateOrder([FromBody] List<int> selectedProductIds)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userId))
                {
                    var tokenStatus = _token.CheckTokenStatus(userId);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }

                    if (selectedProductIds.Count == 0)
                    {
                        return BadRequest("No selected items in the cart.");
                    }

                    var createdOrders = _orderService.CreateOrders(selectedProductIds, userId); // Truyền danh sách ID sản phẩm và userId

                    return Ok(createdOrders);
                }
                else
                {
                    return BadRequest("Invalid UserId.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPost("pay")]
        public ActionResult<Order> Pay([FromQuery]string orderNo, [FromBody] OrderDto orderDto)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
     
                   List< Order> order = _orderService.PayOrder(orderNo, orderDto, userID);

                    return Ok(order);
                }
                else
                {
                    // Invalid or missing userID in the token
                    return BadRequest(new { message = "Invalid UserId." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }


        [Authorize]
        [HttpPost("refund")]
        public ActionResult<List<Order>> RefundProducts([FromQuery] string orderNo, [FromQuery] int ProductId)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userClaims != null && int.TryParse(userClaims.Value, out int userId))
                {
                    var tokenStatus = _token.CheckTokenStatus(userId);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }

                    // Gọi phương thức RefundProducts từ OrderService để xử lý hoàn trả một số lượng sản phẩm
                    var refundedOrders = _orderService.RefundProduct(orderNo, userId, ProductId);

                    return Ok(refundedOrders);
                }
                else
                {
                    // Invalid or missing userID in the token
                    return BadRequest(new { message = "Invalid UserId." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }


    }
}
