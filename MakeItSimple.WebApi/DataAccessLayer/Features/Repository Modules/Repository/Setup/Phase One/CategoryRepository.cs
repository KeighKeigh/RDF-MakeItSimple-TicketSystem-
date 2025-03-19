using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_One
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MisDbContext context;

        public CategoryRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<Category> CategoryExist(int? id)
        {
            return await context.Categories.FindAsync(id);
        }
    }
}
