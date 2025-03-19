using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.UpsertPms
{
    public partial class UpsertPms
    {

        public class Handler : IRequestHandler<UpsertPmsCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpsertPmsCommand command, CancellationToken cancellationToken)
            {

                Pms pms = null;
                var pmsTechnicianUpdateList = new List<int>();

                var userDetails = await unitOfWork.User
                    .UserExist(command.Added_By);

                var pmsFormIdNotExist = await unitOfWork.PmsForm
                    .PmsFormIdNotExist(command.PmsFormId);

                if (pmsFormIdNotExist is null)
                    return Result.Failure(PmsFormError.PmsFormIdNotExist());

                var pmsIdExist = await unitOfWork.Pms
                    .PmsIdNotExist(command.Id);

                if (pmsIdExist is null)
                {
                    var createPms = new Pms
                    {
                        PmsFormId = command.PmsFormId,
                        AddedBy = command.Added_By,
                        Requestor = command.Requestor,

                    };

                   await unitOfWork.Pms.Create(createPms);
                    pms = createPms;

                    var createPmsHistory = new PmsHistory
                    {
                        Pms = pms,
                        TransactedBy = command.Added_By,
                        TransactionDate = DateTime.Now,
                        Request = PmsConsString.Requested,
                        Status = $"{PmsConsString.PmsCreated} {userDetails.Fullname}"

                    };

                   await unitOfWork.Pms.CreateHistory(createPmsHistory);

                    var pmsApproverList = await unitOfWork.PmsApprover
                        .PmsApproverByPForm(command.PmsFormId.Value);

                    if (!pmsApproverList.Any())
                        return Result.Failure(PmsApproverError.NoApproverHasBeenSetup());

                    foreach(var approver in pmsApproverList)
                    {
                        var createApproval = new PmsApproval
                        {
                            Pms = pms,
                            UserId = approver.UserId,
                            AddedBy = command.Added_By,
                            ApproverLevel = approver.ApproverLevel

                        };

                        await unitOfWork.Pms.CreateApproval(createApproval);

                        var approverLevel = approver.ApproverLevel == 1 ? $"{approver.ApproverLevel}st"
                            : approver.ApproverLevel == 2 ? $"{approver.ApproverLevel}nd"
                            : approver.ApproverLevel == 3 ? $"{approver.ApproverLevel}rd"
                            : $"{approver.ApproverLevel}th";

                        var createPmsApprovalHistory = new PmsHistory
                        {
                            Pms = pms,
                            TransactedBy = approver.UserId,
                            TransactionDate = DateTime.Now,
                            Request = PmsConsString.ForApproval,
                            Status = $"{PmsConsString.PmsForApproval} {approverLevel} Approver",
                            Approver_Level = approver.ApproverLevel,

                        };

                      await unitOfWork.Pms.CreateHistory(createPmsApprovalHistory);
                    }


                }

                foreach(var questionModule  in command.UpsertQuestionModules)
                {
                    var pmsQuestionModuleIdNotExist = await unitOfWork.PmsQuestionaireModules
                        .PmsQuestionaireModuleIdNotExist(questionModule.PmsQuestionaireModuleId.Value);

                   if(pmsQuestionModuleIdNotExist is null)
                        return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleIdNotExist());

                   foreach(var detail in questionModule.PmsDetails)
                   {
                        var pmsQuestionExist = await unitOfWork.PmsQuestion
                            .PmsQuestionNotExist(detail.PmsQuestionaireId.Value);

                        if(pmsQuestionExist is null)
                            return Result.Failure(PmsQuestionaireError.PmsQuestionIdNotExist());

                        var pmsQuestionTypeExist = await unitOfWork.PmsQuestion
                            .PmsQTypeIdNotExist(detail.PmsQuestionTypeId);

                        if (pmsQuestionTypeExist is null)
                            return Result.Failure(PmsQuestionaireError.PmsQuestionTypeNotExist());


                        var pmsDetailIdExist = await unitOfWork.Pms
                              .PmsDetailIdNotExist(detail.PmsDetailId);

                        if(pmsDetailIdExist is not null)
                        {
                            var update = new PmsDetail
                            {
                                Id = detail.PmsDetailId.Value,
                                Answer = detail.Answer,
                            };

                          await unitOfWork.Pms.Update(update, command.Modified_By.Value);
                        }
                        else
                        {
                            var createDetail = new PmsDetail
                            {
                                Pms = pms,
                                PmsQuestionaireModuleId = questionModule.PmsQuestionaireModuleId,
                                PmsQuestionaireId = detail.PmsQuestionaireId,
                                PmsQuestionTypeId = detail.PmsQuestionTypeId,
                                Answer = detail.Answer,
                            };
                          await unitOfWork.Pms.CreateDetail(createDetail);
                        }

                   }

                }


                foreach (var technician in command.PmsTechnicians)
                {
                    var technicianExist = await unitOfWork.User.UserExist(technician.Technician);

                    if (technician is null)
                        return Result.Failure(PmsError.PmsTechnicianNotExist());

                    var pmsTechnicianExist = await unitOfWork.Pms.PmsTechnicianExist(technician.PmsTechnicianId);

                    if(pmsTechnicianExist is not null)
                    {
                        pmsTechnicianUpdateList.Add(pmsTechnicianExist.Id);
                    }
                    else
                    {
                        var createTechnician = new PmsTechnician
                        {
                            Pms = pms,
                            UserId = technician.Technician,
                        };

                      await unitOfWork.Pms.CreateTechnician(createTechnician);
                        
                    }

                }


                if(command.Id is not null)
                {
                    var pmsTechnicianList = await unitOfWork.Pms
                        .PmsTechnicianListByPms(command.Id.Value);

                    var removePmsTechnicianList = pmsTechnicianList
                        .Where(x => !pmsTechnicianUpdateList.Contains(x.Id))
                        .Select(x => x.Id);

                    if (removePmsTechnicianList.Any())
                    {
                        foreach (var remove in removePmsTechnicianList)
                        {
                           await unitOfWork.Pms.RemovePmsTechnician(remove);
                        }
                    }
                }



                if (!Directory.Exists(PmsConsString.AttachmentPath))
                {
                    Directory.CreateDirectory(PmsConsString.AttachmentPath);
                }

                if (command.PmsAttachments.Count(x => x.Attachment != null) > 0)
                {

                    foreach (var attachments in command.PmsAttachments.Where(a => a.Attachment.Length > 0))
                    {
                        if (attachments.Attachment.Length > 10 * 1024 * 1024)
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                        }

                        var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                        var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                        if (extension == null || !allowedFileTypes.Contains(extension))
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentType());
                        }

                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                        var pmsAttachmentExist = await unitOfWork.Pms
                            .PmsAttachmentExist(attachments.PmsAttachmentId);

                        if(pmsAttachmentExist is not null)
                        {
                            pmsAttachmentExist.Attachment = filePath;
                            pmsAttachmentExist.FileName = attachments.Attachment.FileName;
                            pmsAttachmentExist.FileSize = attachments.Attachment.Length;

                        }
                        else
                        {
                            var createAttachment = new PmsAttachment
                            {
                                Pms = pms,
                                Attachment = filePath,
                                FileName = attachments.Attachment.FileName,
                                FileSize = attachments.Attachment.Length,
                                AddedBy = command.Added_By,

                            };

                           await unitOfWork.Pms.CreateAttachment(createAttachment);

                        }

                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await attachments.Attachment.CopyToAsync(stream);
                        }

                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
                
            }
        }
    }
}
