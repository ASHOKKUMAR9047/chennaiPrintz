using System.ComponentModel.DataAnnotations;

namespace BillingMVCProject.UIViewModels
{
    public class JobCardViewModel
    {
        public string JobCardId { get; set; }
        public DateOnly IssueDate { get; set; }

        public int JobItemid { get; set; }
        public DateTime IssueDatevalues { get; set; }
        public int CustomerId { get; set; }
        public List<CustomerDetailsViewModel> Customers { get; set; }
        public string CustomerName { get; set; }
        public List<JobItemViewModel> Items { get; set; }
    }

    public class JobItemViewModel
    {
        public int SerialNo { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string GSM { get; set; }
        public string IMP { get; set; }
        public string Side { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string MachineReading { get; set; }
        public string EndMachineReading { get; set; }
        public string TotIMP { get; set; }
        public string PlateNum { get; set; }
        public object InvoiceId { get; internal set; }

        public int JobItemId { get; set; }
    }

    public class CustomerDetailsViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; } // Add more fields as necessary
    }

}
