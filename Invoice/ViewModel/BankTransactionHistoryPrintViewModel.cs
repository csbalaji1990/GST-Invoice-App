using System.Collections.Generic;

namespace Invoice.ViewModel
{
    public class BankTransactionHistoryViewModel
    {
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
    }

    public class BankTransactionHistoryPrintViewModel
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

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string BankName { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }

        public List<BankTransactionHistoryViewModel> BankTransactionHistoryViewModel { get; set; }
    }
}