using System.Collections.Generic;

namespace Invoice.ViewModel
{
    public class GstrViewModel
    {
        public string InvoiceName { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public string Gstin { get; set; }
        public string Mobile { get; set; }
        public float TotalValue { get; set; }
        public float TotalDiscountRs { get; set; }
        public float TotalCgstAmount { get; set; }
        public float TotalSgstAmount { get; set; }
        public float TotalIgstAmount { get; set; }
        public float TotalTaxValue { get; set; }
        public float Total { get; set; }
    }

    public class GstrPrintViewModel
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

        public int TotalInvoices { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
        public float TotalValue { get; set; }
        public float TotalDiscountRs { get; set; }
        public float TotalCgstAmount { get; set; }
        public float TotalSgstAmount { get; set; }
        public float TotalIgstAmount { get; set; }
        public float TotalTaxValue { get; set; }
        public float Total { get; set; }

        public List<GstrViewModel> GstrViewModel { get; set; }
    }
}