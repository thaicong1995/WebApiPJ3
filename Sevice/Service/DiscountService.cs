using WebApi.Dto;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.MyDbContext;

namespace WebApi.Sevice.Service
{
    public class DiscountService
    {
        private readonly MyDb _myDb;
        public DiscountService(MyDb myDb)
        {
            _myDb = myDb;
        }
        public Discounts CreateDiscount(DiscountDTo discount)
        {
            try
            {
                if (discount == null)
                {
                    throw new ArgumentNullException(nameof(discount), "Discount information is missing.");
                }

                // Lấy ngày hiện tại
                DateTime currentDate = DateTime.Now;

                // Tạo đối tượng Discount với ngày bắt đầu là ngày hiện tại và ngày kết thúc là ngày hiện tại cộng thêm 1 ngày (1 ngày hiệu lực)
                var newDiscount = new Discounts
                {
                    Code = discount.Code,
                    Value = discount.Value,
                    StartDate = currentDate,
                    EndDate = currentDate.AddDays(1), // Hiệu lực trong 1 ngày
                    _discountStatus = DiscountStatus.Active // Đảm bảo bạn đã gán DiscountStatus từ đối tượng DTO
                };

                // Thêm đối tượng mã giảm giá vào cơ sở dữ liệu
                _myDb.Discounts.Add(newDiscount);
                _myDb.SaveChanges();

                return newDiscount; // Trả về đối tượng Discount sau khi đã thêm vào cơ sở dữ liệu
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }
        }
    }
}
