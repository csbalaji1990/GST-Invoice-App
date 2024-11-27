using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using Invoice.ViewModel;

namespace Invoice.Controllers
{
    public class LedgerController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Supplier()
        {
            var supplierList = (from s in _db.DbSuppliers select new { SupplierId = s.SupplierId.ToString(), s.SupplierName }).ToList();
            supplierList.Insert(0, new { SupplierId = "", SupplierName = "Select Supplier" });
            ViewBag.SupplierList = new SelectList(supplierList, "SupplierId", "SupplierName");

            ViewBag.CurrentYear = DateTime.Now.Year;

            return View();
        }

        public JsonResult GetSupplierLedger(int SupplierId, int Year)
        {
            try
            {
                float? openingBalance = 0, totalCredit = 0, totalDebit = 0, balance = 0;

                var openingBalanceCredit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year < Year select l.Credit).ToList();
                var openingBalanceDebit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year < Year select l.Debit).ToList();

                float? openingBalanceCreditSum = 0, openingBalanceDebitSum = 0;

                if (openingBalanceCredit.Count() > 0)
                    openingBalanceCreditSum = openingBalanceCredit.Sum();

                if (openingBalanceDebit.Count() > 0)
                    openingBalanceDebitSum = openingBalanceDebit.Sum();

                openingBalance = openingBalanceCreditSum - openingBalanceDebitSum;

                var closingBalanceCredit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year == Year select l.Credit).ToList();
                var closingBalanceDebit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year == Year select l.Debit).ToList();

                float? closingBalanceCreditSum = 0, closingBalanceDebitSum = 0;

                if (closingBalanceCredit.Count() > 0)
                    closingBalanceCreditSum = closingBalanceCredit.Sum();

                if (closingBalanceDebit.Count() > 0)
                    closingBalanceDebitSum = closingBalanceDebit.Sum();

                totalCredit = closingBalanceCreditSum + openingBalance;
                totalDebit = closingBalanceDebitSum;
                balance = totalCredit - totalDebit;

                var ledger = (from l in _db.DbSupplierLedgers
                             where l.SupplierId == SupplierId && l.PaymentDate.Year == Year
                              select new LedgerViewModel
                             {
                                 PaymentDate = l.PaymentDate.ToString(),
                                 Description = l.Description,
                                 Credit = l.Credit == null ? "" : l.Credit.ToString(),
                                 Debit = l.Debit == null ? "" : l.Debit.ToString()
                             }).ToList();

                ledger.Insert(0, new LedgerViewModel { PaymentDate = "", Description = "Opening Balance", Credit = openingBalance.ToString(), Debit = "" });
                ledger.Insert(ledger.Count(), new LedgerViewModel { PaymentDate = "", Description = "Closing Balance", Credit = balance.ToString(), Debit = "" });

                return Json(new { success = true, Ledger = ledger }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintSupplier(int SupplierId, int Year)
        {
            float? openingBalance = 0, totalCredit = 0, totalDebit = 0, balance = 0;

            var openingBalanceCredit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year < Year select l.Credit).ToList();
            var openingBalanceDebit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year < Year select l.Debit).ToList();

            float? openingBalanceCreditSum = 0, openingBalanceDebitSum = 0;

            if (openingBalanceCredit.Count() > 0)
                openingBalanceCreditSum = openingBalanceCredit.Sum();

            if (openingBalanceDebit.Count() > 0)
                openingBalanceDebitSum = openingBalanceDebit.Sum();

            openingBalance = openingBalanceCreditSum - openingBalanceDebitSum;

            var closingBalanceCredit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year == Year select l.Credit).ToList();
            var closingBalanceDebit = (from l in _db.DbSupplierLedgers where l.SupplierId == SupplierId && l.PaymentDate.Year == Year select l.Debit).ToList();

            float? closingBalanceCreditSum = 0, closingBalanceDebitSum = 0;

            if (closingBalanceCredit.Count() > 0)
                closingBalanceCreditSum = closingBalanceCredit.Sum();

            if (closingBalanceDebit.Count() > 0)
                closingBalanceDebitSum = closingBalanceDebit.Sum();

            totalCredit = closingBalanceCreditSum + openingBalance;
            totalDebit = closingBalanceDebitSum;
            balance = totalCredit - totalDebit;

            var ledger = (from l in _db.DbSupplierLedgers
                          where l.SupplierId == SupplierId && l.PaymentDate.Year == Year
                          select new LedgerViewModel
                          {
                              PaymentDate = l.PaymentDate.ToString(),
                              Description = l.Description,
                              Credit = l.Credit == null ? "" : l.Credit.ToString(),
                              Debit = l.Debit == null ? "" : l.Debit.ToString()
                          }).ToList();

            ledger.Insert(0, new LedgerViewModel { PaymentDate = "", Description = "Opening Balance", Credit = openingBalance.ToString(), Debit = "" });
            ledger.Insert(ledger.Count(), new LedgerViewModel { PaymentDate = "", Description = "Closing Balance", Credit = balance.ToString(), Debit = "" });

            var model = new LedgerPrintViewModel();

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

            var supplier = (from s in _db.DbSuppliers where s.SupplierId == SupplierId select s).FirstOrDefault();

            if (supplier != null)
            {
                model.SupplierName = supplier.SupplierName;
                model.SupplierAddress = supplier.Address;
                model.SupplierCity = supplier.City;
                model.SupplierState = supplier.State;
                model.SupplierPincode = (supplier.City != "" && supplier.Pincode != "") ? " - " + supplier.Pincode : supplier.Pincode;
                model.SupplierEmail = supplier.Email;
            }

            model.FromDate = "01/01/" + Year;
            model.ToDate = "31/12/" + Year;
            model.OpeningBalance = openingBalance.ToString();
            model.TotalCredit = closingBalanceCreditSum.ToString();
            model.TotalDebit = totalDebit.ToString();
            model.Balance = balance.ToString();

            model.LedgerViewModel = ledger;

            return View(model);
        }

        [CheckIfLoggedIn]
        public ActionResult Customer()
        {
            var customerList = (from c in _db.DbCustomers select new { CustomerId = c.CustomerId.ToString(), c.CustomerName }).ToList();
            customerList.Insert(0, new { CustomerId = "", CustomerName = "Select Customer" });
            ViewBag.CustomerList = new SelectList(customerList, "CustomerId", "CustomerName");

            ViewBag.CurrentYear = DateTime.Now.Year;

            return View();
        }

        public JsonResult GetCustomerLedger(int CustomerId, int Year)
        {
            try
            {
                float? openingBalance = 0, totalCredit = 0, totalDebit = 0, balance = 0;

                var openingBalanceCredit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year < Year select l.Credit).ToList();
                var openingBalanceDebit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year < Year select l.Debit).ToList();

                float? openingBalanceCreditSum = 0, openingBalanceDebitSum = 0;

                if (openingBalanceCredit.Count() > 0)
                    openingBalanceCreditSum = openingBalanceCredit.Sum();

                if (openingBalanceDebit.Count() > 0)
                    openingBalanceDebitSum = openingBalanceDebit.Sum();

                openingBalance = openingBalanceDebitSum - openingBalanceCreditSum;

                var closingBalanceCredit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year == Year select l.Credit).ToList();
                var closingBalanceDebit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year == Year select l.Debit).ToList();

                float? closingBalanceCreditSum = 0, closingBalanceDebitSum = 0;

                if (closingBalanceCredit.Count() > 0)
                    closingBalanceCreditSum = closingBalanceCredit.Sum();

                if (closingBalanceDebit.Count() > 0)
                    closingBalanceDebitSum = closingBalanceDebit.Sum();

                totalCredit = closingBalanceCreditSum;
                totalDebit = closingBalanceDebitSum + openingBalance;
                balance = totalDebit - totalCredit;

                var ledger = (from l in _db.DbCustomerLedgers
                              where l.CustomerId == CustomerId && l.PaymentDate.Year == Year
                              select new LedgerViewModel
                              {
                                  PaymentDate = l.PaymentDate.ToString(),
                                  Description = l.Description,
                                  Credit = l.Credit == null ? "" : l.Credit.ToString(),
                                  Debit = l.Debit == null ? "" : l.Debit.ToString()
                              }).ToList();

                ledger.Insert(0, new LedgerViewModel { PaymentDate = "", Description = "Opening Balance", Credit = "", Debit = openingBalance.ToString() });
                ledger.Insert(ledger.Count(), new LedgerViewModel { PaymentDate = "", Description = "Closing Balance", Credit = "", Debit = balance.ToString() });

                return Json(new { success = true, Ledger = ledger }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintCustomer(int CustomerId, int Year)
        {
            float? openingBalance = 0, totalCredit = 0, totalDebit = 0, balance = 0;

            var openingBalanceCredit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year < Year select l.Credit).ToList();
            var openingBalanceDebit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year < Year select l.Debit).ToList();

            float? openingBalanceCreditSum = 0, openingBalanceDebitSum = 0;

            if (openingBalanceCredit.Count() > 0)
                openingBalanceCreditSum = openingBalanceCredit.Sum();

            if (openingBalanceDebit.Count() > 0)
                openingBalanceDebitSum = openingBalanceDebit.Sum();

            openingBalance = openingBalanceDebitSum - openingBalanceCreditSum;

            var closingBalanceCredit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year == Year select l.Credit).ToList();
            var closingBalanceDebit = (from l in _db.DbCustomerLedgers where l.CustomerId == CustomerId && l.PaymentDate.Year == Year select l.Debit).ToList();

            float? closingBalanceCreditSum = 0, closingBalanceDebitSum = 0;

            if (closingBalanceCredit.Count() > 0)
                closingBalanceCreditSum = closingBalanceCredit.Sum();

            if (closingBalanceDebit.Count() > 0)
                closingBalanceDebitSum = closingBalanceDebit.Sum();

            totalCredit = closingBalanceCreditSum;
            totalDebit = closingBalanceDebitSum + openingBalance;
            balance = totalDebit - totalCredit;

            var ledger = (from l in _db.DbCustomerLedgers
                          where l.CustomerId == CustomerId && l.PaymentDate.Year == Year
                          select new LedgerViewModel
                          {
                              PaymentDate = l.PaymentDate.ToString(),
                              Description = l.Description,
                              Credit = l.Credit == null ? "" : l.Credit.ToString(),
                              Debit = l.Debit == null ? "" : l.Debit.ToString()
                          }).ToList();

            ledger.Insert(0, new LedgerViewModel { PaymentDate = "", Description = "Opening Balance", Credit = "", Debit = openingBalance.ToString() });
            ledger.Insert(ledger.Count(), new LedgerViewModel { PaymentDate = "", Description = "Closing Balance", Credit = "", Debit = balance.ToString() });

            var model = new LedgerPrintViewModel();

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

            var customer = (from c in _db.DbCustomers where c.CustomerId == CustomerId select c).FirstOrDefault();

            if (customer != null)
            {
                model.CustomerName = customer.CustomerName;
                model.CustomerAddress = customer.Address;
                model.CustomerCity = customer.City;
                model.CustomerState = customer.State;
                model.CustomerPincode = (customer.City != "" && customer.Pincode != "") ? " - " + customer.Pincode : customer.Pincode;
                model.CustomerEmail = customer.Email;
            }

            model.FromDate = "01/01/" + Year;
            model.ToDate = "31/12/" + Year;
            model.OpeningBalance = openingBalance.ToString();
            model.TotalCredit = totalCredit.ToString();
            model.TotalDebit = closingBalanceDebitSum.ToString();
            model.Balance = balance.ToString();

            model.LedgerViewModel = ledger;

            return View(model);
        }
    }
}
