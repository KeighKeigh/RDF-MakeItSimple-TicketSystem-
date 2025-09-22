using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.OneCharging;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.User_Management
{
    public class UserRepository : IUserRepository
    {
        private readonly MisDbContext context;

        public UserRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<User> UserExist(Guid? id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<OneChargingMIS> UserDepartment(int? id)
        {
            var userDepartment = await context.OneChargings.AsNoTracking().FirstOrDefaultAsync(x => x.department_id == id);

            return userDepartment;
        }
    }
}
