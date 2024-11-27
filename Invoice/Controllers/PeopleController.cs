using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using System.Data.SqlClient;

namespace Invoice.Controllers
{
    public class PeopleController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Customer()
        {
            var customerList = from c in _db.DbCustomers select new { c.CustomerId, c.CustomerName, c.Gstin, c.Pan, c.Mobile, c.Landline, c.Email, c.Address, c.Pincode, c.City, c.State, c.Country };

            ViewBag.CustomerList = customerList;

            ViewBag.CountryList = new SelectList(_db.DbCountries, "CountryName", "CountryName");
            ViewBag.StateList = new SelectList(_db.DbStates, "StateName", "StateName");

            return View();
        }

        public JsonResult GetCustomer(int CustomerId)
        {
            try
            {
                var customer = (from c in _db.DbCustomers where c.CustomerId == CustomerId select new { c.CustomerId, c.CustomerName, c.Gstin, c.Pan, c.Mobile, c.Landline, c.Email, c.Address, c.Pincode, c.City, c.State, c.Country }).FirstOrDefault();

                if (customer != null)
                    return Json(new { success = true, Customer = customer }, JsonRequestBehavior.AllowGet);
                
                return Json(new { success = false, Message = "Customer not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCustomer(int CustomerId, string CustomerName, string Gstin, string Pan, string Mobile, string Landline, string Email, string Address, string Pincode, string City, string State, string Country)
        {
            try
            {
                var updateCustomer = (from c in _db.DbCustomers where c.CustomerId == CustomerId select c).FirstOrDefault();

                if (updateCustomer != null)
                {
                    updateCustomer.CustomerName = CustomerName;
                    updateCustomer.Gstin = Gstin;
                    updateCustomer.Pan = Pan;
                    updateCustomer.Mobile = Mobile;
                    updateCustomer.Landline = Landline;
                    updateCustomer.Email = Email;
                    updateCustomer.Address = Address;
                    updateCustomer.Pincode = Pincode;
                    updateCustomer.City = City;
                    updateCustomer.State = State;
                    updateCustomer.Country = Country;
                }
                else
                {
                    var newCustomer = new DbCustomer
                    {
                        CustomerId = (from c in _db.DbCustomers orderby c.CustomerId descending select c.CustomerId).FirstOrDefault() + 1,
                        CustomerName = CustomerName,
                        Gstin = Gstin,
                        Pan = Pan,
                        Mobile = Mobile,
                        Landline = Landline,
                        Email = Email,
                        Address = Address,
                        Pincode = Pincode,
                        City = City,
                        State = State,
                        Country = Country
                    };
                    _db.DbCustomers.InsertOnSubmit(newCustomer);
                }

                _db.SubmitChanges();

                var customerList = from c in _db.DbCustomers select new { c.CustomerId, c.CustomerName, c.Gstin, c.Pan, c.Mobile, c.Landline, c.Email, c.Address, c.Pincode, c.City, c.State, c.Country };

                return Json(new { success = true, CustomerList = customerList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteCustomer(int CustomerId)
        {
            try
            {
                var deleteCustomer = (from c in _db.DbCustomers where c.CustomerId == CustomerId select c).FirstOrDefault();

                if (deleteCustomer != null)
                {
                    _db.DbCustomers.DeleteOnSubmit(deleteCustomer);
                    _db.SubmitChanges();

                    var customerList = from c in _db.DbCustomers select new { c.CustomerId, c.CustomerName, c.Gstin, c.Pan, c.Mobile, c.Landline, c.Email, c.Address, c.Pincode, c.City, c.State, c.Country };

                    return Json(new { success = true, CustomerList = customerList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Customer not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))
                {
                    Int32 ErrorCode = ((SqlException)ex).Number;

                    if (ErrorCode == 547)
                        return Json(new { success = false, Message = "Cannot delete this customer as transaction exist" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult Supplier()
        {
            var supplierList = from s in _db.DbSuppliers select new { s.SupplierId, s.SupplierName, s.Gstin, s.Pan, s.Mobile, s.Landline, s.Email, s.Address, s.Pincode, s.City, s.State, s.Country };

            ViewBag.SupplierList = supplierList;

            ViewBag.CountryList = new SelectList(_db.DbCountries, "CountryName", "CountryName");
            ViewBag.StateList = new SelectList(_db.DbStates, "StateName", "StateName");

            return View();
        }

        public JsonResult GetSupplier(int SupplierId)
        {
            try
            {
                var supplier = (from s in _db.DbSuppliers where s.SupplierId == SupplierId select new { s.SupplierId, s.SupplierName, s.Gstin, s.Pan, s.Mobile, s.Landline, s.Email, s.Address, s.Pincode, s.City, s.State, s.Country }).FirstOrDefault();

                if (supplier != null)
                    return Json(new { success = true, Supplier = supplier }, JsonRequestBehavior.AllowGet);
                
                return Json(new { success = false, Message = "Supplier not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveSupplier(int SupplierId, string SupplierName, string Gstin, string Pan, string Mobile, string Landline, string Email, string Address, string Pincode, string City, string State, string Country)
        {
            try
            {
                var updateSupplier = (from s in _db.DbSuppliers where s.SupplierId == SupplierId select s).FirstOrDefault();

                if (updateSupplier != null)
                {
                    updateSupplier.SupplierName = SupplierName;
                    updateSupplier.Gstin = Gstin;
                    updateSupplier.Pan = Pan;
                    updateSupplier.Mobile = Mobile;
                    updateSupplier.Landline = Landline;
                    updateSupplier.Email = Email;
                    updateSupplier.Address = Address;
                    updateSupplier.Pincode = Pincode;
                    updateSupplier.City = City;
                    updateSupplier.State = State;
                    updateSupplier.Country = Country;
                }
                else
                {
                    var newSupplier = new DbSupplier
                    {
                        SupplierId = (from s in _db.DbSuppliers orderby s.SupplierId descending select s.SupplierId).FirstOrDefault() + 1,
                        SupplierName = SupplierName,
                        Gstin = Gstin,
                        Pan = Pan,
                        Mobile = Mobile,
                        Landline = Landline,
                        Email = Email,
                        Address = Address,
                        Pincode = Pincode,
                        City = City,
                        State = State,
                        Country = Country
                    };
                    _db.DbSuppliers.InsertOnSubmit(newSupplier);
                }

                _db.SubmitChanges();

                var supplierList = from s in _db.DbSuppliers select new { s.SupplierId, s.SupplierName, s.Gstin, s.Pan, s.Mobile, s.Landline, s.Email, s.Address, s.Pincode, s.City, s.State, s.Country };

                return Json(new { success = true, SupplierList = supplierList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteSupplier(int SupplierId)
        {
            try
            {
                var deleteSupplier = (from s in _db.DbSuppliers where s.SupplierId == SupplierId select s).FirstOrDefault();

                if (deleteSupplier != null)
                {
                    _db.DbSuppliers.DeleteOnSubmit(deleteSupplier);
                    _db.SubmitChanges();

                    var supplierList = from s in _db.DbSuppliers select new { s.SupplierId, s.SupplierName, s.Gstin, s.Pan, s.Mobile, s.Landline, s.Email, s.Address, s.Pincode, s.City, s.State, s.Country };

                    return Json(new { success = true, SupplierList = supplierList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Supplier not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))
                {
                    var ErrorCode = ((SqlException)ex).Number;

                    if (ErrorCode == 547)
                        return Json(new { success = false, Message = "Cannot delete this supplier as transaction exist" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}