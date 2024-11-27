using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using System.Net.Mail;
using System.IO;
using SelectPdf;

namespace Invoice.Controllers
{
    public class HomeController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        public ActionResult BrowserNotSupported()
        {
            return View();
        }

        public ActionResult Login()
        {
            Session.RemoveAll();

            return View();
        }

        public JsonResult VerifyCredentials(string Password)
        {
            try
            {
                var company = (from c in _db.DbCompanies select c).FirstOrDefault();

                if (company != null)
                {
                    if (company.Password != Password)
                        return Json(new { success = false, Message = "Invalid PIN" });

                    Session["companyId"] = company.CompanyId;
                    Session["companyName"] = company.CompanyName;

                    if (string.IsNullOrEmpty(company.LogoPath))
                        Session["companyLogo"] = "../Logo/profile.png";
                    else
                        Session["companyLogo"] = company.LogoPath;

                    Session["companyState"] = company.State;
                }

                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, ex.Message });
            }
        }

        [CheckIfLoggedIn]
        public ActionResult UpdateProfile()
        {
            var getCompany = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

            ViewBag.StateList = new SelectList(_db.DbStates, "StateName", "StateName");

            return View(getCompany);
        }

        public JsonResult SaveCompanyInfo(string CompanyName, string Gstin, string Pan, string Mobile, string Landline, string Email, string Website, string Address, string Pincode, string City, string State)
        {
            try
            {
                var updateCompany = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                if (updateCompany != null)
                {
                    updateCompany.CompanyName = CompanyName;
                    updateCompany.Gstin = Gstin;
                    updateCompany.Pan = Pan;
                    updateCompany.Mobile = Mobile;
                    updateCompany.Landline = Landline;
                    updateCompany.Email = Email;
                    updateCompany.Website = Website;
                    updateCompany.Address = Address;
                    updateCompany.Pincode = Pincode;
                    updateCompany.City = City;
                    updateCompany.State = State;
                }

                _db.SubmitChanges();

                Session["companyName"] = CompanyName;

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveBankAccountInfo(string AccountNo, string AccountName, string AccountType, string BankName, string Ifsc, string Branch)
        {
            try
            {
                var updateCompany = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                if (updateCompany != null)
                {
                    updateCompany.AccountNo = AccountNo;
                    updateCompany.AccountName = AccountName;
                    updateCompany.AccountType = AccountType;
                    updateCompany.BankName = BankName;
                    updateCompany.Ifsc = Ifsc;
                    updateCompany.Branch = Branch;
                }

                _db.SubmitChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SavePassword(string Password)
        {
            try
            {
                var updateCompany = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                if (updateCompany != null)
                    updateCompany.Password = Password;

                _db.SubmitChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UploadLogo()
        {
            try
            {
                var files = Request.Files;
                var file = files[0];

                if (file != null)
                {
                    var logoExt = Path.GetExtension(file.FileName);
                    var logoModifiedName = "Logo" + logoExt;
                    var logoPath = Path.Combine(Server.MapPath("~/Logo"), logoModifiedName);
                    file.SaveAs(logoPath);

                    var updateCompany = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                    if (updateCompany != null)
                        updateCompany.LogoPath = "../Logo/" + logoModifiedName;

                    _db.SubmitChanges();

                    Session["companyLogo"] = "../Logo/" + logoModifiedName;
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveLogo()
        {
            try
            {
                var updateCompany = (from c in _db.DbCompanies where c.CompanyId == int.Parse(Session["companyId"].ToString()) select c).FirstOrDefault();

                if (updateCompany != null)
                    updateCompany.LogoPath = "";

                _db.SubmitChanges();

                Session["companyLogo"] = "../Logo/profile.png";

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult DownloadPdf(string Html, string Title)
        {
            try
            {
                Html = Uri.UnescapeDataString(Html);

                var htmlToPdf = new HtmlToPdf();
                htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
                htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

                htmlToPdf.Options.WebPageWidth = 848;
                htmlToPdf.Options.WebPageHeight = 0;
                htmlToPdf.Options.WebPageFixedSize = false;

                htmlToPdf.Options.AutoFitWidth = HtmlToPdfPageFitMode.ShrinkOnly;
                htmlToPdf.Options.AutoFitHeight = HtmlToPdfPageFitMode.NoAdjustment;

                htmlToPdf.Options.MarginLeft = 20;
                htmlToPdf.Options.MarginRight = 20;
                htmlToPdf.Options.MarginTop = 20;
                htmlToPdf.Options.MarginBottom = 20;

                PdfDocument doc = htmlToPdf.ConvertHtmlString(Html);
                var fileName = "/Pdf/" + Title + "_" + DateTime.Now.Date.ToString("dd_MM_yyyy_hh_mm_ss") + ".pdf";
                doc.Save(Server.MapPath("~" + fileName));
                doc.Close();

                return Json(new { success = true, FileName = ".." + fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}