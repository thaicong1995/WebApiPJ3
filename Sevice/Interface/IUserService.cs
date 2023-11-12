using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Sevice.Interface
{
    public interface IUserService
    {
        List<User> GetUsers();
        User RegisterUser(User user);
        User LoginUser(UserDto userDto);
    }
}
