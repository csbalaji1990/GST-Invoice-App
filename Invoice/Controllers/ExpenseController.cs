using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;

namespace Invoice.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Index()
        {
            ViewBag.ExpenseList = from e in _db.DbExpenses join b in _db.DbBankAccounts on e.BankAccountId equals b.BankAccountId orderby e.ExpenseDate descending select new { e.ExpenseId, e.ExpenseDate, BankAccount = b.AccountName, e.PaymentMethod, e.Purpose, e.Amount, e.Note };

            var bankAccountList = (from b in _db.DbBankAccounts select new { BankAccountId = b.BankAccountId.ToString(), b.AccountName }).ToList();
            bankAccountList.Insert(0, new { BankAccountId = "", AccountName = "" });
            ViewBag.BankAccountList = new SelectList(bankAccountList, "BankAccountId", "AccountName");

            ViewBag.PaymentMethodList = new SelectList(_db.DbPaymentMethods, "PaymentMethodName", "PaymentMethodName");

            return View();
        }

        public JsonResult GetExpense(int ExpenseId)
        {
            try
            {
                var expense = (from e in _db.DbExpenses where e.ExpenseId == ExpenseId select new { ExpenseDate = e.ExpenseDate.ToString(), e.BankAccountId, e.PaymentMethod, e.Purpose, e.Amount, e.Note }).FirstOrDefault();

                if (expense != null)
                    return Json(new { success = true, Expense = expense }, JsonRequestBehavior.AllowGet);

                return Json(new {success = false, Message = "Expense not found"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveExpense(int ExpenseId, DateTime ExpenseDate, int BankAccountId, string PaymentMethod, string Purpose, float Amount, string Note)
        {
            try
            {
                var updateExpense = (from e in _db.DbExpenses where e.ExpenseId == ExpenseId select e).FirstOrDefault();

                if (updateExpense != null)
                {
                    var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == updateExpense.BankTransactionId select t).FirstOrDefault();

                    if (transaction != null)
                    {
                        transaction.BankAccountId = BankAccountId;
                        transaction.TransactionDate = ExpenseDate;
                        transaction.TransactionType = "Expense";
                        transaction.Debit = Amount;
                        transaction.Description = Purpose;
                    }

                    if (updateExpense.BankAccountId == BankAccountId)
                    {
                        var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == updateExpense.BankAccountId select b).FirstOrDefault();

                        if (bankAccount != null)
                            bankAccount.Balance = bankAccount.Balance + float.Parse(updateExpense.Amount.ToString()) - Amount;
                    }
                    else
                    {
                        var oldBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == updateExpense.BankAccountId select b).FirstOrDefault();

                        if (oldBankAccount != null)
                            oldBankAccount.Balance = oldBankAccount.Balance + float.Parse(updateExpense.Amount.ToString());

                        var newBankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                        if (newBankAccount != null)
                            newBankAccount.Balance = newBankAccount.Balance - Amount;
                    }

                    updateExpense.ExpenseDate = ExpenseDate;
                    updateExpense.BankAccountId = BankAccountId;
                    updateExpense.PaymentMethod = PaymentMethod;
                    updateExpense.Purpose = Purpose;
                    updateExpense.Amount = Amount;
                    updateExpense.Note = Note;
                }
                else
                {
                    var transactionId = (from t in _db.DbBankTransactions orderby t.BankTransactionId descending select t.BankTransactionId).FirstOrDefault() + 1;

                    var bankTransaction = new DbBankTransaction
                    {
                        BankTransactionId = transactionId,
                        BankAccountId = BankAccountId,
                        TransactionDate = ExpenseDate,
                        TransactionType = "Expense",
                        Debit = Amount,
                        Description = Purpose
                    };
                    _db.DbBankTransactions.InsertOnSubmit(bankTransaction);

                    var newExpense = new DbExpense
                    {
                        ExpenseId = (from e in _db.DbExpenses orderby e.ExpenseId descending select e.ExpenseId).FirstOrDefault() + 1,
                        ExpenseDate = ExpenseDate,
                        BankAccountId = BankAccountId,
                        PaymentMethod = PaymentMethod,
                        Purpose = Purpose,
                        Amount = Amount,
                        Note = Note,
                        BankTransactionId = transactionId
                    };
                    _db.DbExpenses.InsertOnSubmit(newExpense);

                    var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == BankAccountId select b).FirstOrDefault();

                    if (bankAccount != null)
                        bankAccount.Balance -= Amount;
                }

                _db.SubmitChanges();

                var expenseList = from e in _db.DbExpenses join b in _db.DbBankAccounts on e.BankAccountId equals b.BankAccountId orderby e.ExpenseDate descending select new { e.ExpenseId, e.ExpenseDate, BankAccount = b.AccountName, e.PaymentMethod, e.Purpose, e.Amount, e.Note };

                return Json(new { success = true, ExpenseList = expenseList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteExpense(int ExpenseId)
        {
            try
            {
                var deleteExpense = (from e in _db.DbExpenses where e.ExpenseId == ExpenseId select e).FirstOrDefault();

                if (deleteExpense != null)
                {
                    _db.DbExpenses.DeleteOnSubmit(deleteExpense);
                    _db.SubmitChanges();

                    var transaction = (from t in _db.DbBankTransactions where t.BankTransactionId == deleteExpense.BankTransactionId select t).FirstOrDefault();

                    if (transaction != null)
                        _db.DbBankTransactions.DeleteOnSubmit(transaction);

                    var bankAccount = (from b in _db.DbBankAccounts where b.BankAccountId == deleteExpense.BankAccountId select b).FirstOrDefault();

                    if (bankAccount != null)
                        bankAccount.Balance += float.Parse(deleteExpense.Amount.ToString());

                    _db.SubmitChanges();

                    var expenseList = from e in _db.DbExpenses join b in _db.DbBankAccounts on e.BankAccountId equals b.BankAccountId orderby e.ExpenseDate descending select new { e.ExpenseId, e.ExpenseDate, BankAccount = b.AccountName, e.PaymentMethod, e.Purpose, e.Amount, e.Note };

                    return Json(new { success = true, DepositList = expenseList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new {success = false, Message = "Expense not found"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}