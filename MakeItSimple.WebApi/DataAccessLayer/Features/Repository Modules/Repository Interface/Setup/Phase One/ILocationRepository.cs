namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One
{
    public interface ILocationRepository 
    {
        Task<bool> LocationCodeExist(string locationCode);
    }
}
