using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;

namespace Invoice.Controllers
{
    public class TransactionController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Payment()
        {
            var paymentList = from l in _db.DbSupplierLedgers
                              join h in _db.DbPurchaseInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo
                              join s in _db.DbSuppliers on l.SupplierId equals s.SupplierId
                              join b in _db.DbBankAccounts on l.BankAccountId equals b.BankAccountId
                              where l.Debit != null
                              orderby l.SupplierLedgerId descending
                              select new { l.SupplierLedgerId, PaymentNo = l.Prefix + l.PaymentNo, l.PaymentDate, InvoiceName = h.Prefix + h.InvoiceName, s.SupplierName, b.AccountName, l.PaymentMethod, l.Debit };

            ViewBag.PaymentList = paymentList;

            var bankAccountList = (from b in _db.DbBankAccounts select new { BankAccountId = b.BankAccountId.ToString(), b.AccountName }).ToList();
            bankAccountList.Insert(0, new { BankAccountId = "", AccountName = "" });
            ViewBag.BankAccountList = new SelectList(bankAccountList, "BankAccountId", "AccountName");

            ViewBag.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");

            return View();
        }

        public JsonResult GetPayment(int SupplierLedgerId)
        {
            try
            {
                var ledger = (from l in _db.DbSupplierLedgers join h in _db.DbPurchaseInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo where l.SupplierLedgerId == SupplierLedgerId select new { l.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, l.BankAccountId, l.PaymentMethod, l.Debit, PaymentDate = l.PaymentDate.ToString() }).FirstOrDefault();

                if (ledger != null)
                {
                    var invoiceBalance = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == ledger.InvoiceNo select h.Balance).FirstOrDefault();

                    return Json(new { success = true, Ledger = ledger, Balance = invoiceBalance + ledger.Debit }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Payment not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdatePayment(int SupplierLedgerId, string PaymentMethod, DateTime PaymentDate, int BankAccountId, float Amount, string Note)
        {
            try
            {
                var ledger = (from l in _db.DbSupplierLedgers where l.SupplierLedgerId == SupplierLedgerId select l).FirstOrDefault();

                if (ledger != null)
                {
                    var header = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == ledger.InvoiceNo select h).FirstOrDefault();

                    if (header != null)
                    {
                        header.Paid = header.Paid - float.Parse(ledger.Debit.ToString()) + Amount;
                        header.Balance = float.Parse((header.Total - header.Paid).ToString("0.##"));

                        if (header.Balance == header.Total)
                            header.Status = "Pending";

                        if (header.Balance > 0 && header.Balance < header.Total)
                            header.Status = "Partially";

                        if (header.Balance == 0)
                            header.Status = "Paid";

                        var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == ledger.BankTransactionId select t).FirstOrDefault();

                        if (transaction != null)
                        {
                            transaction.BankAccountId = BankAccountId;
                            transaction.TransactionDate = PaymentDate;
                            transaction.TransactionType = "Purchase";
                            transaction.Debit = Amount;
                            transaction.Description = "Payment for Invoice " + header.Prefix + header.InvoiceName;
                        }
                    }

                    if (ledger.BankAccountId == BankAccountId)
                    {
                        var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == ledger.BankAccountId select b).FirstOrDefault();

                        if (bankAccount != null)
                            bankAccount.Balance = bankAccount.Balance + float.Parse(ledger.Debit.ToString()) - Amount;
                    }
                    else
                    {
                        var oldBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == ledger.BankAccountId select b).FirstOrDefault();

                        if (oldBankAccount != null)
                            oldBankAccount.Balance += float.Parse(ledger.Debit.ToString());

                        var newBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                        if (newBankAccount != null)
                            newBankAccount.Balance -= Amount;
                    }

                    ledger.PaymentMethod = PaymentMethod;
                    ledger.PaymentDate = PaymentDate;
                    ledger.BankAccountId = BankAccountId;
                    ledger.Debit = Amount;
                    ledger.Note = Note;

                    _db.SubmitChanges();

                    var paymentList = from l in _db.DbSupplierLedgers
                                      join h in _db.DbPurchaseInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo
                                      join s in _db.DbSuppliers on l.SupplierId equals s.SupplierId
                                      join b in _db.DbBankAccounts on l.BankAccountId equals b.BankAccountId
                                      where l.Debit != null
                                      select new { l.SupplierLedgerId, PaymentNo = l.Prefix + l.PaymentNo, l.PaymentDate, InvoiceName = h.Prefix + h.InvoiceName, s.SupplierName, b.AccountName, l.PaymentMethod, l.Debit };

                    return Json(new { success = true, PaymentList = paymentList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Payment not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeletePayment(int SupplierLedgerId)
        {
            try
            {
                var deleteLedger = (from l in _db.DbSupplierLedgers where l.SupplierLedgerId == SupplierLedgerId select l).FirstOrDefault();

                if (deleteLedger != null)
                {
                    _db.DbSupplierLedgers.DeleteOnSubmit(deleteLedger);

                    _db.SubmitChanges();

                    var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == deleteLedger.BankTransactionId select t).FirstOrDefault();

                    if (transaction != null)
                        _db.DbBankTransactions.DeleteOnSubmit(transaction);

                    var header = (from h in _db.DbPurchaseInvoiceHeaders where h.InvoiceNo == deleteLedger.InvoiceNo select h).FirstOrDefault();

                    if (header != null)
                    {
                        header.Paid -= float.Parse(deleteLedger.Debit.ToString());
                        header.Balance = float.Parse((header.Total - header.Paid).ToString("0.##"));

                        if (header.Balance == header.Total)
                            header.Status = "Pending";

                        if (header.Balance > 0 && header.Balance < header.Total)
                            header.Status = "Partially";

                        if (header.Balance == 0)
                            header.Status = "Paid";
                    }

                    var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == deleteLedger.BankAccountId select b).FirstOrDefault();

                    if (bankAccount != null)
                        bankAccount.Balance += float.Parse(deleteLedger.Debit.ToString());

                    _db.SubmitChanges();

                    var paymentList = from l in _db.DbSupplierLedgers
                                      join h in _db.DbPurchaseInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo
                                      join s in _db.DbSuppliers on l.SupplierId equals s.SupplierId
                                      join b in _db.DbBankAccounts on l.BankAccountId equals b.BankAccountId
                                      where l.Debit != null
                                      select new { l.SupplierLedgerId, PaymentNo = l.Prefix + l.PaymentNo, l.PaymentDate, InvoiceName = h.Prefix + h.InvoiceName, s.SupplierName, b.AccountName, l.PaymentMethod, l.Debit };

                    return Json(new { success = true, PaymentList = paymentList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Payment not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult Receipt()
        {
            var receiptList = from l in _db.DbCustomerLedgers
                              join h in _db.DbSalesInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo
                              join c in _db.DbCustomers on l.CustomerId equals c.CustomerId
                              join b in _db.DbBankAccounts on l.BankAccountId equals b.BankAccountId
                              where l.Credit != null
                              orderby l.CustomerLedgerId descending
                              select new { l.CustomerLedgerId, ReceiptNo = l.Prefix + l.ReceiptNo, l.PaymentDate, InvoiceName = h.Prefix + h.InvoiceName, c.CustomerName, b.AccountName, l.PaymentMethod, l.Credit };

            ViewBag.ReceiptList = receiptList;

            var bankAccountList = (from b in _db.DbBankAccounts select new { BankAccountId = b.BankAccountId.ToString(), b.AccountName }).ToList();
            bankAccountList.Insert(0, new { BankAccountId = "", AccountName = "" });
            ViewBag.BankAccountList = new SelectList(bankAccountList, "BankAccountId", "AccountName");

            ViewBag.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");

            return View();
        }

        public JsonResult GetReceipt(int CustomerLedgerId)
        {
            try
            {
                var ledger = (from l in _db.DbCustomerLedgers join h in _db.DbSalesInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo where l.CustomerLedgerId == CustomerLedgerId select new { l.InvoiceNo, InvoiceName = h.Prefix + h.InvoiceName, l.BankAccountId, l.PaymentMethod, l.Credit, PaymentDate = l.PaymentDate.ToString() }).FirstOrDefault();

                if (ledger != null)
                {
                    var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == ledger.InvoiceNo select h).FirstOrDefault();

                    return Json(new { success = true, Ledger = ledger, Balance = header.Balance + ledger.Credit }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Receipt not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateReceipt(int CustomerLedgerId, string PaymentMethod, DateTime PaymentDate, int BankAccountId, float Amount, string Note)
        {
            try
            {
                var ledger = (from l in _db.DbCustomerLedgers where l.CustomerLedgerId == CustomerLedgerId select l).FirstOrDefault();

                if (ledger != null)
                {
                    var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == ledger.InvoiceNo select h).FirstOrDefault();

                    if (header != null)
                    {
                        header.Paid = header.Paid - float.Parse(ledger.Credit.ToString()) + Amount;
                        header.Balance = float.Parse((header.Total - header.Paid).ToString("0.##"));

                        if (header.Balance == header.Total)
                            header.Status = "Pending";

                        if (header.Balance > 0 && header.Balance < header.Total)
                            header.Status = "Partially";

                        if (header.Balance == 0)
                            header.Status = "Paid";

                        var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == ledger.BankTransactionId select t).FirstOrDefault();

                        if (transaction != null)
                        {
                            transaction.BankAccountId = BankAccountId;
                            transaction.TransactionDate = PaymentDate;
                            transaction.TransactionType = "Sales";
                            transaction.Credit = Amount;
                            transaction.Description = "Payment for Invoice " + header.Prefix + header.InvoiceName;
                        }
                    }

                    if (ledger.BankAccountId == BankAccountId)
                    {
                        var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == ledger.BankAccountId select b).FirstOrDefault();

                        if (bankAccount != null)
                            bankAccount.Balance = bankAccount.Balance - float.Parse(ledger.Credit.ToString()) + Amount;
                    }
                    else
                    {
                        var oldBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == ledger.BankAccountId select b).FirstOrDefault();

                        if (oldBankAccount != null)
                            oldBankAccount.Balance -= float.Parse(ledger.Credit.ToString());

                        var newBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                        if (newBankAccount != null)
                            newBankAccount.Balance += Amount;
                    }

                    ledger.PaymentMethod = PaymentMethod;
                    ledger.PaymentDate = PaymentDate;
                    ledger.BankAccountId = BankAccountId;
                    ledger.Credit = Amount;
                    ledger.Note = Note;

                    _db.SubmitChanges();

                    var receiptList = from l in _db.DbCustomerLedgers
                                      join h in _db.DbSalesInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo
                                      join c in _db.DbCustomers on l.CustomerId equals c.CustomerId
                                      join b in _db.DbBankAccounts on l.BankAccountId equals b.BankAccountId
                                      where l.Credit != null
                                      select new { l.CustomerLedgerId, ReceiptNo = l.Prefix + l.ReceiptNo, l.PaymentDate, InvoiceName = h.Prefix + h.InvoiceName, c.CustomerName, b.AccountName, l.PaymentMethod, l.Credit };

                    return Json(new { success = true, ReceiptList = receiptList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Receipt not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteReceipt(int CustomerLedgerId)
        {
            try
            {
                var deleteLedger = (from l in _db.DbCustomerLedgers where l.CustomerLedgerId == CustomerLedgerId select l).FirstOrDefault();

                if (deleteLedger != null)
                {
                    _db.DbCustomerLedgers.DeleteOnSubmit(deleteLedger);

                    _db.SubmitChanges();

                    var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == deleteLedger.BankTransactionId select t).FirstOrDefault();

                    if (transaction != null)
                        _db.DbBankTransactions.DeleteOnSubmit(transaction);

                    var header = (from h in _db.DbSalesInvoiceHeaders where h.InvoiceNo == deleteLedger.InvoiceNo select h).FirstOrDefault();

                    if (header != null)
                    {
                        header.Paid -= float.Parse(deleteLedger.Credit.ToString());
                        header.Balance = float.Parse((header.Total - header.Paid).ToString("0.##"));

                        if (header.Balance == header.Total)
                            header.Status = "Pending";

                        if (header.Balance > 0 && header.Balance < header.Total)
                            header.Status = "Partially";

                        if (header.Balance == 0)
                            header.Status = "Paid";
                    }

                    var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == deleteLedger.BankAccountId select b).FirstOrDefault();

                    if (bankAccount != null)
                        bankAccount.Balance -= float.Parse(deleteLedger.Credit.ToString());

                    _db.SubmitChanges();

                    var receiptList = from l in _db.DbCustomerLedgers
                                      join h in _db.DbSalesInvoiceHeaders on l.InvoiceNo equals h.InvoiceNo
                                      join c in _db.DbCustomers on l.CustomerId equals c.CustomerId
                                      join b in _db.DbBankAccounts on l.BankAccountId equals b.BankAccountId
                                      where l.Credit != null
                                      select new { l.CustomerLedgerId, ReceiptNo = l.Prefix + l.ReceiptNo, l.PaymentDate, InvoiceName = h.Prefix + h.InvoiceName, c.CustomerName, b.AccountName, l.PaymentMethod, l.Credit };

                    return Json(new { success = true, ReceiptList = receiptList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Receipt not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}