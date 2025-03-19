using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two
{
    public class PmsError
    {
        public static Error PmsTechnicianNotExist() =>
        new Error("Pms.PmsTechnicianNotExist", "Pms technician not exist!");
        public static Error PmsNotExist() =>
        new Error("Pms.PmsNotExist", "Pms not exist!");

        public static Error PmsAlreadyInApproval() =>
        new Error("Pms.PmsAlreadyInApproval", "Pms already in approval!");

        public static Error PmsNotAuthorized() =>
        new Error("Pms.PmsNotAuthorized", "Pms not authorized!");

        public static Error PmsAttachmentNotExist() =>
        new Error("Pms.PmsAttachmentNotExist", "Pms attachment not exist!");
    }
}
