﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;

namespace MakeItSimple.WebApi.Models.Setup.CategorySetup
{
    public partial class Category : BaseEntity
    {
        public int Id {  get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User ModifiedByUser { get; set; }

        public string CategoryDescription { get; set; }

        public int ? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }

    }
}
