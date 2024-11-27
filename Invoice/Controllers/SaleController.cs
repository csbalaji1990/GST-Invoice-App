using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using Invoice.ViewModel;
using System.Text.RegularExpressions;

namespace Invoice.Controllers
{
    public class SaleController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult InvoiceList()
        {
            ViewBag.List = from h in _db.DbSalesInvoiceHeaders join c in _db.DbCustomers on h.CustomerId equals c.CustomerId orderby h.InvoiceNo descending select new { h.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, h.InvoiceDate, h.CustomerId, c.CustomerName, h.Total, h.Paid, h.Balance, h.Status };

            var bankAccountList = (from b in _db.DbBankAccounts select new { BankAccountId = b.BankAccountId.ToString(), b.AccountName }).ToList();
            bankAccountList.Insert(0, new { BankAccountId = "", AccountName = "" });
            ViewBag.BankAccountList = new SelectList(bankAccountList, "BankAccountId", "AccountName");

            ViewBag.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");

            return View();
        }

        public JsonResult SaveLedger(int CustomerId, int InvoiceNo, string PaymentMethod, DateTime PaymentDate, int BankAccountId, float Amount, string Note)
        {
            try
            {
                var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

                if (header != null)
                {
                    var ledgerId = (from l in _db.DbCustomerLedgers orderby l.CustomerLedgerId descending select l.CustomerLedgerId).FirstOrDefault() + 1;
                    var receiptNo = (from r in _db.DbCustomerLedgers orderby r.ReceiptNo descending select r.ReceiptNo).FirstOrDefault() + 1;
                    var transactionId = (from t in _db.DbBankTransactions orderby t.BankTransactionId descending select t.BankTransactionId).FirstOrDefault() + 1;

                    if (receiptNo == null)
                        receiptNo = 1;

                    var bankTransaction = new DbBankTransaction
                    {
                        BankTransactionId = transactionId,
                        BankAccountId = BankAccountId,
                        TransactionDate = PaymentDate,
                        TransactionType = "Sales",
                        Credit = Amount,
                        Description = "Received Payment (RC-" + receiptNo + ") for Invoice " + header.Prefix + header.InvoiceName
                    };
                    _db.DbBankTransactions.InsertOnSubmit(bankTransaction);

                    var ledger = new DbCustomerLedger
                    {
                        CustomerLedgerId = ledgerId,
                        CustomerId = CustomerId,
                        InvoiceNo = InvoiceNo,
                        Prefix = "RC-",
                        ReceiptNo = receiptNo,
                        PaymentMethod = PaymentMethod,
                        PaymentDate = PaymentDate,
                        BankAccountId = BankAccountId,
                        Credit = Amount,
                        Note = Note,
                        BankTransactionId = transactionId,
                        Description = "Payment (RC-" + receiptNo + ") to Invoice " + header.Prefix + header.InvoiceName
                    };
                    _db.DbCustomerLedgers.InsertOnSubmit(ledger);

                    header.Paid += Amount;
                    header.Balance = float.Parse((header.Total - header.Paid).ToString("0.##"));

                    if (header.Balance == header.Total)
                        header.Status = "Pending";

                    if (header.Balance > 0 && header.Balance < header.Total)
                        header.Status = "Partially";

                    if (header.Balance == 0)
                        header.Status = "Paid";

                    var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                    if (bankAccount != null)
                        bankAccount.Balance += Amount;

                    _db.SubmitChanges();

                    var list = from h in _db.DbSalesInvoiceHeaders join c in _db.DbCustomers on h.CustomerId equals c.CustomerId orderby h.InvoiceNo descending select new { h.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, h.InvoiceDate, h.CustomerId, c.CustomerName, h.Total, h.Paid, h.Balance, h.Status };

                    return Json(new { success = true, List = list }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Invoice not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult InvoiceEntry(int? InvoiceNo)
        {
            ViewBag.ItemList = from i in _db.DbItems select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, Price = i.SalesPrice, InclusiveTax = i.SalesPriceInclusiveTax, i.UnitId, i.TaxRate, i.Cess, i.StockQuantity };

            var model = new SalesInvoiceViewModel();

            var customerList = (from c in _db.DbCustomers select new { CustomerId = c.CustomerId.ToString(), c.CustomerName }).ToList();
            customerList.Insert(0, new { CustomerId = "", CustomerName = "" });
            model.CustomerList = new SelectList(customerList, "CustomerId", "CustomerName");

            model.PlaceOfSupplyList = new SelectList(_db.DbStateCodes, "StateName", "StateName");
            model.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");
            model.PaymentTermList = new SelectList(_db.DbPaymentTerms, "DueDays", "PaymentTermName");

            ViewBag.UnitList = (from u in _db.DbUnits orderby u.UnitName select new { UnitId = u.UnitId.ToString(), u.UnitName }).ToList();
            ViewBag.TaxList = (from t in _db.DbTaxes select new { t.TaxRate, t.TaxName }).ToList();

            if (InvoiceNo == null)
            {
                model.Prefix = (from h in _db.DbSalesInvoiceHeaders orderby h.InvoiceNo descending select h.Prefix).FirstOrDefault() ?? "INV";

                var list = _db.DbSalesInvoiceHeaders.OrderBy(x => x.InvoiceNo).Select(x => x.InvoiceName);

                foreach (var i in list)
                {
                    if (Regex.IsMatch(i, "^[0-9]*$"))
                        model.InvoiceName = (int.Parse(i) + 1).ToString();
                }

                if (model.InvoiceName == null)
                    model.InvoiceName = "1";

                model.InvoiceDate = DateTime.Now.Date;
                model.PaymentMethod = "Cash";
                model.PaymentTerm = "Due on Receipt";
                model.DueDate = DateTime.Now.Date;
                model.PlaceOfSupply = Session["companyState"].ToString();

                return View(model);
            }
            else
            {
                var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

                if (header != null)
                {
                    model.Prefix = header.Prefix;
                    model.InvoiceName = header.InvoiceName;
                    model.InvoiceDate = header.InvoiceDate;
                    model.CustomerId = header.CustomerId;
                    model.PlaceOfSupply = header.PlaceOfSupply;
                    model.ReverseCharge = header.ReverseCharge;
                    model.DiscountOnAll = header.DiscountOnAll;
                    model.TotalDiscount = header.TotalDiscount;
                    model.DeliveryNote = header.DeliveryNote;
                    model.DeliveryNoteDate = header.DeliveryNoteDate;
                    model.PaymentMethod = header.PaymentMethod;
                    model.PaymentMethod = header.PaymentMethod;
                    model.PaymentTerm = header.PaymentTerm;
                    model.DueDate = header.DueDate;
                    model.SupplierReference = header.SupplierReference;
                    model.OtherReference = header.OtherReference;
                    model.PoNo = header.PoNo;
                    model.PoDate = header.PoDate;
                    model.DespatchDocumentNo = header.DespatchDocumentNo;
                    model.DespatchedThrough = header.DespatchedThrough;
                    model.TotalValue = header.TotalValue;
                    model.TotalDiscountRs = header.TotalDiscountRs;
                    model.TotalTaxableValue = header.TotalTaxableValue;
                    model.TotalCgstAmount = header.TotalCgstAmount;
                    model.TotalSgstAmount = header.TotalSgstAmount;
                    model.TotalIgstAmount = header.TotalIgstAmount;
                    model.TotalTaxValue = header.TotalTaxValue;
                    model.TotalCessRs = header.TotalCessRs;
                    model.RoundOff = header.RoundOff;
                    model.RoundOffValue = header.RoundOffValue;
                    model.Total = header.Total;
                    model.Terms = header.Terms;

                    model.SalesInvoiceItemDetails = (from d in _db.DbSalesInvoiceItemDetails join i in _db.DbItems on d.ItemCode equals i.ItemCode where d.InvoiceNo == InvoiceNo select new SalesInvoiceItemDetails { ItemCode = d.ItemCode, ItemName = d.ItemName, ItemDescription = d.ItemDescription, HsnSac = d.HsnSac, Price = d.Price, InclusiveTax = d.InclusiveTax, Quantity = d.Quantity, UnitId = d.UnitId, Discount = d.Discount, DiscountRs = d.DiscountRs, TaxableValue = d.TaxableValue, TaxRate = d.TaxRate, CgstRate = d.CgstRate, SgstRate = d.SgstRate, IgstRate = d.IgstRate, CgstAmount = d.CgstAmount, SgstAmount = d.SgstAmount, IgstAmount = d.IgstAmount, Cess = d.Cess, CessRs = d.CessRs, Amount = d.Amount, StockQuantity = i.StockQuantity }).ToList();

                    return View(model);
                }

                return RedirectToAction("InvoiceList");
            }
        }

        public JsonResult SaveInvoice(SalesInvoiceViewModel Model)
        {
            try
            {
                if (Model.InvoiceNo == null)
                {
                    var invoiceNo = (from h in _db.DbSalesInvoiceHeaders orderby h.InvoiceNo descending select h.InvoiceNo).FirstOrDefault() + 1;

                    var header = new DbSalesInvoiceHeader
                    {
                        InvoiceNo = invoiceNo,
                        Prefix = Model.Prefix,
                        InvoiceName = Model.InvoiceName,
                        InvoiceDate = Model.InvoiceDate,
                        CustomerId = Model.CustomerId,
                        PlaceOfSupply = Model.PlaceOfSupply,
                        ReverseCharge = Model.ReverseCharge,
                        DiscountOnAll = Model.DiscountOnAll,
                        TotalDiscount = Model.TotalDiscount,
                        DeliveryNote = Model.DeliveryNote,
                        DeliveryNoteDate = Model.DeliveryNoteDate,
                        PaymentMethod = Model.PaymentMethod,
                        PaymentTerm = Model.PaymentTerm,
                        DueDate = Model.DueDate,
                        SupplierReference = Model.SupplierReference,
                        OtherReference = Model.OtherReference,
                        PoNo = Model.PoNo,
                        PoDate = Model.PoDate,
                        DespatchDocumentNo = Model.DespatchDocumentNo,
                        DespatchedThrough = Model.DespatchedThrough,
                        TotalValue = Model.TotalValue,
                        TotalDiscountRs = Model.TotalDiscountRs,
                        TotalTaxableValue = Model.TotalTaxableValue,
                        TotalCgstAmount = Model.TotalCgstAmount,
                        TotalSgstAmount = Model.TotalSgstAmount,
                        TotalIgstAmount = Model.TotalIgstAmount,
                        TotalTaxValue = Model.TotalTaxValue,
                        TotalCessRs = Model.TotalCessRs,
                        RoundOff = Model.RoundOff,
                        RoundOffValue = Model.RoundOffValue,
                        Total = Model.Total,
                        Terms = Model.Terms,
                        Paid = 0,
                        Balance = Model.Total,
                        Status = "Pending"
                    };
                    _db.DbSalesInvoiceHeaders.InsertOnSubmit(header);

                    var itemDetails = new List<DbSalesInvoiceItemDetail>();

                    for (var i = 0; i <= Model.SalesInvoiceItemDetails.Count - 1; i++)
                    {
                        var invoiceItem = new DbSalesInvoiceItemDetail
                        {
                            Id = i + 1,
                            InvoiceNo = invoiceNo,
                            ItemCode = Model.SalesInvoiceItemDetails[i].ItemCode,
                            ItemName = Model.SalesInvoiceItemDetails[i].ItemName,
                            ItemDescription = Model.SalesInvoiceItemDetails[i].ItemDescription,
                            HsnSac = Model.SalesInvoiceItemDetails[i].HsnSac,
                            Price = Model.SalesInvoiceItemDetails[i].Price,
                            InclusiveTax = Model.SalesInvoiceItemDetails[i].InclusiveTax,
                            Quantity = Model.SalesInvoiceItemDetails[i].Quantity,
                            UnitId = Model.SalesInvoiceItemDetails[i].UnitId,
                            SubAmount = Model.SalesInvoiceItemDetails[i].SubAmount,
                            Discount = Model.SalesInvoiceItemDetails[i].Discount,
                            DiscountRs = Model.SalesInvoiceItemDetails[i].DiscountRs,
                            TaxablePrice = Model.SalesInvoiceItemDetails[i].TaxablePrice,
                            TaxableValue = Model.SalesInvoiceItemDetails[i].TaxableValue,
                            TaxRate = Model.SalesInvoiceItemDetails[i].TaxRate,
                            CgstRate = Model.SalesInvoiceItemDetails[i].CgstRate,
                            SgstRate = Model.SalesInvoiceItemDetails[i].SgstRate,
                            IgstRate = Model.SalesInvoiceItemDetails[i].IgstRate,
                            CgstAmount = Model.SalesInvoiceItemDetails[i].CgstAmount,
                            SgstAmount = Model.SalesInvoiceItemDetails[i].SgstAmount,
                            IgstAmount = Model.SalesInvoiceItemDetails[i].IgstAmount,
                            Cess = Model.SalesInvoiceItemDetails[i].Cess,
                            CessRs = Model.SalesInvoiceItemDetails[i].CessRs,
                            Amount = Model.SalesInvoiceItemDetails[i].Amount
                        };
                        itemDetails.Add(invoiceItem);

                        var item = (from m in _db.DbItems where m.ItemCode == Model.SalesInvoiceItemDetails[i].ItemCode select m).FirstOrDefault();

                        if (item != null)
                            item.StockQuantity -= Model.SalesInvoiceItemDetails[i].Quantity;

                    }
                    _db.DbSalesInvoiceItemDetails.InsertAllOnSubmit(itemDetails);

                    var hsnSac = itemDetails.Select(x => x.HsnSac).Distinct();

                    var j = 1;

                    foreach (var h in hsnSac)
                    {
                        var taxableValue = from d in itemDetails where d.HsnSac == h select d.TaxableValue;
                        var cgstRate = (from d in itemDetails where d.HsnSac == h select d.CgstRate).FirstOrDefault();
                        var cgstAmount = from d in itemDetails where d.HsnSac == h select d.CgstAmount;
                        var sgstRate = (from d in itemDetails where d.HsnSac == h select d.SgstRate).FirstOrDefault();
                        var sgstAmount = from d in itemDetails where d.HsnSac == h select d.SgstAmount;
                        var igstRate = (from d in itemDetails where d.HsnSac == h select d.IgstRate).FirstOrDefault();
                        var igstAmount = from d in itemDetails where d.HsnSac == h select d.IgstAmount;

                        var hsnSacDetails = new DbSalesInvoiceHsnSacDetail
                        {
                            Id = j,
                            InvoiceNo = invoiceNo,
                            HsnSac = h,
                            TaxableValue = taxableValue.Sum(),
                            CgstRate = cgstRate,
                            CgstAmount = cgstAmount.Sum(),
                            SgstRate = sgstRate,
                            SgstAmount = sgstAmount.Sum(),
                            IgstRate = igstRate,
                            IgstAmount = igstAmount.Sum(),
                            Amount = cgstAmount.Sum() + sgstAmount.Sum() + igstAmount.Sum()
                        };
                        _db.DbSalesInvoiceHsnSacDetails.InsertOnSubmit(hsnSacDetails);

                        j += 1;
                    }

                    var ledgerId = (from l in _db.DbCustomerLedgers orderby l.CustomerLedgerId descending select l.CustomerLedgerId).FirstOrDefault() + 1;

                    var ledger = new DbCustomerLedger
                    {
                        CustomerLedgerId = ledgerId,
                        CustomerId = header.CustomerId,
                        InvoiceNo = invoiceNo,
                        PaymentDate = header.InvoiceDate,
                        Debit = header.Total,
                        Description = "Invoice #" + header.Prefix + header.InvoiceName
                    };
                    _db.DbCustomerLedgers.InsertOnSubmit(ledger);

                    _db.SubmitChanges();

                    return Json(new { success = true, InvoiceNo = invoiceNo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == Model.InvoiceNo select h).FirstOrDefault();

                    if (header != null)
                    {
                        var deleteItemDetails = from d in _db.DbSalesInvoiceItemDetails where d.InvoiceNo == Model.InvoiceNo select d;
                        _db.DbSalesInvoiceItemDetails.DeleteAllOnSubmit(deleteItemDetails);

                        foreach (var d in deleteItemDetails)
                        {
                            var item = (from m in _db.DbItems where m.ItemCode == d.ItemCode select m).FirstOrDefault();

                            if (item != null)
                                item.StockQuantity += d.Quantity;
                        }

                        var deleteHsnSacDetails = from d in _db.DbSalesInvoiceHsnSacDetails where d.InvoiceNo == Model.InvoiceNo select d;
                        _db.DbSalesInvoiceHsnSacDetails.DeleteAllOnSubmit(deleteHsnSacDetails);

                        var itemDetails = new List<DbSalesInvoiceItemDetail>();

                        for (var i = 0; i <= Model.SalesInvoiceItemDetails.Count - 1; i++)
                        {
                            var invoiceItem = new DbSalesInvoiceItemDetail
                            {
                                Id = i + 1,
                                InvoiceNo = (int)Model.InvoiceNo,
                                ItemCode = Model.SalesInvoiceItemDetails[i].ItemCode,
                                ItemName = Model.SalesInvoiceItemDetails[i].ItemName,
                                ItemDescription = Model.SalesInvoiceItemDetails[i].ItemDescription,
                                HsnSac = Model.SalesInvoiceItemDetails[i].HsnSac,
                                Price = Model.SalesInvoiceItemDetails[i].Price,
                                InclusiveTax = Model.SalesInvoiceItemDetails[i].InclusiveTax,
                                Quantity = Model.SalesInvoiceItemDetails[i].Quantity,
                                UnitId = Model.SalesInvoiceItemDetails[i].UnitId,
                                SubAmount = Model.SalesInvoiceItemDetails[i].SubAmount,
                                Discount = Model.SalesInvoiceItemDetails[i].Discount,
                                DiscountRs = Model.SalesInvoiceItemDetails[i].DiscountRs,
                                TaxablePrice = Model.SalesInvoiceItemDetails[i].TaxablePrice,
                                TaxableValue = Model.SalesInvoiceItemDetails[i].TaxableValue,
                                TaxRate = Model.SalesInvoiceItemDetails[i].TaxRate,
                                CgstRate = Model.SalesInvoiceItemDetails[i].CgstRate,
                                SgstRate = Model.SalesInvoiceItemDetails[i].SgstRate,
                                IgstRate = Model.SalesInvoiceItemDetails[i].IgstRate,
                                CgstAmount = Model.SalesInvoiceItemDetails[i].CgstAmount,
                                SgstAmount = Model.SalesInvoiceItemDetails[i].SgstAmount,
                                IgstAmount = Model.SalesInvoiceItemDetails[i].IgstAmount,
                                Cess = Model.SalesInvoiceItemDetails[i].Cess,
                                CessRs = Model.SalesInvoiceItemDetails[i].CessRs,
                                Amount = Model.SalesInvoiceItemDetails[i].Amount
                            };
                            itemDetails.Add(invoiceItem);

                            var item = (from m in _db.DbItems where m.ItemCode == Model.SalesInvoiceItemDetails[i].ItemCode select m).FirstOrDefault();

                            if (item != null)
                                item.StockQuantity -= Model.SalesInvoiceItemDetails[i].Quantity;
                        }
                        _db.DbSalesInvoiceItemDetails.InsertAllOnSubmit(itemDetails);

                        header.Prefix = Model.Prefix;
                        header.InvoiceName = Model.InvoiceName;
                        header.InvoiceDate = Model.InvoiceDate;
                        header.CustomerId = Model.CustomerId;
                        header.PlaceOfSupply = Model.PlaceOfSupply;
                        header.ReverseCharge = Model.ReverseCharge;
                        header.DiscountOnAll = Model.DiscountOnAll;
                        header.TotalDiscount = Model.TotalDiscount;
                        header.DeliveryNote = Model.DeliveryNote;
                        header.DeliveryNoteDate = Model.DeliveryNoteDate;
                        header.PaymentMethod = Model.PaymentMethod;
                        header.PaymentTerm = Model.PaymentTerm;
                        header.DueDate = Model.DueDate;
                        header.SupplierReference = Model.SupplierReference;
                        header.OtherReference = Model.OtherReference;
                        header.PoNo = Model.PoNo;
                        header.PoDate = Model.PoDate;
                        header.DespatchDocumentNo = Model.DespatchDocumentNo;
                        header.DespatchedThrough = Model.DespatchedThrough;
                        header.TotalValue = Model.TotalValue;
                        header.TotalDiscountRs = Model.TotalDiscountRs;
                        header.TotalTaxableValue = Model.TotalTaxableValue;
                        header.TotalCgstAmount = Model.TotalCgstAmount;
                        header.TotalSgstAmount = Model.TotalSgstAmount;
                        header.TotalIgstAmount = Model.TotalIgstAmount;
                        header.TotalTaxValue = Model.TotalTaxValue;
                        header.TotalCessRs = Model.TotalCessRs;
                        header.RoundOff = Model.RoundOff;
                        header.RoundOffValue = Model.RoundOffValue;
                        header.Total = Model.Total;
                        header.Balance = Model.Total;
                        header.Terms = Model.Terms;

                        var hsnSac = itemDetails.Select(x => x.HsnSac).Distinct();

                        var j = 1;

                        foreach (var h in hsnSac)
                        {
                            var taxableValue = from d in itemDetails where d.HsnSac == h select d.TaxableValue;
                            var cgstRate = (from d in itemDetails where d.HsnSac == h select d.CgstRate).FirstOrDefault();
                            var cgstAmount = from d in itemDetails where d.HsnSac == h select d.CgstAmount;
                            var sgstRate = (from d in itemDetails where d.HsnSac == h select d.SgstRate).FirstOrDefault();
                            var sgstAmount = from d in itemDetails where d.HsnSac == h select d.SgstAmount;
                            var igstRate = (from d in itemDetails where d.HsnSac == h select d.IgstRate).FirstOrDefault();
                            var igstAmount = from d in itemDetails where d.HsnSac == h select d.IgstAmount;

                            var hsnSacDetails = new DbSalesInvoiceHsnSacDetail
                            {
                                Id = j,
                                InvoiceNo = (int)Model.InvoiceNo,
                                HsnSac = h,
                                TaxableValue = taxableValue.Sum(),
                                CgstRate = cgstRate,
                                CgstAmount = cgstAmount.Sum(),
                                SgstRate = sgstRate,
                                SgstAmount = sgstAmount.Sum(),
                                IgstRate = igstRate,
                                IgstAmount = igstAmount.Sum(),
                                Amount = cgstAmount.Sum() + sgstAmount.Sum() + igstAmount.Sum()
                            };
                            _db.DbSalesInvoiceHsnSacDetails.InsertOnSubmit(hsnSacDetails);

                            j += 1;
                        }

                        var ledger = (from l in _db.DbCustomerLedgers where l.InvoiceNo == Model.InvoiceNo select l).FirstOrDefault();

                        if (ledger != null)
                        {
                            ledger.CustomerId = Model.CustomerId;
                            ledger.PaymentDate = Model.InvoiceDate;
                            ledger.Debit = Model.Total;
                        }

                        _db.SubmitChanges();

                        return Json(new { success = true, Model.InvoiceNo }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { success = false, Message = "Invoice not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteInvoice(int InvoiceNo)
        {
            try
            {
                var deleteInvoice = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

                if (deleteInvoice != null)
                {
                    var deleteItemDetails = from d in _db.DbSalesInvoiceItemDetails where d.InvoiceNo == InvoiceNo select d;
                    _db.DbSalesInvoiceItemDetails.DeleteAllOnSubmit(deleteItemDetails);

                    foreach (var d in deleteItemDetails)
                    {
                        var item = (from m in _db.DbItems where m.ItemCode == d.ItemCode select m).FirstOrDefault();

                        if (item != null)
                            item.StockQuantity += d.Quantity;
                    }

                    var deleteHsnSacDetails = from d in _db.DbSalesInvoiceHsnSacDetails where d.InvoiceNo == InvoiceNo select d;
                    _db.DbSalesInvoiceHsnSacDetails.DeleteAllOnSubmit(deleteHsnSacDetails);

                    var deleteLedger = (from l in _db.DbCustomerLedgers where l.InvoiceNo == InvoiceNo select l).FirstOrDefault();
                    if (deleteLedger != null) _db.DbCustomerLedgers.DeleteOnSubmit(deleteLedger);

                    _db.SubmitChanges();

                    _db.DbSalesInvoiceHeaders.DeleteOnSubmit(deleteInvoice);

                    _db.SubmitChanges();

                    var list = from h in _db.DbSalesInvoiceHeaders join c in _db.DbCustomers on h.CustomerId equals c.CustomerId orderby h.InvoiceNo descending select new { h.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, h.InvoiceDate, h.CustomerId, c.CustomerName, h.Total, h.Paid, h.Balance, h.Status };

                    return Json(new { success = true, List = list }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Invoice not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintInvoiceTemplate1(int InvoiceNo)
        {
            var model = new SalesInvoicePrintViewModel();

            var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

            if (header != null)
            {
                var company = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                if (company != null)
                {
                    model.CompanyLogo = company.LogoPath;
                    model.CompanyName = company.CompanyName;
                    model.CompanyAddress = company.Address;
                    model.CompanyCity = company.City;
                    model.CompanyPincode = (company.City != "" && company.Pincode != "") ? " - " + company.Pincode : company.Pincode;
                    model.CompanyStateCode = (from s in _db.DbStateCodes where s.StateName == company.State select s.StateCode).FirstOrDefault();
                    model.CompanyState = (company.City != "" || company.Pincode != "") ? ", " + company.State : company.State;
                    model.CompanyMobile = (company.Mobile != "") ? "Ph: " + company.Mobile : "";
                    model.CompanyLandline = (company.Mobile == "" && company.Landline != "") ? "Ph: " + company.Landline : (company.Mobile != "" && company.Landline != "") ? ", " + company.Landline : "";
                    model.CompanyEmail = (company.Email != "") ? "Email: " + company.Email : "";
                    model.CompanyWebsite = (company.Website != "") ? "Website: " + company.Website : "";
                    model.CompanyGstin = company.Gstin;
                    model.CompanyPan = company.Pan;
                    model.CompanyAccountNo = company.AccountNo;
                    model.CompanyAccountName = company.AccountName;
                    model.CompanyAccountType = company.AccountType;
                    model.CompanyBankName = company.BankName;
                    model.CompanyIfsc = company.Ifsc;
                    model.CompanyBranch = company.Branch;
                }

                var customer = (from c in _db.DbCustomers where c.CustomerId == header.CustomerId select c).FirstOrDefault();

                if (customer != null)
                {
                    model.CustomerName = customer.CustomerName;
                    model.CustomerAddress = customer.Address;
                    model.CustomerCity = customer.City;
                    model.CustomerState = customer.State;
                    model.CustomerStateCode = (from s in _db.DbStateCodes where s.StateName == customer.State select s.StateCode).FirstOrDefault();
                    model.CustomerPincode = (customer.City != "" && customer.Pincode != "") ? " - " + customer.Pincode : customer.Pincode;
                    model.CustomerGstin = customer.Gstin;
                    model.CustomerEmail = customer.Email;
                }

                model.Prefix = header.Prefix;
                model.InvoiceName = header.InvoiceName;
                model.InvoiceDate = header.InvoiceDate.ToString("dd-MM-yyyy");
                model.PlaceOfSupply = header.PlaceOfSupply;
                model.DeliveryNote = header.DeliveryNote;

                model.DeliveryNoteDate = header.DeliveryNoteDate != null ? Convert.ToDateTime(header.DeliveryNoteDate).ToString("dd-MM-yyyy") : "";

                model.PaymentMethod = header.PaymentMethod;
                model.PaymentTerm = header.PaymentTerm;
                model.SupplierReference = header.SupplierReference;
                model.OtherReference = header.OtherReference;
                model.PoNo = header.PoNo;

                model.PoDate = header.PoDate != null ? Convert.ToDateTime(header.PoDate).ToString("dd-MM-yyyy") : "";

                model.DespatchDocumentNo = header.DespatchDocumentNo;
                model.DespatchedThrough = header.DespatchedThrough;
                model.TotalDiscountRs = header.TotalDiscountRs.ToInr();
                model.TotalTaxableValue = header.TotalTaxableValue.ToInr();
                model.TotalCgstAmount = header.TotalCgstAmount.ToInr();
                model.TotalSgstAmount = header.TotalSgstAmount.ToInr();
                model.TotalIgstAmount = header.TotalIgstAmount.ToInr();
                model.TotalTaxValue = header.TotalTaxValue.ToInr();
                model.TotalCessRs = header.TotalCessRs.ToInr();
                model.RoundOff = header.RoundOff;
                model.RoundOffValue = header.RoundOffValue.ToInr();
                model.Total = header.Total.ToInr();
                model.Terms = header.Terms;
                model.TotalInWords = decimal.Parse(header.Total.ToString(), System.Globalization.NumberStyles.Any).ToString().ToWords();
                model.TotalTaxInWords = header.TotalTaxValue.ToString().ToWords();

                model.SalesInvoicePrintItemDetails = (from d in _db.DbSalesInvoiceItemDetails join u in _db.DbUnits on d.UnitId equals u.UnitId where d.InvoiceNo == InvoiceNo select new SalesInvoicePrintItemDetails { Id = d.Id, ItemCode = d.ItemCode, ItemName = d.ItemName, ItemDescription = d.ItemDescription, HsnSac = d.HsnSac, TaxablePrice = Helper.Inr(d.TaxablePrice), Quantity = d.Quantity, Unit = u.UnitName, Discount = d.Discount, DiscountRs = Helper.Inr(d.DiscountRs), SubAmount = Helper.Inr(d.SubAmount) }).ToList();

                model.SalesInvoicePrintHsnSacDetails = (from h in _db.DbSalesInvoiceHsnSacDetails where h.InvoiceNo == InvoiceNo select new SalesInvoicePrintHsnSacDetails { HsnSac = h.HsnSac, TaxableValue = Helper.Inr(h.TaxableValue), CgstRate = h.CgstRate, CgstAmount = Helper.Inr(h.CgstAmount), SgstRate = h.SgstRate, SgstAmount = Helper.Inr(h.SgstAmount), IgstRate = h.IgstRate, IgstAmount = Helper.Inr(h.IgstAmount), Amount = Helper.Inr(h.Amount) }).ToList();
            }

            return View(model);
        }

        [CheckIfLoggedIn]
        public ActionResult PrintInvoiceTemplate2(int InvoiceNo)
        {
            var model = new SalesInvoicePrintViewModel();

            var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

            if (header != null)
            {
                var company = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                if (company != null)
                {
                    model.CompanyLogo = company.LogoPath;
                    model.CompanyName = company.CompanyName;
                    model.CompanyAddress = company.Address;
                    model.CompanyCity = company.City;
                    model.CompanyPincode = (company.City != "" && company.Pincode != "") ? " - " + company.Pincode : company.Pincode;
                    model.CompanyStateCode = (from s in _db.DbStateCodes where s.StateName == company.State select s.StateCode).FirstOrDefault();
                    model.CompanyState = (company.City != "" || company.Pincode != "") ? ", " + company.State : company.State;
                    model.CompanyMobile = (company.Mobile != "") ? "Ph: " + company.Mobile : "";
                    model.CompanyLandline = (company.Mobile == "" && company.Landline != "") ? "Ph: " + company.Landline : (company.Mobile != "" && company.Landline != "") ? ", " + company.Landline : "";
                    model.CompanyEmail = (company.Email != "") ? "Email: " + company.Email : "";
                    model.CompanyWebsite = (company.Website != "") ? "Website: " + company.Website : "";
                    model.CompanyGstin = company.Gstin;
                    model.CompanyPan = company.Pan;
                    model.CompanyAccountNo = company.AccountNo;
                    model.CompanyAccountName = company.AccountName;
                    model.CompanyAccountType = company.AccountType;
                    model.CompanyBankName = company.BankName;
                    model.CompanyIfsc = company.Ifsc;
                    model.CompanyBranch = company.Branch;
                }

                var customer = (from c in _db.DbCustomers where c.CustomerId == header.CustomerId select c).FirstOrDefault();

                if (customer != null)
                {
                    model.CustomerName = customer.CustomerName;
                    model.CustomerAddress = customer.Address;
                    model.CustomerCity = customer.City;
                    model.CustomerState = customer.State;
                    model.CustomerStateCode = (from s in _db.DbStateCodes where s.StateName == customer.State select s.StateCode).FirstOrDefault();
                    model.CustomerPincode = (customer.City != "" && customer.Pincode != "") ? " - " + customer.Pincode : customer.Pincode;
                    model.CustomerGstin = customer.Gstin;
                    model.CustomerEmail = customer.Email;
                }

                model.Prefix = header.Prefix;
                model.InvoiceName = header.InvoiceName;
                model.InvoiceDate = header.InvoiceDate.ToString("dd-MM-yyyy");
                model.PlaceOfSupply = header.PlaceOfSupply;
                model.DeliveryNote = header.DeliveryNote;

                model.DeliveryNoteDate = header.DeliveryNoteDate != null ? Convert.ToDateTime(header.DeliveryNoteDate).ToString("dd-MM-yyyy") : "";

                model.PaymentMethod = header.PaymentMethod;
                model.PaymentTerm = header.PaymentTerm;
                model.SupplierReference = header.SupplierReference;
                model.OtherReference = header.OtherReference;
                model.PoNo = header.PoNo;

                model.PoDate = header.PoDate != null ? Convert.ToDateTime(header.PoDate).ToString("dd-MM-yyyy") : "";

                model.DespatchDocumentNo = header.DespatchDocumentNo;
                model.DespatchedThrough = header.DespatchedThrough;
                model.TotalDiscountRs = header.TotalDiscountRs.ToInr();
                model.TotalTaxableValue = header.TotalTaxableValue.ToInr();
                model.TotalCgstAmount = header.TotalCgstAmount.ToInr();
                model.TotalSgstAmount = header.TotalSgstAmount.ToInr();
                model.TotalIgstAmount = header.TotalIgstAmount.ToInr();
                model.TotalTaxValue = header.TotalTaxValue.ToInr();
                model.TotalCessRs = header.TotalCessRs.ToInr();
                model.RoundOff = header.RoundOff;
                model.RoundOffValue = header.RoundOffValue.ToInr();
                model.Total = header.Total.ToInr();
                model.Terms = header.Terms;
                model.TotalInWords = decimal.Parse(header.Total.ToString(), System.Globalization.NumberStyles.Any).ToString().ToWords();
                model.TotalTaxInWords = header.TotalTaxValue.ToString().ToWords();

                model.SalesInvoicePrintItemDetails = (from d in _db.DbSalesInvoiceItemDetails join u in _db.DbUnits on d.UnitId equals u.UnitId where d.InvoiceNo == InvoiceNo select new SalesInvoicePrintItemDetails { Id = d.Id, ItemCode = d.ItemCode, ItemName = d.ItemName, ItemDescription = d.ItemDescription, HsnSac = d.HsnSac, TaxablePrice = Helper.Inr(d.TaxablePrice), Quantity = d.Quantity, Unit = u.UnitName, Discount = d.Discount, DiscountRs = Helper.Inr(d.DiscountRs), SubAmount = Helper.Inr(d.SubAmount) }).ToList();

                model.SalesInvoicePrintHsnSacDetails = (from h in _db.DbSalesInvoiceHsnSacDetails where h.InvoiceNo == InvoiceNo select new SalesInvoicePrintHsnSacDetails { HsnSac = h.HsnSac, TaxableValue = Helper.Inr(h.TaxableValue), CgstRate = h.CgstRate, CgstAmount = Helper.Inr(h.CgstAmount), SgstRate = h.SgstRate, SgstAmount = Helper.Inr(h.SgstAmount), IgstRate = h.IgstRate, IgstAmount = Helper.Inr(h.IgstAmount), Amount = Helper.Inr(h.Amount) }).ToList();
            }

            return View(model);
        }

        [CheckIfLoggedIn]
        public ActionResult QuotationList()
        {
            ViewBag.List = from h in _db.DbQuotationHeaders join c in _db.DbCustomers on h.CustomerId equals c.CustomerId orderby h.QuotationNo descending select new { h.QuotationNo, QuotationName = h.Prefix + h.QuotationName, h.QuotationDate, h.CustomerId, c.CustomerName, h.Total };

            return View();
        }

        [CheckIfLoggedIn]
        public ActionResult QuotationEntry(int? QuotationNo)
        {
            ViewBag.ItemList = from i in _db.DbItems select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, Price = i.SalesPrice, InclusiveTax = i.SalesPriceInclusiveTax, i.UnitId, i.TaxRate, i.Cess };

            var model = new QuotationViewModel();

            var customerList = (from c in _db.DbCustomers select new { CustomerId = c.CustomerId.ToString(), c.CustomerName }).ToList();
            customerList.Insert(0, new { CustomerId = "", CustomerName = "" });
            model.CustomerList = new SelectList(customerList, "CustomerId", "CustomerName");

            model.PlaceOfSupplyList = new SelectList(_db.DbStateCodes, "StateName", "StateName");
            model.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");
            model.PaymentTermList = new SelectList(_db.DbPaymentTerms, "DueDays", "PaymentTermName");

            ViewBag.UnitList = (from u in _db.DbUnits orderby u.UnitName select new { UnitId = u.UnitId.ToString(), u.UnitName }).ToList();
            ViewBag.TaxList = (from t in _db.DbTaxes select new { t.TaxRate, t.TaxName }).ToList();

            if (QuotationNo == null)
            {
                model.Prefix = (from h in _db.DbQuotationHeaders orderby h.QuotationNo descending select h.Prefix).FirstOrDefault() ?? "QUO";

                var list = _db.DbQuotationHeaders.OrderBy(x => x.QuotationNo).Select(x => x.QuotationName);

                foreach (var i in list)
                {
                    if (Regex.IsMatch(i, "^[0-9]*$"))
                        model.QuotationName = (int.Parse(i) + 1).ToString();
                }

                if (model.QuotationName == null)
                    model.QuotationName = "1";

                model.QuotationDate = DateTime.Now.Date;
                model.PaymentMethod = "Cash";
                model.PaymentTerm = "Due on Receipt";
                model.DueDate = DateTime.Now.Date;
                model.PlaceOfSupply = Session["companyState"].ToString();

                return View(model);
            }
            else
            {
                var header = (from h in _db.DbQuotationHeaders where h.QuotationNo == QuotationNo select h).FirstOrDefault();

                if (header != null)
                {
                    model.Prefix = header.Prefix;
                    model.QuotationName = header.QuotationName;
                    model.QuotationDate = header.QuotationDate;
                    model.CustomerId = header.CustomerId;
                    model.PlaceOfSupply = header.PlaceOfSupply;
                    model.ReverseCharge = header.ReverseCharge;
                    model.DiscountOnAll = header.DiscountOnAll;
                    model.TotalDiscount = header.TotalDiscount;
                    model.DeliveryNote = header.DeliveryNote;
                    model.DeliveryNoteDate = header.DeliveryNoteDate;
                    model.PaymentMethod = header.PaymentMethod;
                    model.PaymentMethod = header.PaymentMethod;
                    model.PaymentTerm = header.PaymentTerm;
                    model.DueDate = header.DueDate;
                    model.SupplierReference = header.SupplierReference;
                    model.OtherReference = header.OtherReference;
                    model.PoNo = header.PoNo;
                    model.PoDate = header.PoDate;
                    model.DespatchDocumentNo = header.DespatchDocumentNo;
                    model.DespatchedThrough = header.DespatchedThrough;
                    model.TotalValue = header.TotalValue;
                    model.TotalDiscountRs = header.TotalDiscountRs;
                    model.TotalTaxableValue = header.TotalTaxableValue;
                    model.TotalCgstAmount = header.TotalCgstAmount;
                    model.TotalSgstAmount = header.TotalSgstAmount;
                    model.TotalIgstAmount = header.TotalIgstAmount;
                    model.TotalTaxValue = header.TotalTaxValue;
                    model.RoundOff = header.RoundOff;
                    model.RoundOffValue = header.RoundOffValue;
                    model.Total = header.Total;
                    model.Terms = header.Terms;

                    model.QuotationItemDetails = (from d in _db.DbQuotationItemDetails where d.QuotationNo == QuotationNo select new QuotationItemDetails { ItemCode = d.ItemCode, ItemName = d.ItemName, ItemDescription = d.ItemDescription, HsnSac = d.HsnSac, Price = d.Price, InclusiveTax = d.InclusiveTax, Quantity = d.Quantity, UnitId = d.UnitId, Discount = d.Discount, DiscountRs = d.DiscountRs, TaxableValue = d.TaxableValue, TaxRate = d.TaxRate, CgstRate = d.CgstRate, SgstRate = d.SgstRate, IgstRate = d.IgstRate, CgstAmount = d.CgstAmount, SgstAmount = d.SgstAmount, IgstAmount = d.IgstAmount, Amount = d.Amount }).ToList();

                    return View(model);
                }

                return RedirectToAction("QuotationList");
            }
        }

        public JsonResult SaveQuotation(QuotationViewModel Model)
        {
            try
            {
                if (Model.QuotationNo == null)
                {
                    var quotationNo = (from h in _db.DbQuotationHeaders orderby h.QuotationNo descending select h.QuotationNo).FirstOrDefault() + 1;

                    var header = new DbQuotationHeader
                    {
                        QuotationNo = quotationNo,
                        Prefix = Model.Prefix,
                        QuotationName = Model.QuotationName,
                        QuotationDate = Model.QuotationDate,
                        CustomerId = Model.CustomerId,
                        PlaceOfSupply = Model.PlaceOfSupply,
                        ReverseCharge = Model.ReverseCharge,
                        DiscountOnAll = Model.DiscountOnAll,
                        TotalDiscount = Model.TotalDiscount,
                        DeliveryNote = Model.DeliveryNote,
                        DeliveryNoteDate = Model.DeliveryNoteDate,
                        PaymentMethod = Model.PaymentMethod,
                        PaymentTerm = Model.PaymentTerm,
                        DueDate = Model.DueDate,
                        SupplierReference = Model.SupplierReference,
                        OtherReference = Model.OtherReference,
                        PoNo = Model.PoNo,
                        PoDate = Model.PoDate,
                        DespatchDocumentNo = Model.DespatchDocumentNo,
                        DespatchedThrough = Model.DespatchedThrough,
                        TotalValue = Model.TotalValue,
                        TotalDiscountRs = Model.TotalDiscountRs,
                        TotalTaxableValue = Model.TotalTaxableValue,
                        TotalCgstAmount = Model.TotalCgstAmount,
                        TotalSgstAmount = Model.TotalSgstAmount,
                        TotalIgstAmount = Model.TotalIgstAmount,
                        TotalTaxValue = Model.TotalTaxValue,
                        TotalCessRs = Model.TotalCessRs,
                        RoundOff = Model.RoundOff,
                        RoundOffValue = Model.RoundOffValue,
                        Total = Model.Total,
                        Terms = Model.Terms
                    };
                    _db.DbQuotationHeaders.InsertOnSubmit(header);

                    var itemDetails = new List<DbQuotationItemDetail>();

                    for (var i = 0; i <= Model.QuotationItemDetails.Count - 1; i++)
                    {
                        var quotationItem = new DbQuotationItemDetail
                        {
                            Id = i + 1,
                            QuotationNo = quotationNo,
                            ItemCode = Model.QuotationItemDetails[i].ItemCode,
                            ItemName = Model.QuotationItemDetails[i].ItemName,
                            ItemDescription = Model.QuotationItemDetails[i].ItemDescription,
                            HsnSac = Model.QuotationItemDetails[i].HsnSac,
                            Price = Model.QuotationItemDetails[i].Price,
                            InclusiveTax = Model.QuotationItemDetails[i].InclusiveTax,
                            Quantity = Model.QuotationItemDetails[i].Quantity,
                            UnitId = Model.QuotationItemDetails[i].UnitId,
                            SubAmount = Model.QuotationItemDetails[i].SubAmount,
                            Discount = Model.QuotationItemDetails[i].Discount,
                            DiscountRs = Model.QuotationItemDetails[i].DiscountRs,
                            TaxablePrice = Model.QuotationItemDetails[i].TaxablePrice,
                            TaxableValue = Model.QuotationItemDetails[i].TaxableValue,
                            TaxRate = Model.QuotationItemDetails[i].TaxRate,
                            CgstRate = Model.QuotationItemDetails[i].CgstRate,
                            SgstRate = Model.QuotationItemDetails[i].SgstRate,
                            IgstRate = Model.QuotationItemDetails[i].IgstRate,
                            CgstAmount = Model.QuotationItemDetails[i].CgstAmount,
                            SgstAmount = Model.QuotationItemDetails[i].SgstAmount,
                            IgstAmount = Model.QuotationItemDetails[i].IgstAmount,
                            Cess = Model.QuotationItemDetails[i].Cess,
                            CessRs = Model.QuotationItemDetails[i].CessRs,
                            Amount = Model.QuotationItemDetails[i].Amount
                        };
                        itemDetails.Add(quotationItem);
                    }
                    _db.DbQuotationItemDetails.InsertAllOnSubmit(itemDetails);

                    var hsnSac = itemDetails.Select(x => x.HsnSac).Distinct();

                    var j = 1;

                    foreach (var h in hsnSac)
                    {
                        var taxableValue = from d in itemDetails where d.HsnSac == h select d.TaxableValue;
                        var cgstRate = (from d in itemDetails where d.HsnSac == h select d.CgstRate).FirstOrDefault();
                        var cgstAmount = from d in itemDetails where d.HsnSac == h select d.CgstAmount;
                        var sgstRate = (from d in itemDetails where d.HsnSac == h select d.SgstRate).FirstOrDefault();
                        var sgstAmount = from d in itemDetails where d.HsnSac == h select d.SgstAmount;
                        var igstRate = (from d in itemDetails where d.HsnSac == h select d.IgstRate).FirstOrDefault();
                        var igstAmount = from d in itemDetails where d.HsnSac == h select d.IgstAmount;

                        var hsnSacDetails = new DbQuotationHsnSacDetail
                        {
                            Id = j,
                            QuotationNo = quotationNo,
                            HsnSac = h,
                            TaxableValue = taxableValue.Sum(),
                            CgstRate = cgstRate,
                            CgstAmount = cgstAmount.Sum(),
                            SgstRate = sgstRate,
                            SgstAmount = sgstAmount.Sum(),
                            IgstRate = igstRate,
                            IgstAmount = igstAmount.Sum(),
                            Amount = cgstAmount.Sum() + sgstAmount.Sum() + igstAmount.Sum()
                        };
                        _db.DbQuotationHsnSacDetails.InsertOnSubmit(hsnSacDetails);

                        j += 1;
                    }

                    _db.SubmitChanges();

                    return Json(new { success = true, QuotationNo = quotationNo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var header = (from h in _db.DbQuotationHeaders where h.QuotationNo == Model.QuotationNo select h).FirstOrDefault();

                    if (header != null)
                    {
                        var deleteItemDetails = from d in _db.DbQuotationItemDetails where d.QuotationNo == Model.QuotationNo select d;
                        _db.DbQuotationItemDetails.DeleteAllOnSubmit(deleteItemDetails);

                        var deleteHsnSacDetails = from d in _db.DbQuotationHsnSacDetails where d.QuotationNo == Model.QuotationNo select d;
                        _db.DbQuotationHsnSacDetails.DeleteAllOnSubmit(deleteHsnSacDetails);

                        var itemDetails = new List<DbQuotationItemDetail>();

                        for (var i = 0; i <= Model.QuotationItemDetails.Count - 1; i++)
                        {
                            var quotationItem = new DbQuotationItemDetail
                            {
                                Id = i + 1,
                                QuotationNo = (int)Model.QuotationNo,
                                ItemCode = Model.QuotationItemDetails[i].ItemCode,
                                ItemName = Model.QuotationItemDetails[i].ItemName,
                                ItemDescription = Model.QuotationItemDetails[i].ItemDescription,
                                HsnSac = Model.QuotationItemDetails[i].HsnSac,
                                Price = Model.QuotationItemDetails[i].Price,
                                InclusiveTax = Model.QuotationItemDetails[i].InclusiveTax,
                                Quantity = Model.QuotationItemDetails[i].Quantity,
                                UnitId = Model.QuotationItemDetails[i].UnitId,
                                SubAmount = Model.QuotationItemDetails[i].SubAmount,
                                Discount = Model.QuotationItemDetails[i].Discount,
                                DiscountRs = Model.QuotationItemDetails[i].DiscountRs,
                                TaxablePrice = Model.QuotationItemDetails[i].TaxablePrice,
                                TaxableValue = Model.QuotationItemDetails[i].TaxableValue,
                                TaxRate = Model.QuotationItemDetails[i].TaxRate,
                                CgstRate = Model.QuotationItemDetails[i].CgstRate,
                                SgstRate = Model.QuotationItemDetails[i].SgstRate,
                                IgstRate = Model.QuotationItemDetails[i].IgstRate,
                                CgstAmount = Model.QuotationItemDetails[i].CgstAmount,
                                SgstAmount = Model.QuotationItemDetails[i].SgstAmount,
                                IgstAmount = Model.QuotationItemDetails[i].IgstAmount,
                                Cess = Model.QuotationItemDetails[i].Cess,
                                CessRs = Model.QuotationItemDetails[i].CessRs,
                                Amount = Model.QuotationItemDetails[i].Amount
                            };
                            itemDetails.Add(quotationItem);
                        }
                        _db.DbQuotationItemDetails.InsertAllOnSubmit(itemDetails);

                        header.Prefix = Model.Prefix;
                        header.QuotationName = Model.QuotationName;
                        header.QuotationDate = Model.QuotationDate;
                        header.CustomerId = Model.CustomerId;
                        header.PlaceOfSupply = Model.PlaceOfSupply;
                        header.ReverseCharge = Model.ReverseCharge;
                        header.DiscountOnAll = Model.DiscountOnAll;
                        header.TotalDiscount = Model.TotalDiscount;
                        header.DeliveryNote = Model.DeliveryNote;
                        header.DeliveryNoteDate = Model.DeliveryNoteDate;
                        header.PaymentMethod = Model.PaymentMethod;
                        header.PaymentTerm = Model.PaymentTerm;
                        header.DueDate = Model.DueDate;
                        header.SupplierReference = Model.SupplierReference;
                        header.OtherReference = Model.OtherReference;
                        header.PoNo = Model.PoNo;
                        header.PoDate = Model.PoDate;
                        header.DespatchDocumentNo = Model.DespatchDocumentNo;
                        header.DespatchedThrough = Model.DespatchedThrough;
                        header.TotalValue = Model.TotalValue;
                        header.TotalDiscountRs = Model.TotalDiscountRs;
                        header.TotalTaxableValue = Model.TotalTaxableValue;
                        header.TotalCgstAmount = Model.TotalCgstAmount;
                        header.TotalSgstAmount = Model.TotalSgstAmount;
                        header.TotalIgstAmount = Model.TotalIgstAmount;
                        header.TotalTaxValue = Model.TotalTaxValue;
                        header.TotalCessRs = Model.TotalCessRs;
                        header.RoundOff = Model.RoundOff;
                        header.RoundOffValue = Model.RoundOffValue;
                        header.Total = Model.Total;
                        header.Terms = Model.Terms;

                        var hsnSac = itemDetails.Select(x => x.HsnSac).Distinct();

                        var j = 1;

                        foreach (var h in hsnSac)
                        {
                            var taxableValue = from d in itemDetails where d.HsnSac == h select d.TaxableValue;
                            var cgstRate = (from d in itemDetails where d.HsnSac == h select d.CgstRate).FirstOrDefault();
                            var cgstAmount = from d in itemDetails where d.HsnSac == h select d.CgstAmount;
                            var sgstRate = (from d in itemDetails where d.HsnSac == h select d.SgstRate).FirstOrDefault();
                            var sgstAmount = from d in itemDetails where d.HsnSac == h select d.SgstAmount;
                            var igstRate = (from d in itemDetails where d.HsnSac == h select d.IgstRate).FirstOrDefault();
                            var igstAmount = from d in itemDetails where d.HsnSac == h select d.IgstAmount;

                            var hsnSacDetails = new DbQuotationHsnSacDetail
                            {
                                Id = j,
                                QuotationNo = (int)Model.QuotationNo,
                                HsnSac = h,
                                TaxableValue = taxableValue.Sum(),
                                CgstRate = cgstRate,
                                CgstAmount = cgstAmount.Sum(),
                                SgstRate = sgstRate,
                                SgstAmount = sgstAmount.Sum(),
                                IgstRate = igstRate,
                                IgstAmount = igstAmount.Sum(),
                                Amount = cgstAmount.Sum() + sgstAmount.Sum() + igstAmount.Sum()
                            };
                            _db.DbQuotationHsnSacDetails.InsertOnSubmit(hsnSacDetails);

                            j += 1;
                        }

                        _db.SubmitChanges();

                        return Json(new { success = true, Model.QuotationNo }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { success = false, Message = "Quotation not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteQuotation(int QuotationNo)
        {
            try
            {
                var deleteQuotation = (from h in _db.DbQuotationHeaders where h.QuotationNo == QuotationNo select h).FirstOrDefault();

                if (deleteQuotation != null)
                {
                    var deleteItemDetails = from d in _db.DbQuotationItemDetails where d.QuotationNo == QuotationNo select d;
                    _db.DbQuotationItemDetails.DeleteAllOnSubmit(deleteItemDetails);

                    var deleteHsnSacDetails = from d in _db.DbQuotationHsnSacDetails where d.QuotationNo == QuotationNo select d;
                    _db.DbQuotationHsnSacDetails.DeleteAllOnSubmit(deleteHsnSacDetails);

                    _db.SubmitChanges();

                    _db.DbQuotationHeaders.DeleteOnSubmit(deleteQuotation);

                    _db.SubmitChanges();

                    var list = from h in _db.DbQuotationHeaders join c in _db.DbCustomers on h.CustomerId equals c.CustomerId orderby h.QuotationNo descending select new { h.QuotationNo, QuotationName = h.Prefix + h.QuotationName, h.QuotationDate, h.CustomerId, c.CustomerName, h.Total };

                    return Json(new { success = true, List = list }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Quotation not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintQuotation(int QuotationNo)
        {
            var model = new QuotationPrintViewModel();

            var header = (from h in _db.DbQuotationHeaders where h.QuotationNo == QuotationNo select h).FirstOrDefault();

            if (header != null)
            {
                var company = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                if (company != null)
                {
                    model.CompanyLogo = company.LogoPath;
                    model.CompanyName = company.CompanyName;
                    model.CompanyAddress = company.Address;
                    model.CompanyCity = company.City;
                    model.CompanyPincode = (company.City != "" && company.Pincode != "") ? " - " + company.Pincode : company.Pincode;
                    model.CompanyStateCode = (from s in _db.DbStateCodes where s.StateName == company.State select s.StateCode).FirstOrDefault();
                    model.CompanyState = (company.City != "" || company.Pincode != "") ? ", " + company.State : company.State;
                    model.CompanyMobile = (company.Mobile != "") ? "Ph: " + company.Mobile : "";
                    model.CompanyLandline = (company.Mobile == "" && company.Landline != "") ? "Ph: " + company.Landline : (company.Mobile != "" && company.Landline != "") ? ", " + company.Landline : "";
                    model.CompanyEmail = (company.Email != "") ? "Email: " + company.Email : "";
                    model.CompanyWebsite = (company.Website != "") ? "Website: " + company.Website : "";
                    model.CompanyGstin = company.Gstin;
                    model.CompanyPan = company.Pan;
                    model.CompanyAccountNo = company.AccountNo;
                    model.CompanyAccountName = company.AccountName;
                    model.CompanyAccountType = company.AccountType;
                    model.CompanyBankName = company.BankName;
                    model.CompanyIfsc = company.Ifsc;
                    model.CompanyBranch = company.Branch;
                }

                var customer = (from c in _db.DbCustomers where c.CustomerId == header.CustomerId select c).FirstOrDefault();

                if (customer != null)
                {
                    model.CustomerName = customer.CustomerName;
                    model.CustomerAddress = customer.Address;
                    model.CustomerCity = customer.City;
                    model.CustomerState = customer.State;
                    model.CustomerStateCode = (from s in _db.DbStateCodes where s.StateName == customer.State select s.StateCode).FirstOrDefault();
                    model.CustomerPincode = (customer.City != "" && customer.Pincode != "") ? " - " + customer.Pincode : customer.Pincode;
                    model.CustomerGstin = customer.Gstin;
                    model.CustomerEmail = customer.Email;
                }

                model.Prefix = header.Prefix;
                model.QuotationName = header.QuotationName;
                model.QuotationDate = header.QuotationDate.ToString("dd-MM-yyyy");
                model.PlaceOfSupply = header.PlaceOfSupply;
                model.DeliveryNote = header.DeliveryNote;

                model.DeliveryNoteDate = header.DeliveryNoteDate != null ? Convert.ToDateTime(header.DeliveryNoteDate).ToString("dd-MM-yyyy") : "";

                model.PaymentMethod = header.PaymentMethod;
                model.PaymentTerm = header.PaymentTerm;
                model.SupplierReference = header.SupplierReference;
                model.OtherReference = header.OtherReference;
                model.PoNo = header.PoNo;

                model.PoDate = header.PoDate != null ? Convert.ToDateTime(header.PoDate).ToString("dd-MM-yyyy") : "";

                model.DespatchDocumentNo = header.DespatchDocumentNo;
                model.DespatchedThrough = header.DespatchedThrough;
                model.TotalDiscountRs = header.TotalDiscountRs.ToInr();
                model.TotalTaxableValue = header.TotalTaxableValue.ToInr();
                model.TotalCgstAmount = header.TotalCgstAmount.ToInr();
                model.TotalSgstAmount = header.TotalSgstAmount.ToInr();
                model.TotalIgstAmount = header.TotalIgstAmount.ToInr();
                model.TotalTaxValue = header.TotalTaxValue.ToInr();
                model.TotalCessRs = header.TotalCessRs.ToInr();
                model.RoundOff = header.RoundOff;
                model.RoundOffValue = header.RoundOffValue.ToInr();
                model.Total = header.Total.ToInr();
                model.Terms = header.Terms;
                model.TotalInWords = decimal.Parse(header.Total.ToString(), System.Globalization.NumberStyles.Any).ToString().ToWords();
                model.TotalTaxInWords = header.TotalTaxValue.ToString().ToWords();

                model.QuotationPrintItemDetails = (from d in _db.DbQuotationItemDetails join u in _db.DbUnits on d.UnitId equals u.UnitId where d.QuotationNo == QuotationNo select new QuotationPrintItemDetails { Id = d.Id, ItemCode = d.ItemCode, ItemName = d.ItemName, ItemDescription = d.ItemDescription, HsnSac = d.HsnSac, TaxablePrice = Helper.Inr(d.TaxablePrice), Quantity = d.Quantity, Unit = u.UnitName, Discount = d.Discount, DiscountRs = Helper.Inr(d.DiscountRs), SubAmount = Helper.Inr(d.SubAmount) }).ToList();

                model.QuotationPrintHsnSacDetails = (from h in _db.DbQuotationHsnSacDetails where h.QuotationNo == QuotationNo select new QuotationPrintHsnSacDetails { HsnSac = h.HsnSac, TaxableValue = Helper.Inr(h.TaxableValue), CgstRate = h.CgstRate, CgstAmount = Helper.Inr(h.CgstAmount), SgstRate = h.SgstRate, SgstAmount = Helper.Inr(h.SgstAmount), IgstRate = h.IgstRate, IgstAmount = Helper.Inr(h.IgstAmount), Amount = Helper.Inr(h.Amount) }).ToList();
            }

            return View(model);
        }
    }
}