using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_One
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly MisDbContext context;

        public SubCategoryRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<SubCategory> SubCategoryExist(int? id)
        {
            return await context.SubCategories.FindAsync(id);
        }
    }
}
