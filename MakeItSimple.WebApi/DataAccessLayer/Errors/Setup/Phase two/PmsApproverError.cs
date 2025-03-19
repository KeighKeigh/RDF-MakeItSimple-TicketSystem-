using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two
{
    public class PmsApproverError
    {
        public static Error PmsApproverIdNotExist() =>
        new Error("PmsApprover.PmsApproverIdNotExist", "Pms approver id not exist!");

        public static Error UserIdNotExist() =>
        new Error("PmsApprover.UserIdNotExist", "User id not exist!");

        public static Error UserIdDuplicate() =>
        new Error("PmsApprover.UserIdDuplicate", "User id duplicated!");

        public static Error ApproverLevelDuplicate() =>
        new Error("PmsApprover.ApproverLevelDuplicate", "Approver level duplicated!");

        public static Error NoApproverHasBeenSetup() =>
        new Error("PmsApprover.NoApproverHasBeenSetup", "No approver has been setup!");



    }
}
