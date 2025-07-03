using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class TicketDateError
    {
        public static Error DateApproverTicketIdNotExist() =>
        new Error("TicketDateError.DateApproverTicketIdNotExist", "Ticket concern not exist!");

        public static Error TicketAlreadyCancel() =>
        new Error("TicketDateError.TicketAlreadyCancel", "Ticket concern already cancelled!");

        public static Error TicketIdNotExist() =>
        new Error("TicketDateError.TicketNotExist", "Ticket transaction not exist!");

        public static Error ApproverUnAuthorized() =>
        new Error("TicketDateError.ApproverInvalid", "User not authorize to approve!");

        public static Error DateTimeInvalid() =>
        new Error("TicketDateError.DateTimeInvalid", "Invalid start/target date!");

        public static Error ReTicketConcernUnable() =>
        new Error("TicketDateError.ClosingConcernUnable", "Re-Ticket request already in approval!");

        public static Error ClosingTicketConcernUnable() =>
        new Error("TicketDateError.ClosingTicketConcernUnable", "Approver Date Ticket request already in approval!");

        public static Error TicketConcernIdAlreadyExist() =>
         new Error("TicketDateError.TicketConcernIdAlreadyExist", "Ticket concern already exist!");

        public static Error ClosingTicketIdNotExist() =>
       new Error("TicketDateError.ReTicketIdNotExist", "Approver Date Ticket request not exist!");

        public static Error ClosingTicketIdAlreadyExist() =>
       new Error("TicketDateError.ClosingTicketIdAlreadyExist", "Approver Date Ticket request already exist!");

        public static Error NoApproverHasSetup() =>
        new Error("TicketDateError.NoApproverHasSetup", "No aprrover has been setup!");
    }
}
