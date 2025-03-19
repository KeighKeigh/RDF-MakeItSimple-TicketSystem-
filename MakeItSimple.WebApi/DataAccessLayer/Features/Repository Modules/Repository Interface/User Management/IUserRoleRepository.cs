using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using System.Collections.Generic;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management
{
    public interface IUserRoleRepository
    {
        Task<List<string>> UserRoleByPermission(string permission);
        Task<IReadOnlyList<UserRole>> UserRoleList();

        Task<List<string>> UserRoleCheckPermission(string id);

    }
}
