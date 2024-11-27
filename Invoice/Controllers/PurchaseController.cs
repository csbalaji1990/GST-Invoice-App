using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using Invoice.ViewModel;
using System.Text.RegularExpressions;

namespace Invoice.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult InvoiceList()
        {
            ViewBag.List = from h in _db.DbPurchaseInvoiceHeaders join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId orderby h.InvoiceNo descending select new { h.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, h.InvoiceDate, h.SupplierId, s.SupplierName, h.Total, h.Paid, h.Balance, h.Status };

            var bankAccountList = (from b in _db.DbBankAccounts select new { BankAccountId = b.BankAccountId.ToString(), b.AccountName }).ToList();
            bankAccountList.Insert(0, new { BankAccountId = "", AccountName = "" });
            ViewBag.BankAccountList = new SelectList(bankAccountList, "BankAccountId", "AccountName");

            ViewBag.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");

            return View();
        }

        public JsonResult SaveLedger(int SupplierId, int InvoiceNo, string PaymentMethod, DateTime PaymentDate, int BankAccountId, float Amount, string Note)
        {
            try
            {
                var header = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

                if (header != null)
                {
                    var ledgerId = (from l in _db.DbSupplierLedgers orderby l.SupplierLedgerId descending select l.SupplierLedgerId).FirstOrDefault() + 1;
                    var paymentNo = (from p in _db.DbSupplierLedgers orderby p.PaymentNo descending select p.PaymentNo).FirstOrDefault() + 1;
                    var transactionId = (from t in _db.DbBankTransactions orderby t.BankTransactionId descending select t.BankTransactionId).FirstOrDefault() + 1;

                    if (paymentNo == null)
                        paymentNo = 1;

                    var bankTransaction = new DbBankTransaction
                    {
                        BankTransactionId = transactionId,
                        BankAccountId = BankAccountId,
                        TransactionDate = PaymentDate,
                        TransactionType = "Purchase",
                        Debit = Amount,
                        Description = "Sent Payment (PM-" + paymentNo + ") for Invoice " + header.Prefix + header.InvoiceName
                    };
                    _db.DbBankTransactions.InsertOnSubmit(bankTransaction);

                    var ledger = new DbSupplierLedger
                    {
                        SupplierLedgerId = ledgerId,
                        SupplierId = SupplierId,
                        InvoiceNo = InvoiceNo,
                        Prefix = "PM-",
                        PaymentNo = paymentNo,
                        PaymentMethod = PaymentMethod,
                        PaymentDate = PaymentDate,
                        BankAccountId = BankAccountId,
                        Debit = Amount,
                        Note = Note,
                        BankTransactionId = transactionId,
                        Description = "Payment (PM-" + paymentNo + ") to Invoice " + header.Prefix + header.InvoiceName
                    };
                    _db.DbSupplierLedgers.InsertOnSubmit(ledger);

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
                        bankAccount.Balance -= Amount;

                    _db.SubmitChanges();

                    var list = from h in _db.DbPurchaseInvoiceHeaders join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId orderby h.InvoiceNo descending select new { h.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, h.InvoiceDate, h.SupplierId, s.SupplierName, h.Total, h.Paid, h.Balance, h.Status };

                    return Json(new { success = true, List = list }, JsonRequestBehavior.AllowGet);
                }

                return Json(new {success = false, Message = "Invoice not found"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult InvoiceEntry(int? InvoiceNo)
        {
            ViewBag.ItemList = from i in _db.DbItems select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, Price = i.PurchasePrice, InclusiveTax = i.PurchasePriceInclusiveTax, i.UnitId, i.TaxRate, i.Cess };

            var model = new PurchaseInvoiceViewModel();

            var supplierList = (from s in _db.DbSuppliers select new { SupplierId = s.SupplierId.ToString(), s.SupplierName }).ToList();
            supplierList.Insert(0, new { SupplierId = "", SupplierName = "" });
            model.SupplierList = new SelectList(supplierList, "SupplierId", "SupplierName");

            model.PlaceOfSupplyList = new SelectList(_db.DbStateCodes, "StateName", "StateName");
            model.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");
            model.PaymentTermList = new SelectList(_db.DbPaymentTerms, "DueDays", "PaymentTermName");

            ViewBag.UnitList = (from u in _db.DbUnits orderby u.UnitName select new { UnitId = u.UnitId.ToString(), u.UnitName }).ToList();
            ViewBag.TaxList = (from t in _db.DbTaxes select new { t.TaxRate, t.TaxName }).ToList();

            if (InvoiceNo == null)
            {
                model.Prefix = (from h in _db.DbPurchaseInvoiceHeaders orderby h.InvoiceNo descending select h.Prefix).FirstOrDefault() ?? "PUR";

                var list = _db.DbPurchaseInvoiceHeaders.OrderBy(x => x.InvoiceNo).Select(x => x.InvoiceName);

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
                var header = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

                if (header != null)
                {
                    model.Prefix = header.Prefix;
                    model.InvoiceName = header.InvoiceName;
                    model.InvoiceDate = header.InvoiceDate;
                    model.SupplierId = header.SupplierId;
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

                    model.PurchaseInvoiceItemDetails = (from d in _db.DbPurchaseInvoiceItemDetails where d.InvoiceNo == InvoiceNo select new PurchaseInvoiceItemDetails { ItemCode = d.ItemCode, ItemName = d.ItemName, ItemDescription = d.ItemDescription, HsnSac = d.HsnSac, Price = d.Price, InclusiveTax = d.InclusiveTax, Quantity = d.Quantity, UnitId = d.UnitId, Discount = d.Discount, DiscountRs = d.DiscountRs, TaxableValue = d.TaxableValue, TaxRate = d.TaxRate, CgstRate = d.CgstRate, SgstRate = d.SgstRate, IgstRate = d.IgstRate, CgstAmount = d.CgstAmount, SgstAmount = d.SgstAmount, IgstAmount = d.IgstAmount, Amount = d.Amount }).ToList();

                    return View(model);
                }

                return RedirectToAction("InvoiceList");
            }
        }

        public JsonResult SaveInvoice(PurchaseInvoiceViewModel Model)
        {
            try
            {
                if (Model.InvoiceNo == null)
                {
                    var invoiceNo = (from h in _db.DbPurchaseInvoiceHeaders orderby h.InvoiceNo descending select h.InvoiceNo).FirstOrDefault() + 1;

                    var header = new DbPurchaseInvoiceHeader
                    {
                        InvoiceNo = invoiceNo,
                        Prefix = Model.Prefix,
                        InvoiceName = Model.InvoiceName,
                        InvoiceDate = Model.InvoiceDate,
                        SupplierId = Model.SupplierId,
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
                    _db.DbPurchaseInvoiceHeaders.InsertOnSubmit(header);

                    var itemDetails = new List<DbPurchaseInvoiceItemDetail>();

                    for (var i = 0; i <= Model.PurchaseInvoiceItemDetails.Count - 1; i++)
                    {
                        var invoiceItem = new DbPurchaseInvoiceItemDetail
                        {
                            Id = i + 1,
                            InvoiceNo = invoiceNo,
                            ItemCode = Model.PurchaseInvoiceItemDetails[i].ItemCode,
                            ItemName = Model.PurchaseInvoiceItemDetails[i].ItemName,
                            ItemDescription = Model.PurchaseInvoiceItemDetails[i].ItemDescription,
                            HsnSac = Model.PurchaseInvoiceItemDetails[i].HsnSac,
                            Price = Model.PurchaseInvoiceItemDetails[i].Price,
                            InclusiveTax = Model.PurchaseInvoiceItemDetails[i].InclusiveTax,
                            Quantity = Model.PurchaseInvoiceItemDetails[i].Quantity,
                            UnitId = Model.PurchaseInvoiceItemDetails[i].UnitId,
                            SubAmount = Model.PurchaseInvoiceItemDetails[i].SubAmount,
                            Discount = Model.PurchaseInvoiceItemDetails[i].Discount,
                            DiscountRs = Model.PurchaseInvoiceItemDetails[i].DiscountRs,
                            TaxablePrice = Model.PurchaseInvoiceItemDetails[i].TaxablePrice,
                            TaxableValue = Model.PurchaseInvoiceItemDetails[i].TaxableValue,
                            TaxRate = Model.PurchaseInvoiceItemDetails[i].TaxRate,
                            CgstRate = Model.PurchaseInvoiceItemDetails[i].CgstRate,
                            SgstRate = Model.PurchaseInvoiceItemDetails[i].SgstRate,
                            IgstRate = Model.PurchaseInvoiceItemDetails[i].IgstRate,
                            CgstAmount = Model.PurchaseInvoiceItemDetails[i].CgstAmount,
                            SgstAmount = Model.PurchaseInvoiceItemDetails[i].SgstAmount,
                            IgstAmount = Model.PurchaseInvoiceItemDetails[i].IgstAmount,
                            Cess = Model.PurchaseInvoiceItemDetails[i].Cess,
                            CessRs = Model.PurchaseInvoiceItemDetails[i].CessRs,
                            Amount = Model.PurchaseInvoiceItemDetails[i].Amount
                        };
                        itemDetails.Add(invoiceItem);

                        var item = (from m in _db.DbItems where m.ItemCode == Model.PurchaseInvoiceItemDetails[i].ItemCode select m).FirstOrDefault();

                        if (item != null)
                            item.StockQuantity += Model.PurchaseInvoiceItemDetails[i].Quantity;

                    }
                    _db.DbPurchaseInvoiceItemDetails.InsertAllOnSubmit(itemDetails);

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

                        var hsnSacDetails = new DbPurchaseInvoiceHsnSacDetail
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
                        _db.DbPurchaseInvoiceHsnSacDetails.InsertOnSubmit(hsnSacDetails);

                        j += 1;
                    }

                    var ledgerId = (from l in _db.DbSupplierLedgers orderby l.SupplierLedgerId descending select l.SupplierLedgerId).FirstOrDefault() + 1;

                    var ledger = new DbSupplierLedger
                    {
                        SupplierLedgerId = ledgerId,
                        SupplierId = header.SupplierId,
                        InvoiceNo = invoiceNo,
                        PaymentDate = header.InvoiceDate,
                        Credit = header.Total,
                        Description = "Invoice #" + header.Prefix + header.InvoiceName
                    };
                    _db.DbSupplierLedgers.InsertOnSubmit(ledger);

                    _db.SubmitChanges();

                    return Json(new { success = true, InvoiceNo = invoiceNo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var header = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == Model.InvoiceNo select h).FirstOrDefault();

                    if (header != null)
                    {
                        var deleteItemDetails = from d in _db.DbPurchaseInvoiceItemDetails where d.InvoiceNo == Model.InvoiceNo select d;
                        _db.DbPurchaseInvoiceItemDetails.DeleteAllOnSubmit(deleteItemDetails);

                        foreach (var d in deleteItemDetails)
                        {
                            var item = (from m in _db.DbItems where m.ItemCode == d.ItemCode select m).FirstOrDefault();

                            if (item != null)
                                item.StockQuantity -= d.Quantity;
                        }

                        var deleteHsnSacDetails = from d in _db.DbPurchaseInvoiceHsnSacDetails where d.InvoiceNo == Model.InvoiceNo select d;
                        _db.DbPurchaseInvoiceHsnSacDetails.DeleteAllOnSubmit(deleteHsnSacDetails);

                        var itemDetails = new List<DbPurchaseInvoiceItemDetail>();

                        for (var i = 0; i <= Model.PurchaseInvoiceItemDetails.Count - 1; i++)
                        {
                            var invoiceItem = new DbPurchaseInvoiceItemDetail
                            {
                                Id = i + 1,
                                InvoiceNo = (int)Model.InvoiceNo,
                                ItemCode = Model.PurchaseInvoiceItemDetails[i].ItemCode,
                                ItemName = Model.PurchaseInvoiceItemDetails[i].ItemName,
                                ItemDescription = Model.PurchaseInvoiceItemDetails[i].ItemDescription,
                                HsnSac = Model.PurchaseInvoiceItemDetails[i].HsnSac,
                                Price = Model.PurchaseInvoiceItemDetails[i].Price,
                                InclusiveTax = Model.PurchaseInvoiceItemDetails[i].InclusiveTax,
                                Quantity = Model.PurchaseInvoiceItemDetails[i].Quantity,
                                UnitId = Model.PurchaseInvoiceItemDetails[i].UnitId,
                                SubAmount = Model.PurchaseInvoiceItemDetails[i].SubAmount,
                                Discount = Model.PurchaseInvoiceItemDetails[i].Discount,
                                DiscountRs = Model.PurchaseInvoiceItemDetails[i].DiscountRs,
                                TaxablePrice = Model.PurchaseInvoiceItemDetails[i].TaxablePrice,
                                TaxableValue = Model.PurchaseInvoiceItemDetails[i].TaxableValue,
                                TaxRate = Model.PurchaseInvoiceItemDetails[i].TaxRate,
                                CgstRate = Model.PurchaseInvoiceItemDetails[i].CgstRate,
                                SgstRate = Model.PurchaseInvoiceItemDetails[i].SgstRate,
                                IgstRate = Model.PurchaseInvoiceItemDetails[i].IgstRate,
                                CgstAmount = Model.PurchaseInvoiceItemDetails[i].CgstAmount,
                                SgstAmount = Model.PurchaseInvoiceItemDetails[i].SgstAmount,
                                IgstAmount = Model.PurchaseInvoiceItemDetails[i].IgstAmount,
                                Cess = Model.PurchaseInvoiceItemDetails[i].Cess,
                                CessRs = Model.PurchaseInvoiceItemDetails[i].CessRs,
                                Amount = Model.PurchaseInvoiceItemDetails[i].Amount
                            };
                            itemDetails.Add(invoiceItem);

                            var item = (from m in _db.DbItems where m.ItemCode == Model.PurchaseInvoiceItemDetails[i].ItemCode select m).FirstOrDefault();

                            if (item != null)
                                item.StockQuantity += Model.PurchaseInvoiceItemDetails[i].Quantity;
                        }
                        _db.DbPurchaseInvoiceItemDetails.InsertAllOnSubmit(itemDetails);

                        header.Prefix = Model.Prefix;
                        header.InvoiceName = Model.InvoiceName;
                        header.InvoiceDate = Model.InvoiceDate;
                        header.SupplierId = Model.SupplierId;
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
                        header.Balance = Model.Total;

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

                            var hsnSacDetails = new DbPurchaseInvoiceHsnSacDetail
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
                            _db.DbPurchaseInvoiceHsnSacDetails.InsertOnSubmit(hsnSacDetails);

                            j += 1;
                        }

                        var ledger = (from l in _db.DbSupplierLedgers where l.InvoiceNo == Model.InvoiceNo select l).FirstOrDefault();

                        if (ledger != null)
                        {
                            ledger.SupplierId = Model.SupplierId;
                            ledger.PaymentDate = Model.InvoiceDate;
                            ledger.Credit = Model.Total;
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
                var deleteInvoice = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

                if (deleteInvoice != null)
                {
                    var deleteItemDetails = from d in _db.DbPurchaseInvoiceItemDetails where d.InvoiceNo == InvoiceNo select d;
                    _db.DbPurchaseInvoiceItemDetails.DeleteAllOnSubmit(deleteItemDetails);

                    foreach (var d in deleteItemDetails)
                    {
                        var item = (from m in _db.DbItems where m.ItemCode == d.ItemCode select m).FirstOrDefault();

                        if (item != null)
                            item.StockQuantity -= d.Quantity;
                    }

                    var deleteHsnSacDetails = from d in _db.DbPurchaseInvoiceHsnSacDetails where d.InvoiceNo == InvoiceNo select d;
                    _db.DbPurchaseInvoiceHsnSacDetails.DeleteAllOnSubmit(deleteHsnSacDetails);

                    var deleteLedger = (from l in _db.DbSupplierLedgers where l.InvoiceNo == InvoiceNo select l).FirstOrDefault();
                    if (deleteLedger != null) _db.DbSupplierLedgers.DeleteOnSubmit(deleteLedger);

                    _db.SubmitChanges();

                    _db.DbPurchaseInvoiceHeaders.DeleteOnSubmit(deleteInvoice);

                    _db.SubmitChanges();

                    var list = from h in _db.DbPurchaseInvoiceHeaders join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId orderby h.InvoiceNo descending select new { h.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, h.InvoiceDate, h.SupplierId, s.SupplierName, h.Total, h.Paid, h.Balance, h.Status };

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
        public ActionResult PrintInvoice(int InvoiceNo)
        {
            var model = new PurchaseInvoicePrintViewModel();

            var header = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == InvoiceNo select h).FirstOrDefault();

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

                var supplier = (from s in _db.DbSuppliers where s.SupplierId == header.SupplierId select s).FirstOrDefault();

                if (supplier != null)
                {
                    model.SupplierName = supplier.SupplierName;
                    model.SupplierAddress = supplier.Address;
                    model.SupplierCity = supplier.City;
                    model.SupplierState = supplier.State;
                    model.SupplierStateCode = (from s in _db.DbStateCodes where s.StateName == supplier.State select s.StateCode).FirstOrDefault();
                    model.SupplierPincode = (supplier.City != "" && supplier.Pincode != "") ? " - " + supplier.Pincode : supplier.Pincode;
                    model.SupplierGstin = supplier.Gstin;
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

                model.PurchaseInvoicePrintItemDetails = (from d in _db.DbPurchaseInvoiceItemDetails join u in _db.DbUnits on d.UnitId equals u.UnitId where d.InvoiceNo == InvoiceNo select new PurchaseInvoicePrintItemDetails { Id = d.Id, ItemCode = d.ItemCode, ItemName = d.ItemName, ItemDescription = d.ItemDescription, HsnSac = d.HsnSac, TaxablePrice = Helper.Inr(d.TaxablePrice), Quantity = d.Quantity, Unit = u.UnitName, Discount = d.Discount, DiscountRs = Helper.Inr(d.DiscountRs), SubAmount = Helper.Inr(d.SubAmount) }).ToList();

                model.PurchaseInvoicePrintHsnSacDetails = (from h in _db.DbPurchaseInvoiceHsnSacDetails where h.InvoiceNo == InvoiceNo select new PurchaseInvoicePrintHsnSacDetails { HsnSac = h.HsnSac, TaxableValue = Helper.Inr(h.TaxableValue), CgstRate = h.CgstRate, CgstAmount = Helper.Inr(h.CgstAmount), SgstRate = h.SgstRate, SgstAmount = Helper.Inr(h.SgstAmount), IgstRate = h.IgstRate, IgstAmount = Helper.Inr(h.IgstAmount), Amount = Helper.Inr(h.Amount) }).ToList();
            }

            return View(model);
        }
    }
}