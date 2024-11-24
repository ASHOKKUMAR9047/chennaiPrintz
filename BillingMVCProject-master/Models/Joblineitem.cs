using System;
using System.Collections.Generic;

namespace BillingMVCProject.Models
{
    public partial class Joblineitem
    {
        public int JobItemId { get; set; }
        public string JobCardId { get; set; } = null!;
        public int SerialNo { get; set; }
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string? Gsm { get; set; }
        public string? Imp { get; set; }
        public string? Side { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? MachineReading { get; set; }
        public string? EndMachineReading { get; set; }
        public string? TotImp { get; set; }
        public string? PlateNum { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
