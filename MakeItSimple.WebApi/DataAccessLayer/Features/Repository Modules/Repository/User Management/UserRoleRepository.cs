using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.User_Management
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly MisDbContext context;

        public UserRoleRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> UserRoleByPermission(string permission)
        {
            var userRoleList = await context.UserRoles.ToListAsync();

                 return userRoleList
                    .Where(x => x.Permissions
                    .Contains(permission))
                    .Select(x => x.UserRoleName)
                    .ToList();
        }

        public async Task<IReadOnlyList<UserRole>> UserRoleList()
        {
            return await context.UserRoles.ToListAsync();
        }

        public async Task<List<string>> UserRoleCheckPermission(string id)
        {
            //var permissionCheck = context.Users.Where(x => x.Id == id);



            //return  permissionCheck.Where(x => x.Id == id).Select(x => x.Id).ToList();

            var permissionCheck = await context.Users.ToListAsync();
            return permissionCheck
                .Where(x => x.UserRoleId == 5)
                .Select(x => x.Id.ToString()) // Convert Guid to string
                .ToList();
        }
    }
}
