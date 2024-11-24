using BillingMVCProject.Models;

namespace BillingMVCProject.UIViewModels
{
    public class IndexViewModel
    {
        public List<Customer> Customers { get; set; }
        public string JobCardId { get; set; }

        public int? CustomerId { get; set; }

        public List<JobCardItemViewModel> Items { get; set; }// Assuming Jobcardgenerate is your model for JobCard

    }
    public class Customer
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
    }

    public class JobCardItemViewModel
    {
        // Serial number of the item
        public int SerialNo { get; set; }

        // Description of the item
        public string Description { get; set; }

        // Size of the item
        public string Size { get; set; }

        // GSM (Grams per Square Meter) value
        public string GSM { get; set; }

        // Impressions
        public string IMP { get; set; }

        // Printing side (e.g., Front/Back)
        public string Side { get; set; }

        // Start time of the job
        public string StartTime { get; set; }

        // End time of the job
        public string EndTime { get; set; }

        // Initial machine reading
        public string MachineReading { get; set; }

        // Final machine reading
        public string EndMachineReading { get; set; }

        // Total impressions
        public string TotIMP { get; set; }

        // Plate number
        public string PlateNum { get; set; }
    }

}
