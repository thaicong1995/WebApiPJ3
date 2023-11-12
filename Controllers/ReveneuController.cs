using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.MyDbContext;
using WebApi.Sevice.Service;
using WebApi.TokenConfig;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReveneuController : ControllerBase
    {
        private readonly Token _token;
        private readonly RevenueService _revenueService;
        public ReveneuController (Token token, RevenueService revenueService)
        {
            _token = token;
            _revenueService = revenueService;
        }

        [Authorize]
        [HttpGet("{shopId}")]
        public IActionResult GetRevenueByUser(int shopId)
        {
            try
            {
                // Lấy userID của người dùng từ Claims trong token
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    // Kiểm tra trạng thái của token
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }

                    var revenue = _revenueService.GetRevenueByUser(shopId, userID);
                    return Ok(revenue);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
            }

            return Unauthorized("Unauthorized");
        }
    }

}

