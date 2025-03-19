using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management;
using MakeItSimple.WebApi.Models;

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
    }
}
