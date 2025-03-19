﻿using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.Models.Setup.Phase_Two
{
    public class PmsQuestionaireModule : BaseIdEntity
    {
        [Required]
        public string QuestionaireModuleName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public int PmsFormId { get; set; }
        public virtual PmsForm PmsForm { get; set; }

        public int? Order { get; set; }

        public ICollection<QuestionTransactionId> QuestionTransactionIds { get; set; }
    }
}
