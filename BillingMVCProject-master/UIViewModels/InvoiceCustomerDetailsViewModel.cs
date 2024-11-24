namespace BillingMVCProject.UIViewModels
{
    public class InvoiceCustomerDetailsViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceId { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? InvoiceOutStandingAmount { get; set; }

        public decimal? TotalOutstandingAmount { get; set; }

        public string InvoiceDate { get; set; }
    }

    public class InvoiceDetailsWithSum
    {
        public IEnumerable<InvoiceCustomerDetailsViewModel> InvoiceDetails { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
    }
}
