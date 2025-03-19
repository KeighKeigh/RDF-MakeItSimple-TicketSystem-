﻿

namespace MakeItSimple.WebApi.Common.ConstantString
{
    public class TicketingConString
    {

        public const string Approval = "For Approval";
        public const string OnGoing = "Ongoing";
        public const string Done = "Done";

        public const string Users = "Users";
        public const string ApproverTransfer = "Admin";

        // Ticketing Role

        public const string Admin = "Admin";

        public const string Approver = "Approver";
        public const string Requestor = "Requestor";
        public const string Receiver = "Receiver";
        public const string IssueHandler = "Issue Handler";
        public const string Support = "Support";

        //Request History

        public const string RequestCreated = "A concern was requested by ";
        public const string ConcernCreated = "A concern was created by";
        public const string RequestAssign = "A concern was assigned to ";

        //Close History

        public const string CloseRequest = "Ticket closure requested";
        public const string CloseForApproval = "Ticket closure pending approval from";
        public const string CloseCancel = "Ticket closure request canceled";
        public const string CloseReject = "Ticket closure request rejected by";
        public const string CloseApprove = "Ticket closure request approved by";
        public const string CloseApproveReceiver = "Receiver approved the ticket closure request";
        public const string CloseConfirm = "Ticket confirmed as resolved";
        public const string CloseForConfirmation = "Ticket closure for confirmation by";
        public const string CloseReturn = "Ticket remains open due to unresolved issues";

        //Transfer History

        public const string TransferRequest = "Ticket transfer requested";

        public const string TransferForApproval = "Ticket transfer pending approval from";

        public const string TransferCancel = "Ticket transfer request canceled";
        public const string TransferReject = "Ticket transfer request rejected by";
        public const string TransferApprove = "Ticket transfer request approved by";

        public const string TransferApproval = "Transfer Approval";

        // OnHold History

        public const string OnHoldRequest = "Ticket was on-hold";

        public const string OnHoldForApproval = "Ticket on-hold pending approval from";
        public const string OnHoldApproved = "Ticket on-hold request approved by";
        public const string OnHoldCancel = "Ticket on-hold request canceled";
        public const string OnHoldReject = "Ticket on-hold request rejected by";

        // Concern Status 
        public const string ConcernAssign = "Assign";
        public const string Reject = "Rejected";
        public const string Cancel = "Cancel";
        public const string Confirm = "Confirmed";
        public const string Disapprove = "Disapprove";

        public const string ForApprovalTicket = "For Approval";
        public const string CurrentlyFixing = "Ongoing";
        public const string ConcernStatus = "ConcernStatus";

        public const string Concern = "Concern";
        public const string Manual = "Manual";
        public const string RequestTicket = "Request Ticket";

        public const string ForReticket = "For Re-Ticket";
        public const string ForClosing = "For Closing Ticket";
        public const string ForTransfer = "For Transfer";
        public const string OpenTicket = "Open Ticket";
        public const string ForOnHold = "For On-Hold";
        public const string OnHold = "On-Hold";
        public const string Resume = "Ticket was resume";
        public const string PendingRequest = "Pending Request";
        public const string ForReDate = "For Re-Date";
        public const string ClosedTicket = "ClosedTicket";
        public const string TransferTicket = "Transfer Ticket";
        public const string CloseTicket = "Closing Ticket";
        public const string Open = "Open Ticket";
        public const string ReTicket = "Re-Ticket";
        public const string ReDate = "Re-Date";
        public const string Closed = "Closed";
        public const string Transfer = "Transfer";
        public const string Request = "Request Concern";
        public const string NotConfirm = "For Confirmation";
        public const string Approve = "Approved";
        public const string TicketClosed = "Ticket Closed";
        public const string RequestApproval = "For approval by ";
        public const string OnTime = "On-Time";
        public const string Delay = "Delay";
        public const string Technician = "Technician";

        //

        public const string Today = "Today";
        public const string Yesterday = "Yesterday";
        public const string ThreeDays = "ThreeDays";
        public const string Week = "Week";
        public const string TwoWeeks = "TwoWeeks";
        public const string Month = "Month";
        public const string SixMonths = "SixMonths";
        public const string Year = "Year";

        // Attachment

        public const string AttachmentPath = @"C:\inetpub\vhosts\rdfmis.com\httpdocs\MIS_Assets\Tickets";
        public const string AttachmentProfilePath = @"C:\inetpub\vhosts\rdfmis.com\httpdocs\MIS_Assets\Profile";


    }
}
