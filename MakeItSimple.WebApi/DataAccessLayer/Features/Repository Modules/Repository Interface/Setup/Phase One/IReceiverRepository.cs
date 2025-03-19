using MakeItSimple.WebApi.Models.Setup.ReceiverSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One
{
    public interface IReceiverRepository
    {
        Task<Receiver> ReceiverExistByBusinessUnitId(int? businessUnitId);
    }
}
