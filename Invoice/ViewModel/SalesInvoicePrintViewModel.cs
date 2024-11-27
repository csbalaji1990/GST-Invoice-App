using System.Collections.Generic;

namespace Invoice.ViewModel
{
    public struct SalesInvoicePrintItemDetails
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string HsnSac { get; set; }
        public float Quantity { get; set; }
        public string Unit { get; set; }
        public string TaxablePrice { get; set; }
        public float Discount { get; set; }
        public string DiscountRs { get; set; }
        public string SubAmount { get; set; }
    }

    public struct SalesInvoicePrintHsnSacDetails
    {
        public string HsnSac { get; set; }
        public string TaxableValue { get; set; }
        public float CgstRate { get; set; }
        public string CgstAmount { get; set; }
        public float SgstRate { get; set; }
        public string SgstAmount { get; set; }
        public float IgstRate { get; set; }
        public string IgstAmount { get; set; }
        public string Amount { get; set; }
    }

    public class SalesInvoicePrintViewModel
    {
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyStateCode { get; set; }
        public string CompanyPincode { get; set; }
        public string CompanyMobile { get; set; }
        public string CompanyLandline { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyGstin { get; set; }
        public string CompanyPan { get; set; }
        public string CompanyAccountNo { get; set; }
        public string CompanyAccountName { get; set; }
        public string CompanyAccountType { get; set; }
        public string CompanyBankName { get; set; }
        public string CompanyIfsc { get; set; }
        public string CompanyBranch { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerStateCode { get; set; }
        public string CustomerPincode { get; set; }
        public string CustomerGstin { get; set; }
        public string CustomerEmail { get; set; }

        public string Prefix { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceDate { get; set; }
        public string PlaceOfSupply { get; set; }
        public string DeliveryNote { get; set; }
        public string DeliveryNoteDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentTerm { get; set; }
        public string SupplierReference { get; set; }
        public string OtherReference { get; set; }
        public string PoNo { get; set; }
        public string PoDate { get; set; }
        public string DespatchDocumentNo { get; set; }
        public string DespatchedThrough { get; set; }
        public string TotalValue { get; set; }
        public string TotalDiscountRs { get; set; }
        public string TotalTaxableValue { get; set; }
        public string TotalCgstAmount { get; set; }
        public string TotalSgstAmount { get; set; }
        public string TotalIgstAmount { get; set; }
        public string TotalTaxValue { get; set; }
        public string TotalCessRs { get; set; }
        public bool RoundOff { get; set; }
        public string RoundOffValue { get; set; }
        public string Total { get; set; }
        public string Terms { get; set; }
        public string TotalInWords { get; set; }
        public string TotalTaxInWords { get; set; }

        public List<SalesInvoicePrintItemDetails> SalesInvoicePrintItemDetails { get; set; }
        public List<SalesInvoicePrintHsnSacDetails> SalesInvoicePrintHsnSacDetails { get; set; }
    }
}