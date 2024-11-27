using System.Collections.Generic;

namespace Invoice.ViewModel
{
    public class LedgerViewModel
    {
        public string PaymentDate { get; set; }
        public string Description { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
    }

    public class LedgerPrintViewModel
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

        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierCity { get; set; }
        public string SupplierState { get; set; }
        public string SupplierPincode { get; set; }
        public string SupplierEmail { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerPincode { get; set; }
        public string CustomerEmail { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string OpeningBalance { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string Balance { get; set; }

        public List<LedgerViewModel> LedgerViewModel { get; set; }
    }
}