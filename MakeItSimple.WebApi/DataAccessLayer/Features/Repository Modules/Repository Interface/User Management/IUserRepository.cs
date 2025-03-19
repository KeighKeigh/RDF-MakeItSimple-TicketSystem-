using MakeItSimple.WebApi.Models;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management
{
    public interface IUserRepository
    {
        Task<User> UserExist(Guid? id);
        
    }
}
