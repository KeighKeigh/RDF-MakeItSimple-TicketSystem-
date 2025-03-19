﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;

namespace MakeItSimple.WebApi.Models.Setup.LocationSetup
{
    public class Location : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int LocationNo { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string Manual { get; set; }

        public int? SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public DateTime SyncDate { get; set; }

        public string SyncStatus { get; set; }


        //public ICollection<SubUnit> SubUnits { get; set; }


    }
}
