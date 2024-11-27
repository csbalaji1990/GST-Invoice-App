using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Invoice.ViewModel
{
    public struct SalesInvoiceItemDetails
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string HsnSac { get; set; }
        public float Price { get; set; }
        public bool InclusiveTax { get; set; }
        public float Quantity { get; set; }
        public int UnitId { get; set; }
        public float SubAmount { get; set; }
        public float Discount { get; set; }
        public float DiscountRs { get; set; }
        public float TaxablePrice { get; set; }
        public float TaxableValue { get; set; }
        public float TaxRate { get; set; }
        public float CgstRate { get; set; }
        public float SgstRate { get; set; }
        public float IgstRate { get; set; }
        public float CgstAmount { get; set; }
        public float SgstAmount { get; set; }
        public float IgstAmount { get; set; }
        public float Cess { get; set; }
        public float CessRs { get; set; }
        public float Amount { get; set; }
        public float StockQuantity { get; set; }
    }

    public class SalesInvoiceViewModel
    {
        public SelectList CustomerList { get; set; }
        public SelectList PlaceOfSupplyList { get; set; }
        public SelectList PaymentMethodList { get; set; }
        public SelectList PaymentTermList { get; set; }

        public SelectList CountryList { get; set; }
        public SelectList StateList { get; set; }
        public SelectList CityList { get; set; }

        public SelectList UnitList { get; set; }
        public SelectList TaxList { get; set; }

        public int? InvoiceNo { get; set; }
        public string Prefix { get; set; }
        public string InvoiceName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int CustomerId { get; set; }
        public string PlaceOfSupply { get; set; }
        public bool ReverseCharge { get; set; }
        public string DeliveryNote { get; set; }
        public DateTime? DeliveryNoteDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentTerm { get; set; }
        public DateTime? DueDate { get; set; }
        public string SupplierReference { get; set; }
        public string OtherReference { get; set; }
        public string PoNo { get; set; }
        public DateTime? PoDate { get; set; }
        public string DespatchDocumentNo { get; set; }
        public string DespatchedThrough { get; set; }
        public float TotalValue { get; set; }
        public float TotalDiscountRs { get; set; }
        public float TotalTaxableValue { get; set; }
        public float TotalCgstAmount { get; set; }
        public float TotalSgstAmount { get; set; }
        public float TotalIgstAmount { get; set; }
        public float TotalTaxValue { get; set; }
        public float TotalCessRs { get; set; }
        public bool DiscountOnAll { get; set; }
        public float TotalDiscount { get; set; }
        public bool RoundOff { get; set; }
        public float RoundOffValue { get; set; }
        public float Total { get; set; }
        public string Terms { get; set; }
        public List<SalesInvoiceItemDetails> SalesInvoiceItemDetails { get; set; }
    }
}