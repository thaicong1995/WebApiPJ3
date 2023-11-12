
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X9;
using System.Security.Claims;
using WebApi.Dto;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.MyDbContext;
using WebApi.TokenConfig;

namespace WebApi.VNpay
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly VnPayService _vnPayService;
        private readonly MyDb _myDb;
        private readonly Token _token;

        public VnPayController(VnPayService vnPayService, MyDb myDb, Token token)
        {
            _vnPayService = vnPayService;
            _myDb = myDb;
            _token = token;
        }

        [HttpPost("create-payment/{orderNo}")]
        public IActionResult CreatePayment(string orderNo, [FromBody] OrderDto orderDto)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        return Unauthorized("The token is no longer valid");
                    }

                    var paymentUrl = _vnPayService.CreatePaymentUrl(userID, orderNo, HttpContext, orderDto);
                    Console.WriteLine($"Order Number--------------------------------------------------------: {orderNo}");
                    return Ok(new { paymentUrl });
                }
                else
                {
                    return BadRequest(new { message = "Invalid UserId." });
                }
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(new { error = "cant create payment..", message = ex.Message });
            }
        }

        [HttpGet("payment-callback")]
        public IActionResult PaymentCallback([FromQuery] string vnp_OrderInfo, [FromQuery] string vnp_ResponseCode)
        {
            try
            {
                if (vnp_ResponseCode == "00")
                {
                    var orderInfo = vnp_OrderInfo;
                    var dataParts = orderInfo.Split(' ').Select(part => part.Trim(' ')).ToArray();

                    if (dataParts.Length >= 3)
                    {
                        var orderNo = dataParts[0];
                        if (int.TryParse(dataParts[1], out int userId) && int.TryParse(dataParts[2], out int discount))
                        {
                            Console.WriteLine($"Order No: {orderNo}, User ID: {userId}, Discount: {discount}");

                            _vnPayService.UpdateOrderAndCartStatus(orderNo, userId, discount);
                            return Ok(new { message = "Sucess!." });
                        }
                        return BadRequest(new { error = "Invalid Data." });
                    }
                    return BadRequest(new { error = "Dữ liệu not enough" });
                }

                else
                {
                    Console.WriteLine("Payment failed");
                    return BadRequest(new { error = "Payment failed." });
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, có thể ghi log
                return BadRequest(new { error = "An error occurred", message = ex.Message });
            }
        }


    }
}
