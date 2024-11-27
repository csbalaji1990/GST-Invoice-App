using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using Invoice.Models;
using Invoice.ViewModel;

namespace Invoice.Controllers
{
    public class BankingController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult BankAccount()
        {
            ViewBag.BankAccountList = from b in _db.DbBankAccounts select new { b.BankAccountId, b.AccountNo, b.AccountName, b.AccountType, b.BankName, b.Branch, b.Ifsc, b.Balance };

            return View();
        }

        public JsonResult GetBankAccount(int BankAccountId)
        {
            try
            {
                var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select new { b.BankAccountId, b.AccountNo, b.AccountName, b.AccountType, b.BankName, b.Branch, b.Ifsc, b.Balance }).FirstOrDefault();

                if (bankAccount != null)
                    return Json(new { success = true, BankAccount = bankAccount }, JsonRequestBehavior.AllowGet);

                return Json(new {success = false, Message = "Bank account not found"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveBankAccount(int BankAccountId, string AccountNo, string AccountName, string AccountType, string BankName, string Branch, string Ifsc, float Balance)
        {
            try
            {
                var updateBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                if (updateBankAccount != null)
                {
                    updateBankAccount.AccountName = AccountName;
                    updateBankAccount.AccountNo = AccountNo;
                    updateBankAccount.AccountType = AccountType;
                    updateBankAccount.BankName = BankName;
                    updateBankAccount.Branch = Branch;
                    updateBankAccount.Ifsc = Ifsc;
                    updateBankAccount.Balance = Balance;
                }
                else
                {
                    var newBankAccount = new DbBankAccount
                    {
                        BankAccountId = (from b in _db.DbBankAccounts orderby b.BankAccountId descending select b.BankAccountId).FirstOrDefault() + 1,
                        AccountName = AccountName,
                        AccountNo = AccountNo,
                        AccountType = AccountType,
                        BankName = BankName,
                        Branch = Branch,
                        Ifsc = Ifsc,
                        Balance = Balance
                    };
                    _db.DbBankAccounts.InsertOnSubmit(newBankAccount);
                }

                _db.SubmitChanges();

                var bankAccountList = from b in _db.DbBankAccounts select new { b.BankAccountId, b.AccountNo, b.AccountName, b.AccountType, b.BankName, b.Branch, b.Ifsc, b.Balance };

                return Json(new { success = true, BankAccountList = bankAccountList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteBankAccount(int BankAccountId)
        {
            try
            {
                var deleteBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                if (deleteBankAccount != null)
                {
                    _db.DbBankAccounts.DeleteOnSubmit(deleteBankAccount);
                    _db.SubmitChanges();

                    var bankAccountList = from b in _db.DbBankAccounts select new { b.BankAccountId, b.AccountNo, b.AccountName, b.AccountType, b.BankName, b.Branch, b.Ifsc, b.Balance };

                    return Json(new { success = true, BankAccountList = bankAccountList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new {success = false, Message = "Bank account not found"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))
                {
                    var ErrorCode = ((SqlException)ex).Number;

                    if (ErrorCode == 547)
                        return Json(new { success = false, Message = "Cannot delete this bank account as transaction exist" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult Deposit()
        {
            ViewBag.DepositList = from d in _db.DbDeposits join b in _db.DbBankAccounts on d.BankAccountId equals b.BankAccountId orderby d.DepositDate descending select new { d.DepositId, d.DepositDate, BankAccount = b.AccountName, d.PaymentMethod, d.Purpose, d.Amount, d.Note };

            var bankAccountList = (from b in _db.DbBankAccounts select new { BankAccountId = b.BankAccountId.ToString(), b.AccountName }).ToList();
            bankAccountList.Insert(0, new { BankAccountId = "", AccountName = "" });
            ViewBag.BankAccountList = new SelectList(bankAccountList, "BankAccountId", "AccountName");

            ViewBag.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");

            return View();
        }

        public JsonResult GetDeposit(int DepositId)
        {
            try
            {
                var deposit = (from d in _db.DbDeposits where d.DepositId == DepositId select new { DepositDate = d.DepositDate.ToString(), d.BankAccountId, d.PaymentMethod, d.Purpose, d.Amount, d.Note }).FirstOrDefault();

                if (deposit != null)
                    return Json(new { success = true, Deposit = deposit }, JsonRequestBehavior.AllowGet);

                return Json(new {success = false, Message = "Deposit not found"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveDeposit(int DepositId, DateTime DepositDate, int BankAccountId, string PaymentMethod, string Purpose, float Amount, string Note)
        {
            try
            {
                var updateDeposit = (from d in _db.DbDeposits where d.DepositId == DepositId select d).FirstOrDefault();

                if (updateDeposit != null)
                {
                    var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == updateDeposit.BankTransactionId select t).FirstOrDefault();

                    if (transaction != null)
                    {
                        transaction.BankAccountId = BankAccountId;
                        transaction.TransactionDate = DepositDate;
                        transaction.TransactionType = "Deposit";
                        transaction.Credit = Amount;
                        transaction.Description = Purpose;
                    }

                    if (updateDeposit.BankAccountId == BankAccountId)
                    {
                        var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == updateDeposit.BankAccountId select b).FirstOrDefault();

                        if (bankAccount != null)
                            bankAccount.Balance = bankAccount.Balance - float.Parse(updateDeposit.Amount.ToString()) + Amount;
                    }
                    else
                    {
                        var oldBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == updateDeposit.BankAccountId select b).FirstOrDefault();

                        if (oldBankAccount != null)
                            oldBankAccount.Balance = oldBankAccount.Balance - float.Parse(updateDeposit.Amount.ToString());

                        var newBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                        if (newBankAccount != null)
                            newBankAccount.Balance = newBankAccount.Balance + Amount;
                    }

                    updateDeposit.DepositDate = DepositDate;
                    updateDeposit.BankAccountId = BankAccountId;
                    updateDeposit.PaymentMethod = PaymentMethod;
                    updateDeposit.Purpose = Purpose;
                    updateDeposit.Amount = Amount;
                    updateDeposit.Note = Note;
                }
                else
                {
                    var transactionId = (from t in _db.DbBankTransactions orderby t.BankTransactionId descending select t.BankTransactionId).FirstOrDefault() + 1;

                    var bankTransaction = new DbBankTransaction
                    {
                        BankTransactionId = transactionId,
                        BankAccountId = BankAccountId,
                        TransactionDate = DepositDate,
                        TransactionType = "Deposit",
                        Credit = Amount,
                        Description = Purpose
                    };
                    _db.DbBankTransactions.InsertOnSubmit(bankTransaction);

                    var newDeposit = new DbDeposit
                    {
                        DepositId = (from b in _db.DbDeposits orderby b.DepositId descending select b.DepositId).FirstOrDefault() + 1,
                        DepositDate = DepositDate,
                        BankAccountId = BankAccountId,
                        PaymentMethod = PaymentMethod,
                        Purpose = Purpose,
                        Amount = Amount,
                        Note = Note,
                        BankTransactionId = transactionId
                    };
                    _db.DbDeposits.InsertOnSubmit(newDeposit);

                    var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                    if (bankAccount != null)
                        bankAccount.Balance += Amount;
                }

                _db.SubmitChanges();

                var depositList = from d in _db.DbDeposits join b in _db.DbBankAccounts on d.BankAccountId equals b.BankAccountId orderby d.DepositDate descending select new { d.DepositId, d.DepositDate, BankAccount = b.AccountName, d.PaymentMethod, d.Purpose, d.Amount, d.Note };

                return Json(new { success = true, DepositList = depositList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteDeposit(int DepositId)
        {
            try
            {
                var deleteDeposit = (from d in _db.DbDeposits where d.DepositId == DepositId select d).FirstOrDefault();

                if (deleteDeposit != null)
                {
                    _db.DbDeposits.DeleteOnSubmit(deleteDeposit);
                    _db.SubmitChanges();

                    var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == deleteDeposit.BankTransactionId select t).FirstOrDefault();

                    if (transaction != null)
                        _db.DbBankTransactions.DeleteOnSubmit(transaction);

                    var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == deleteDeposit.BankAccountId select b).FirstOrDefault();

                    if (bankAccount != null)
                        bankAccount.Balance -= float.Parse(deleteDeposit.Amount.ToString());

                    _db.SubmitChanges();

                    var depositList = from d in _db.DbDeposits join b in _db.DbBankAccounts on d.BankAccountId equals b.BankAccountId orderby d.DepositDate descending select new { d.DepositId, d.DepositDate, BankAccount = b.AccountName, d.PaymentMethod, d.Purpose, d.Amount, d.Note };

                    return Json(new { success = true, DepositList = depositList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new {success = false, Message = "Deposit not found"}, JsonRequestBehavior.AllowGet);
            }
                catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult History()
        {
            var bankAccountList = (from b in _db.DbBankAccounts select new { BankAccountId = b.BankAccountId.ToString(), b.AccountName }).ToList();
            bankAccountList.Insert(0, new { BankAccountId = "", AccountName = "Select Account" });
            ViewBag.BankAccountList = new SelectList(bankAccountList, "BankAccountId", "AccountName");

            ViewBag.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            return View();
        }

        public JsonResult GetHistory(DateTime FromDate, DateTime ToDate, int BankAccountId)
        {
            try
            {
                var history = (from t in _db.DbBankTransactions where t.TransactionDate >= FromDate && t.TransactionDate <= ToDate && t.BankAccountId == BankAccountId select new BankTransactionHistoryViewModel { TransactionDate = t.TransactionDate.ToString(), TransactionType = t.TransactionType, Description = t.Description, Credit = t.Credit == null ? "" : t.Credit.ToString(), Debit = t.Debit == null ? "" : t.Debit.ToString() }).ToList();

                return Json(new { success = true, History = history }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintHistory(DateTime FromDate, DateTime ToDate, int BankAccountId)
        {
            var model = new BankTransactionHistoryPrintViewModel();

            var company = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

            if (company != null)
            {
                model.CompanyLogo = company.LogoPath;
                model.CompanyName = company.CompanyName;
                model.CompanyAddress = company.Address;
                model.CompanyCity = company.City;
                model.CompanyPincode = (company.City != "" && company.Pincode != "") ? " - " + company.Pincode : company.Pincode;
                model.CompanyState = (company.City != "" || company.Pincode != "") ? ", " + company.State : company.State;
                model.CompanyMobile = (company.Mobile != "") ? "Ph: " + company.Mobile : "";
                model.CompanyLandline = (company.Mobile == "" && company.Landline != "") ? "Ph: " + company.Landline : (company.Mobile != "" && company.Landline != "") ? ", " + company.Landline : "";
                model.CompanyEmail = (company.Email != "") ? "Email: " + company.Email : "";
                model.CompanyWebsite = (company.Website != "") ? "Website: " + company.Website : "";
            }

            model.FromDate = FromDate.ToString("dd-MM-yyyy");
            model.ToDate = ToDate.ToString("dd-MM-yyyy");
            model.BankName = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b.AccountName).FirstOrDefault();
            model.TotalCredit = (from t in _db.DbBankTransactions where t.TransactionDate >= FromDate && t.TransactionDate <= ToDate && t.BankAccountId == BankAccountId && t.Debit == null select t.Credit).Sum().ToString();
            model.TotalDebit = (from t in _db.DbBankTransactions where t.TransactionDate >= FromDate && t.TransactionDate <= ToDate && t.BankAccountId == BankAccountId && t.Credit == null select t.Debit).Sum().ToString();

            model.BankTransactionHistoryViewModel = (from t in _db.DbBankTransactions where t.TransactionDate >= FromDate && t.TransactionDate <= ToDate && t.BankAccountId == BankAccountId select new BankTransactionHistoryViewModel { TransactionDate = t.TransactionDate.ToString(), TransactionType = t.TransactionType, Description = t.Description, Credit = t.Credit == null ? "" : t.Credit.ToString(), Debit = t.Debit == null ? "" : t.Debit.ToString() }).ToList();

            return View(model);
        }
    }
}