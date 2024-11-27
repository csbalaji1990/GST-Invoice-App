using System.Collections.Generic;

namespace Invoice.ViewModel
{
    public class InventoryViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public float StockQuantity { get; set; }
        public float AlertQuantity { get; set; }
        public float PurchasePrice { get; set; }
        public float SalesPrice { get; set; }
    }

    public class InventoryPrintViewModel
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

        public string CurrentDateTime { get; set; }
        public int TotalItems { get; set; }
        public int TotalAlertItems { get; set; }

        public List<InventoryViewModel> InventoryViewModel { get; set; }
    }
}