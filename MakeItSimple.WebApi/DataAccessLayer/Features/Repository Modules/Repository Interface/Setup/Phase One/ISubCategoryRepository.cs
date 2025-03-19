using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One
{
    public interface ISubCategoryRepository
    {
        Task<SubCategory> SubCategoryExist(int? id);
    }
}
