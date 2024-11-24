using System;
using System.Collections.Generic;

namespace BillingMVCProject.Models
{
    public partial class Jobcarddetail
    {
        public string JobCardId { get; set; } = null!;
        public DateOnly IssueDate { get; set; }
        public int CustomerId { get; set; }
    }
}
