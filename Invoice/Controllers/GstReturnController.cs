using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using Invoice.ViewModel;

namespace Invoice.Controllers
{
    public class GstReturnController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Gstr1()
        {
            var gstr = from h in _db.DbSalesInvoiceHeaders
                       join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                       where h.InvoiceDate.Month == DateTime.Now.Month && h.InvoiceDate.Year == DateTime.Now.Year
                       select new GstrViewModel
                       {
                           InvoiceName = h.Prefix + h.InvoiceName,
                           InvoiceDate = h.InvoiceDate.ToString(),
                           CustomerName = c.CustomerName,
                           Gstin = c.Gstin,
                           Mobile = c.Mobile,
                           TotalValue = h.TotalValue,
                           TotalDiscountRs = h.TotalDiscountRs,
                           TotalCgstAmount = h.TotalCgstAmount,
                           TotalSgstAmount = h.TotalSgstAmount,
                           TotalIgstAmount = h.TotalIgstAmount,
                           TotalTaxValue = h.TotalTaxValue,
                           Total = h.Total
                       };

            ViewBag.CurrentMonth = DateTime.Now.Month;
            ViewBag.CurrentYear = DateTime.Now.Year;
            ViewBag.Gstr = gstr;

            return View();
        }

        public JsonResult GetGstr1(int Month, int Year)
        {
            try
            {
                var gstr = from h in _db.DbSalesInvoiceHeaders
                           join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                           where h.InvoiceDate.Month == Month && h.InvoiceDate.Year == Year
                           select new GstrViewModel
                           {
                               InvoiceName = h.Prefix + h.InvoiceName,
                               InvoiceDate = h.InvoiceDate.ToString(),
                               CustomerName = c.CustomerName,
                               Gstin = c.Gstin,
                               Mobile = c.Mobile,
                               TotalValue = h.TotalValue,
                               TotalDiscountRs = h.TotalDiscountRs,
                               TotalCgstAmount = h.TotalCgstAmount,
                               TotalSgstAmount = h.TotalSgstAmount,
                               TotalIgstAmount = h.TotalIgstAmount,
                               TotalTaxValue = h.TotalTaxValue,
                               Total = h.Total
                           };

                return Json(new { success = true, Gstr = gstr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintGstr1(int Month, string MonthName, int Year)
        {
            var model = new GstrPrintViewModel();

            var gstr = (from h in _db.DbSalesInvoiceHeaders
                        join c in _db.DbCustomers on h.CustomerId equals c.CustomerId
                        where h.InvoiceDate.Month == Month && h.InvoiceDate.Year == Year
                        select new GstrViewModel
                        {
                            InvoiceName = h.Prefix + h.InvoiceName,
                            InvoiceDate = h.InvoiceDate.ToString(),
                            CustomerName = c.CustomerName,
                            Gstin = c.Gstin,
                            Mobile = c.Mobile,
                            TotalValue = h.TotalValue,
                            TotalDiscountRs = h.TotalDiscountRs,
                            TotalCgstAmount = h.TotalCgstAmount,
                            TotalSgstAmount = h.TotalSgstAmount,
                            TotalIgstAmount = h.TotalIgstAmount,
                            TotalTaxValue = h.TotalTaxValue,
                            Total = h.Total
                        }).ToList();

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

            model.TotalInvoices = gstr.Count();
            model.MonthName = MonthName;
            model.Year = Year;
            model.TotalValue = gstr.Sum(x => x.TotalValue);
            model.TotalDiscountRs = gstr.Sum(x => x.TotalDiscountRs);
            model.TotalCgstAmount = gstr.Sum(x => x.TotalCgstAmount);
            model.TotalSgstAmount = gstr.Sum(x => x.TotalSgstAmount);
            model.TotalIgstAmount = gstr.Sum(x => x.TotalIgstAmount);
            model.TotalTaxValue = gstr.Sum(x => x.TotalTaxValue);
            model.Total = gstr.Sum(x => x.Total);

            model.GstrViewModel = gstr;

            return View(model);
        }

        [CheckIfLoggedIn]
        public ActionResult Gstr2()
        {
            var gstr = from h in _db.DbPurchaseInvoiceHeaders
                       join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                       where h.InvoiceDate.Month == DateTime.Now.Month && h.InvoiceDate.Year == DateTime.Now.Year
                       select new GstrViewModel
                       {
                           InvoiceName = h.Prefix + h.InvoiceName,
                           InvoiceDate = h.InvoiceDate.ToString(),
                           SupplierName = s.SupplierName,
                           Gstin = s.Gstin,
                           Mobile = s.Mobile,
                           TotalValue = h.TotalValue,
                           TotalDiscountRs = h.TotalDiscountRs,
                           TotalCgstAmount = h.TotalCgstAmount,
                           TotalSgstAmount = h.TotalSgstAmount,
                           TotalIgstAmount = h.TotalIgstAmount,
                           TotalTaxValue = h.TotalTaxValue,
                           Total = h.Total
                       };

            ViewBag.CurrentMonth = DateTime.Now.Month;
            ViewBag.CurrentYear = DateTime.Now.Year;
            ViewBag.Gstr = gstr;

            return View();
        }

        public JsonResult GetGstr2(int Month, int Year)
        {
            try
            {
                var gstr = from h in _db.DbPurchaseInvoiceHeaders
                           join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                           where h.InvoiceDate.Month == Month && h.InvoiceDate.Year == Year
                           select new GstrViewModel
                           {
                               InvoiceName = h.Prefix + h.InvoiceName,
                               InvoiceDate = h.InvoiceDate.ToString(),
                               SupplierName = s.SupplierName,
                               Gstin = s.Gstin,
                               Mobile = s.Mobile,
                               TotalValue = h.TotalValue,
                               TotalDiscountRs = h.TotalDiscountRs,
                               TotalCgstAmount = h.TotalCgstAmount,
                               TotalSgstAmount = h.TotalSgstAmount,
                               TotalIgstAmount = h.TotalIgstAmount,
                               TotalTaxValue = h.TotalTaxValue,
                               Total = h.Total
                           };

                return Json(new { success = true, Gstr = gstr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintGstr2(int Month, string MonthName, int Year)
        {
            var model = new GstrPrintViewModel();

            var gstr = (from h in _db.DbPurchaseInvoiceHeaders
                        join s in _db.DbSuppliers on h.SupplierId equals s.SupplierId
                        where h.InvoiceDate.Month == Month && h.InvoiceDate.Year == Year
                        select new GstrViewModel
                        {
                            InvoiceName = h.Prefix + h.InvoiceName,
                            InvoiceDate = h.InvoiceDate.ToString(),
                            SupplierName = s.SupplierName,
                            Gstin = s.Gstin,
                            Mobile = s.Mobile,
                            TotalValue = h.TotalValue,
                            TotalDiscountRs = h.TotalDiscountRs,
                            TotalCgstAmount = h.TotalCgstAmount,
                            TotalSgstAmount = h.TotalSgstAmount,
                            TotalIgstAmount = h.TotalIgstAmount,
                            TotalTaxValue = h.TotalTaxValue,
                            Total = h.Total
                        }).ToList();

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

            model.TotalInvoices = gstr.Count();
            model.MonthName = MonthName;
            model.Year = Year;
            model.TotalValue = gstr.Sum(x => x.TotalValue);
            model.TotalDiscountRs = gstr.Sum(x => x.TotalDiscountRs);
            model.TotalCgstAmount = gstr.Sum(x => x.TotalCgstAmount);
            model.TotalSgstAmount = gstr.Sum(x => x.TotalSgstAmount);
            model.TotalIgstAmount = gstr.Sum(x => x.TotalIgstAmount);
            model.TotalTaxValue = gstr.Sum(x => x.TotalTaxValue);
            model.Total = gstr.Sum(x => x.Total);

            model.GstrViewModel = gstr;

            return View(model);
        }
    }
}
