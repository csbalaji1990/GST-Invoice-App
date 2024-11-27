using System.Collections.Generic;

namespace Invoice.ViewModel
{
    public class PendingViewModel
    {
        public string InvoiceName { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public float Total { get; set; }
        public float Paid { get; set; }
        public float Balance { get; set; }
    }

    public class PendingPrintViewModel
    {
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyPincode { get; set; }
        public string CompanyMobile { get; set; }
        public string CompanyLandline { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebsite { get; set; }

        public string TotalInvoices { get; set; }
        public string Total { get; set; }
        public string Paid { get; set; }
        public string Balance { get; set; }

        public List<PendingViewModel> PendingViewModel { get; set; }
    }
}