using MakeItSimple.WebApi.Models.Setup.CategorySetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One
{
    public interface ICategoryRepository
    {
        Task<Category> CategoryExist(int? id);

    }
}
