﻿using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class TransferTicketError
    {

      public static Error TicketIdNotExist() =>
      new Error("TransferTicketError.TicketNotExist", "Ticket transaction not exist!");

      public static Error TicketConcernIdNotExist() =>
      new Error("TransferTicketError.TicketConcernIdNotExist", "Ticket concern not exist!");

       public static Error TransferTicketConcernIdNotExist() =>
      new Error("TransferTicketError.TransferTicketConcernIdNotExist", "Transfer ticket concern not exist!");

      public static Error TicketConcernIdAlreadyExist() =>
      new Error("TransferTicketError.TicketConcernIdAlreadyExist", "Ticket concern already exist!");

      public static Error InvalidTransferTicket() =>
      new Error("TransferTicketError.InvalidTransferTicket", "Invalid transfer to the same user!");

      public static Error ApproverUnAuthorized() =>
      new Error("TransferTicketError.ApproverInvalid", "User not authorize to approve!");

      public static Error TransferConcernUnable() =>
      new Error("TransferTicketError.TransferConcernUnable", "Transfer Ticket request already in approval!");

      public static Error UpdateUnAuthorized() =>
      new Error("TransferTicketError.ApproverInvalid", "User not authorize to update!");

      public static Error UnitNotExist() =>
      new Error("TransferTicketError.UnitNotExist", "Unit not exist!");

      public static Error SubUnitNotExist() =>
      new Error("TransferTicketError.SubUnitNotExist", "Sub unit not exist!");

      public static Error ChannelNotExist() =>
      new Error("TransferTicketError.ChannelNotExist", "Channel not exist!");

      public static Error NoApproverExist() =>
      new Error("TransferTicketError.NoApproverExist", "No approver has been setup!");
      
      public static Error TransferTicketAlreadyExist() =>
      new Error("TransferTicketError.TransferTicketAlreadyExist", "Transfer ticket already exist!");

      public static Error UserNotExist() =>
      new Error("TransferTicketError.UserNotExist", "Member not exist!");
      public static Error DuplicateTransferTicket() =>
      new Error("TransferTicketError.DuplicateTransferTicket", "Transfer ticket duplicated!");

      public static Error DuplicateConcernTicket() =>
      new Error("TransferTicketError.DuplicateConcernTicket", "Request ticket duplicated!"); 

      public static Error DateTimeInvalid() =>
      new Error("TransferTicketError.DateTimeInvalid", "Invalid start/target date!");

      public static Error TransferInvalid() =>
      new Error("TransferTicketError.TransferInvalid", "Ticket concern have another transaction!");

      public static Error TransferAlreadyReject() =>
      new Error("TransferTicketError.TransferAlreadyReject", "Transfer concern already rejected!");

    }
}
