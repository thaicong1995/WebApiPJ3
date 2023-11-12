﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        private readonly Token _token;

        public ProductController(ProductService productService, Token token)
        {
            _productService = productService;
            _token = token;
        }

       

        [HttpGet("GetProduct-Id")]
        public IActionResult GetShopById([FromQuery] int Id)
        {
            try
            {
                var Result = _productService.GetProductByID(Id);
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


        [Authorize]
        [HttpPost("add-new")]
        public IActionResult AddNewProduct([FromForm] IFormFile image, [FromForm] ProductDto productDto, [FromQuery] int ShopId)
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


                    string result = _productService.AddNewProduct(userID, ShopId, productDto, image);

                    if (result.Contains("successfully"))
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
                else
                {
                    return BadRequest("Invalid user ID.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPut("update-product")]
        public IActionResult UpdateProduct([FromForm] IFormFile image, [FromForm] ProductDto productDto, [FromQuery] int shopId, [FromQuery] int productId)
        {
            // Lấy userID của người dùng từ Claims trong token
            var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaims != null && int.TryParse(userClaims.Value, out int userId))
            {
                var tokenStatus = _token.CheckTokenStatus(userId);

                if (tokenStatus == StatusToken.Expired)
                {
                    // Token không còn hợp lệ, từ chối yêu cầu
                    return Unauthorized("Token has expired. Please log in again.");
                }

                string result = _productService.UpdateProduct(userId, shopId, productId, productDto, image);

                if (result.Contains("successfully"))
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Invalid user ID.");
            }
        }

        [Authorize]
        [HttpGet("GetAll-IdShop")]
        public IActionResult GetAllByIdShop([FromQuery]int ShopID)
        {
            try
            {
                var userClaims = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims != null && int.TryParse(userClaims.Value, out int userId))
                {
                    var tokenStatus = _token.CheckTokenStatus(userId);

                    if (tokenStatus == StatusToken.Expired)
                    {
                        // Token không còn hợp lệ, từ chối yêu cầu
                        return Unauthorized("Token has expired. Please log in again.");
                    }
                    var products = _productService.GetAllByShopID(ShopID);

                    if (products != null && products.Any())
                    {                 
                        return Ok(products);
                    }
                    else
                    {
                        return NotFound("No products found for this shop.");
                    }

                }
                else
                {
                    return BadRequest("Invalid user ID.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllProductShop")]
        public IActionResult GetAllByIdShopforUser([FromQuery] int ShopID)
        {
            try
            {
                
                    var products = _productService.GetAllByShopID(ShopID);

                    if (products != null && products.Any())
                    {
                        return Ok(products);
                    }
                    else
                    {
                        return NotFound("No products found for this shop.");
                    }

                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            try
            {
                var products = _productService.GetAll();
                return Ok(products);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


        [HttpGet("search")]
        public object SearchProducts([FromQuery]string keyword)
        {
            try
            { 
                var products = _productService.SearchProducts(keyword);

                if (products == null)
                {
                    return NotFound("No matching products found.");
                }

                return Ok(products);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }

    }
}
    

