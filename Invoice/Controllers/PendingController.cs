using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using Invoice.ViewModel;

namespace Invoice.Controllers
{
    public class PendingController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Payable()
        {
            var supplierList = (from s in _db.DbSuppliers select new { s.SupplierId, s.SupplierName }).ToList();
            supplierList.Insert(0, new { SupplierId = 0, SupplierName = "Select All" });
            ViewBag.SupplierList = new SelectList(supplierList, "SupplierId", "SupplierName");

            ViewBag.Pendings = from h in _db.DbPurchaseInvoiceHeaders
                               join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                               select new PendingViewModel
                               {
                                   InvoiceName = h.Prefix + h.InvoiceName,
                                   InvoiceDate = h.InvoiceDate.ToString(),
                                   SupplierName = s.SupplierName,
                                   Total = h.Total,
                                   Paid = h.Paid,
                                   Balance = h.Balance
                               };

            return View();
        }

        public JsonResult GetPayable(int SupplierId)
        {
            try
            {
                var pendings = new List<PendingViewModel>();

                if (SupplierId == 0)
                    pendings = (from h in _db.DbPurchaseInvoiceHeaders
                                join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                                select new PendingViewModel
                                {
                                    InvoiceName = h.Prefix + h.InvoiceName,
                                    InvoiceDate = h.InvoiceDate.ToString(),
                                    SupplierName = s.SupplierName,
                                    Total = h.Total,
                                    Paid = h.Paid,
                                    Balance = h.Balance
                                }).ToList();
                else
                    pendings = (from h in _db.DbPurchaseInvoiceHeaders
                                join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                                where h.SupplierId == SupplierId
                                select new PendingViewModel
                                {
                                    InvoiceName = h.Prefix + h.InvoiceName,
                                    InvoiceDate = h.InvoiceDate.ToString(),
                                    SupplierName = s.SupplierName,
                                    Total = h.Total,
                                    Paid = h.Paid,
                                    Balance = h.Balance
                                }).ToList();

                return Json(new { success = true, Pendings = pendings }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintPayable(int SupplierId)
        {
            var model = new PendingPrintViewModel();

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

            var pendings = new List<PendingViewModel>();

            if (SupplierId == 0)
                pendings = (from h in _db.DbPurchaseInvoiceHeaders
                            join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                            select new PendingViewModel
                            {
                                InvoiceName = h.Prefix + h.InvoiceName,
                                InvoiceDate = h.InvoiceDate.ToString(),
                                SupplierName = s.SupplierName,
                                Total = h.Total,
                                Paid = h.Paid,
                                Balance = h.Balance
                            }).ToList();
            else
                pendings = (from h in _db.DbPurchaseInvoiceHeaders
                            join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                            where h.SupplierId == SupplierId
                            select new PendingViewModel
                            {
                                InvoiceName = h.Prefix + h.InvoiceName,
                                InvoiceDate = h.InvoiceDate.ToString(),
                                SupplierName = s.SupplierName,
                                Total = h.Total,
                                Paid = h.Paid,
                                Balance = h.Balance
                            }).ToList();

            model.TotalInvoices = pendings.Count().ToString();
            model.Total = pendings.Sum(x => x.Total).ToString();
            model.Paid = pendings.Sum(x => x.Paid).ToString();
            model.Balance = pendings.Sum(x => x.Balance).ToString();

            model.PendingViewModel = pendings;

            return View(model);
        }

        [CheckIfLoggedIn]
        public ActionResult Receivable()
        {
            var customerList = (from c in _db.DbCustomers select new { c.CustomerId, c.CustomerName }).ToList();
            customerList.Insert(0, new { CustomerId = 0, CustomerName = "Select All" });
            ViewBag.CustomerList = new SelectList(customerList, "CustomerId", "CustomerName");

            ViewBag.Pendings = from h in _db.DbSalesInvoiceHeaders
                               join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                               select new PendingViewModel
                               {
                                   InvoiceName = h.Prefix + h.InvoiceName,
                                   InvoiceDate = h.InvoiceDate.ToString(),
                                   CustomerName = c.CustomerName,
                                   Total = h.Total,
                                   Paid = h.Paid,
                                   Balance = h.Balance
                               };

            return View();
        }

        public JsonResult GetReceivable(int CustomerId)
        {
            try
            {
                var pendings = new List<PendingViewModel>();

                if (CustomerId == 0)
                    pendings = (from h in _db.DbSalesInvoiceHeaders
                                join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                                select new PendingViewModel
                                {
                                    InvoiceName = h.Prefix + h.InvoiceName,
                                    InvoiceDate = h.InvoiceDate.ToString(),
                                    CustomerName = c.CustomerName,
                                    Total = h.Total,
                                    Paid = h.Paid,
                                    Balance = h.Balance
                                }).ToList();
                else
                    pendings = (from h in _db.DbSalesInvoiceHeaders
                                join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                                where h.CustomerId == CustomerId
                                select new PendingViewModel
                                {
                                    InvoiceName = h.Prefix + h.InvoiceName,
                                    InvoiceDate = h.InvoiceDate.ToString(),
                                    CustomerName = c.CustomerName,
                                    Total = h.Total,
                                    Paid = h.Paid,
                                    Balance = h.Balance
                                }).ToList();

                return Json(new { success = true, Pendings = pendings }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintReceivable(int CustomerId)
        {
            var model = new PendingPrintViewModel();

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

            var pendings = new List<PendingViewModel>();

            if (CustomerId == 0)
                pendings = (from h in _db.DbSalesInvoiceHeaders
                            join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                            select new PendingViewModel
                            {
                                InvoiceName = h.Prefix + h.InvoiceName,
                                InvoiceDate = h.InvoiceDate.ToString(),
                                CustomerName = c.CustomerName,
                                Total = h.Total,
                                Paid = h.Paid,
                                Balance = h.Balance
                            }).ToList();
            else
                pendings = (from h in _db.DbSalesInvoiceHeaders
                            join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                            where h.CustomerId == CustomerId
                            select new PendingViewModel
                            {
                                InvoiceName = h.Prefix + h.InvoiceName,
                                InvoiceDate = h.InvoiceDate.ToString(),
                                CustomerName = c.CustomerName,
                                Total = h.Total,
                                Paid = h.Paid,
                                Balance = h.Balance
                            }).ToList();

            model.TotalInvoices = pendings.Count().ToString();
            model.Total = pendings.Sum(x => x.Total).ToString();
            model.Paid = pendings.Sum(x => x.Paid).ToString();
            model.Balance = pendings.Sum(x => x.Balance).ToString();

            model.PendingViewModel = pendings;

            return View(model);
        }
    }
}
