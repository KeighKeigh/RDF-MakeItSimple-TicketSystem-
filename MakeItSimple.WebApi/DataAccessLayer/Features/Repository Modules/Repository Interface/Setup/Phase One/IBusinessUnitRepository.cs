using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One
{
    public interface IBusinessUnitRepository 
    {
        Task<BusinessUnit> BusinessUnitExist(int? id);

    }
}
