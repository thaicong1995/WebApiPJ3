﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Dto;
using WebApi.Models.Enum;
using WebApi.MyDbContext;
using WebApi.Sevice.Service;
using WebApi.TokenConfig;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly Token _token;
        private readonly MyDb _myDb;
        private readonly ShopService _shopService;
        private readonly IConfiguration _configuration;
        public ShopController(Token token, MyDb myDb, ShopService shopService, IConfiguration configuration)
        {
            _token = token;
            _myDb = myDb;
            _shopService = shopService;
            _configuration = configuration;
        }


        [Authorize]
        [HttpPost("activate-shop")]
        public IActionResult ActivateShop([FromBody] ShopDto shopDto)
        {
            try
            {             
                // Lấy userID của người dùng từ Claims trong token
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }

                    bool result = _shopService.ActivateShop(userID, shopDto);

                    if (result)
                    {
                        return Ok("Successfully Active..");
                    }
                    else
                    {
                        return BadRequest("Cannot be activated or already activated.??");
                    }
                }
                else
                {
                    // Sai thông tin userID hoặc không có userID trong token
                    return BadRequest(new { message = "Invalid UserId." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPut("update-shop")]
        public IActionResult UpdateShop([FromBody] ShopDto shopDto)
        {
            try
            {
                // Lấy userID của người dùng từ Claims trong token
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userID))
                {
                    var tokenStatus = _token.CheckTokenStatus(userID);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("The token is no longer valid. Please log in again.");
                    }
                    string result = _shopService.UpdateShop(userID, shopDto);
                    if (result == "Updated success !")
                    {
                        return Ok("Updated success !");
                    }
                    else
                    {
                        return BadRequest("You need active Shop");
                    }
                }
                else
                {
                    // Sai thông tin userID hoặc không có userID trong token
                    return BadRequest(new { message = "Invalid UserId." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

       
        [HttpGet("Get-Id")]
        public IActionResult GetShopById([FromQuery]  int Id)
        {
            try
            {
               var Result = _shopService.GetShopByID(Id);
                if (Result != null)
                {
                    return Ok(Result);
                }
                else
                {
                    return NotFound("Not Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

    }  

}

