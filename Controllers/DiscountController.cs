using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Models;
using WebApi.MyDbContext;
using WebApi.Sevice.Service;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly DiscountService _discountService;
        public DiscountController (DiscountService discountService)
        {
            _discountService = discountService;
        }
        [HttpPost("CreateDiscount")]
        public IActionResult CreateDiscount([FromBody] DiscountDTo discount)
        {
            try
            {
                if (discount == null)
                {
                    return BadRequest("Discount information is missing.");
                }

                _discountService.CreateDiscount(discount);

                return Ok("Discount created successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }
    }
}
