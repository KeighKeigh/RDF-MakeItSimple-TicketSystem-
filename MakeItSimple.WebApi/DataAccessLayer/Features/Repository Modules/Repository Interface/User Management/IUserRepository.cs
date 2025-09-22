using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.OneCharging;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management
{
    public interface IUserRepository
    {
        Task<User> UserExist(Guid? id);
        Task<OneChargingMIS> UserDepartment(int? id);


    }
}
