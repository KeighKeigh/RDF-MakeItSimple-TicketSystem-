﻿using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class TicketRequestError
    {
      public static Error TicketIdNotExist() =>
      new Error("TicketRequest.TicketNotExist", "Ticket transaction not exist!");
      public static Error RequestConcernIdNotExist() =>
      new Error("TicketRequest.RequestConcernIdNotExist", "Request concern not exist!");
      public static Error TicketConcernIdNotExist() =>
      new Error("TicketRequest.TicketConcernIdNotExist", "Ticket concern not exist!");
      
      public static Error ConcernDetailsNotNull() =>
      new Error("TicketRequest.ConcernDetailsNotNull", "Concern details must not be empty!");

      public static Error ChannelNotExist() =>
      new Error("TicketRequest.ChannelNotExist", "Channel not exist!");

      public static Error UserNotExist() =>
      new Error("TicketRequest.UserNotExist", "user not exist!");

      public static Error CategoryNotExist() =>
      new Error("TicketRequest.CategoryNotExist", "Category not exist!");
      
      public static Error SubCategoryNotExist() =>
      new Error("TicketRequest.SubCategoryNotExist", "Sub category not exist!");

      public static Error DateTimeInvalid() =>
      new Error("TicketRequest.DateTimeInvalid", "Invalid target date!");

      public static Error DuplicateConcern() =>
      new Error("TicketRequest.DuplicateConcern", "Concern ticket duplicated!");

      public static Error AttachmentNotNull() =>
      new Error("TicketRequest.AttachmentNotNull", "Attachment must not be empty!");

      public static Error AttachmentNotExist() =>
      new Error("TicketRequest.AttachmentNotExist", "Attachment not Exist!");

      public static Error AttachmentAlreadyExist() =>
      new Error("TicketRequest.AttachmentAlreadyExist", "Attachment already exist!");

      public static Error InvalidAttachmentSize() =>
      new Error("TicketRequest.InvalidAttachmentSize", "Invalid Attachment Size!");
      
      public static Error InvalidAttachmentType() =>
      new Error("TicketRequest.InvalidAttachmentType", "Invalid Attachment Type!");

      public static Error ConcernWasInApproval() =>
      new Error("TicketRequest.ConcernWasInApproval", "Concern is for approval!");

      public static Error NotAutorizeToEdit() =>
      new Error("TicketRequest.NotAutorizeToEdit", "User not Autorize to edit!");

      public static Error NotAutorizeToDelete() =>
      new Error("TicketRequest.NotAutorizeToEdit", "User not Autorize to delete!");

        public static Error NotAutorize() =>
        new Error("TicketRequest.NotAutorize", "User not authorized!");

        public static Error ProhibitedWord(string comment) =>
      new Error("TicketRequest.ProhibitedWord", $"Warning Inappropriate word '{comment}'!");
      public static Error TicketCommentNotExist() =>
      new Error("TicketRequest.TicketCommentNotExist", "Comment not Exist!");

        public static Error NoReceiver() =>
            new Error("TicketRequest.NoReceiver", "Receiver is not configured");

        public static Error UnAuthorizedReceiver() =>
     new Error("TicketRequest.UnAuthorizedReceiver", "Receiver not authorized!");

     public static Error NoComment() =>
     new Error("TicketRequest.NoComment", "No comment has been made!");
     
     public static Error InvalidReturnTicket() =>
     new Error("TicketRequest.InvalidReturnTicket", "Return ticket invalid!");

     public static Error InvalidTargetDate() =>
     new Error("TicketRequest.InvalidTargetDate", "Invalid target date!");

     public static Error ConfirmAlready() =>
     new Error("TicketRequest.ConfirmAlready", "Concern already been confirm!");

     public static Error TransactionNotExist() =>
     new Error("TicketRequest.TransactionNotExist", "Ticketing Transaction not exist!");


    public static Error AttachmentNotFound() =>
    new Error("TicketRequest.AttachmentNotFound", "Attachment not found!");

     public static Error FileNotFound() =>
     new Error("TicketRequest.FileNotFound", "File not found!");

        public static Error CompanyNotExist() =>
        new Error("TicketRequest.CompanyNotExist", "Company not exist!");

        public static Error BusinessUnitNotExist() =>
        new Error("TicketRequest.BusinessUnitNotExist", "Business unit not exist!");

        public static Error DepartmentNotExist() =>
        new Error("TicketRequest.DepartmentNotExist", "Department not exist!");

        public static Error UnitNotExist() =>
        new Error("TicketRequest.UnitNotExist", "Unit not exist!");

        public static Error SubUnitNotExist() =>
        new Error("TicketRequest.SubUnitNotExist", "Sub unit not exist!");

        public static Error LocationNotExist() =>
        new Error("TicketRequest.LocationNotExist", "Location not exist!");

        public static Error TicketAlreadyAssign() =>
        new Error("TicketRequest.TicketAlreadyAssign", "Request concern already assigned!");

         public static Error TicketAlreadyCancel() =>
        new Error("TicketRequest.TicketAlreadyCancel", "Request concern already cancelled!");

       public static Error TicketAlreadyApproved() =>
       new Error("TicketRequest.TicketAlreadyApprove", "Ticket concern already approved!");

        public static Error TicketAlreadyReject() =>
       new Error("TicketRequest.TicketAlreadyReject", "Ticket concern already rejected!");

    }
}
