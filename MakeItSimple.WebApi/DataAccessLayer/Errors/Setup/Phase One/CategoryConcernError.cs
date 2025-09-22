using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_One
{
    public class CategoryConcernError
    {

        public static Error CategoryConcernAlreadyExist(string CategoryConcern) =>
    new Error("CategoryConcern.CategoryConcernAlreadyExist", $"Category description {CategoryConcern} already exist!");
        public static Error CategoryConcernNotExist() =>
        new Error("CategoryConcern.CategoryConcernNotExist", $"Category not exist!");

        public static Error CategoryConcernNochanges() =>
        new Error("CategoryConcern.CategoryConcernNochanges", "No changes has made!");

        public static Error CategoryConcernIsUse(string CategoryConcern) =>
        new Error("CategoryConcern.CategoryConcernIsUse", $"Category {CategoryConcern} is use!");
    }
}
