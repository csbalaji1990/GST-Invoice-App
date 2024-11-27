using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using System.Data.SqlClient;
using OfficeOpenXml;
using Invoice.ViewModel;

namespace Invoice.Controllers
{
    public class ItemController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Item()
        {
            var itemList = from i in _db.DbItems join u in _db.DbUnits on i.UnitId equals u.UnitId join c in _db.DbCategories on i.CategoryId equals c.CategoryId select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, i.TaxRate, i.Cess, i.PurchasePrice, PurchasePriceInclusiveTax = i.PurchasePriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", i.SalesPrice, SalesPriceInclusiveTax = i.SalesPriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", Unit = u.UnitName, Category = c.CategoryName, i.StockQuantity, i.AlertQuantity };

            ViewBag.ItemList = itemList;

            ViewBag.TaxList = new SelectList(_db.DbTaxes, "TaxRate", "TaxName");
            ViewBag.CategoryList = new SelectList(_db.DbCategories.OrderBy(x => x.CategoryName), "CategoryId", "CategoryName");
            ViewBag.UnitList = new SelectList(_db.DbUnits.OrderBy(x => x.UnitName), "UnitId", "UnitName");

            return View();
        }

        public JsonResult GetItem(string ItemCode)
        {
            try
            {
                var item = (from i in _db.DbItems where i.ItemCode == ItemCode select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, i.TaxRate, i.Cess, i.PurchasePrice, i.PurchasePriceInclusiveTax, i.SalesPrice, i.SalesPriceInclusiveTax, i.CategoryId, i.UnitId, i.StockQuantity, i.AlertQuantity }).FirstOrDefault();

                if (item != null)
                    return Json(new { success = true, Item = item }, JsonRequestBehavior.AllowGet);

                return Json(new { success = false, Message = "Item not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveItem(string Action, string ItemCode, string ItemName, string ItemDescription, string HsnSac, float TaxRate, float Cess, float PurchasePrice, bool PurchasePriceInclusiveTax, float SalesPrice, bool SalesPriceInclusiveTax, int CategoryId, int UnitId, float StockQuantity, float AlertQuantity)
        {
            try
            {
                var updateItem = (from i in _db.DbItems where i.ItemCode == ItemCode select i).FirstOrDefault();

                if (updateItem != null)
                {
                    if (Action == "Add")
                        return Json(new { success = false, Message = "Item already exist" }, JsonRequestBehavior.AllowGet);

                    updateItem.ItemName = ItemName;
                    updateItem.ItemDescription = ItemDescription;
                    updateItem.HsnSac = HsnSac;
                    updateItem.TaxRate = TaxRate;
                    updateItem.Cess = Cess;
                    updateItem.PurchasePrice = PurchasePrice;
                    updateItem.PurchasePriceInclusiveTax = PurchasePriceInclusiveTax;
                    updateItem.SalesPrice = SalesPrice;
                    updateItem.SalesPriceInclusiveTax = SalesPriceInclusiveTax;
                    updateItem.CategoryId = CategoryId;
                    updateItem.UnitId = UnitId;
                    updateItem.StockQuantity = StockQuantity;
                    updateItem.AlertQuantity = AlertQuantity;
                }
                else
                {
                    var newItem = new DbItem
                    {
                        ItemCode = ItemCode,
                        ItemName = ItemName,
                        ItemDescription = ItemDescription,
                        HsnSac = HsnSac,
                        TaxRate = TaxRate,
                        Cess = Cess,
                        PurchasePrice = PurchasePrice,
                        PurchasePriceInclusiveTax = PurchasePriceInclusiveTax,
                        SalesPrice = SalesPrice,
                        SalesPriceInclusiveTax = SalesPriceInclusiveTax,
                        CategoryId = CategoryId,
                        UnitId = UnitId,
                        StockQuantity = StockQuantity,
                        AlertQuantity = AlertQuantity
                    };
                    _db.DbItems.InsertOnSubmit(newItem);
                }

                _db.SubmitChanges();

                var itemList = from i in _db.DbItems join u in _db.DbUnits on i.UnitId equals u.UnitId join c in _db.DbCategories on i.CategoryId equals c.CategoryId select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, i.TaxRate, i.Cess, i.PurchasePrice, PurchasePriceInclusiveTax = i.PurchasePriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", i.SalesPrice, SalesPriceInclusiveTax = i.SalesPriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", Unit = u.UnitName, Category = c.CategoryName, i.StockQuantity, i.AlertQuantity };

                return Json(new { success = true, ItemList = itemList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteItem(string ItemCode)
        {
            try
            {
                var deleteItem = (from i in _db.DbItems where i.ItemCode == ItemCode select i).FirstOrDefault();

                if (deleteItem != null)
                {
                    _db.DbItems.DeleteOnSubmit(deleteItem);
                    _db.SubmitChanges();

                    var itemList = from i in _db.DbItems join u in _db.DbUnits on i.UnitId equals u.UnitId join c in _db.DbCategories on i.CategoryId equals c.CategoryId select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, i.TaxRate, i.Cess, i.PurchasePrice, PurchasePriceInclusiveTax = i.PurchasePriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", i.SalesPrice, SalesPriceInclusiveTax = i.SalesPriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", Unit = u.UnitName, Category = c.CategoryName, i.StockQuantity, i.AlertQuantity };

                    return Json(new { success = true, ItemList = itemList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Item not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))
                {
                    var ErrorCode = ((SqlException)ex).Number;

                    if (ErrorCode == 547)
                        return Json(new { success = false, Message = "Cannot delete this item as transaction exist" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ImportItems()
        {
            try
            {
                if ((Request.Files != null) && (Request.Files.Count > 0))
                {
                    using (var package = new ExcelPackage(Request.InputStream))
                    {
                        var workSheet = package.Workbook.Worksheets.First();

                        for (var i = 1; i <= workSheet.Dimension.End.Row - 1; i++)
                        {
                            int categoryId, unitId;
                            string itemCode, itemName, itemDescription, hsnSac, categoryName, unitName;
                            float taxRate, cess, purchasePrice, salesPrice, stockQuantity, alertQuantity;
                            bool purchasePriceInclusiveTax, salesPriceInclusiveTax;

                            if ((workSheet.Cells[i + 1, 1].Value) == null) itemCode = ""; else itemCode = (workSheet.Cells[i + 1, 1].Value).ToString();
                            if ((workSheet.Cells[i + 1, 2].Value) == null) itemName = ""; else itemName = (workSheet.Cells[i + 1, 2].Value).ToString();
                            if ((workSheet.Cells[i + 1, 3].Value) == null) itemDescription = ""; else itemDescription = (workSheet.Cells[i + 1, 3].Value).ToString();
                            if ((workSheet.Cells[i + 1, 4].Value) == null) hsnSac = ""; else hsnSac = (workSheet.Cells[i + 1, 4].Value).ToString();
                            taxRate = float.Parse((workSheet.Cells[i + 1, 5].Value).ToString());
                            cess = float.Parse((workSheet.Cells[i + 1, 6].Value).ToString());
                            purchasePrice = float.Parse((workSheet.Cells[i + 1, 7].Value).ToString());
                            purchasePriceInclusiveTax = bool.Parse((workSheet.Cells[i + 1, 8].Value).ToString());
                            salesPrice = float.Parse((workSheet.Cells[i + 1, 9].Value).ToString());
                            salesPriceInclusiveTax = bool.Parse((workSheet.Cells[i + 1, 10].Value).ToString());
                            categoryName = (workSheet.Cells[i + 1, 11].Value).ToString();
                            unitName = (workSheet.Cells[i + 1, 12].Value).ToString();
                            stockQuantity = float.Parse((workSheet.Cells[i + 1, 13].Value).ToString());
                            alertQuantity = float.Parse((workSheet.Cells[i + 1, 14].Value).ToString());

                            var category = (from c in _db.DbCategories where c.CategoryName == categoryName select c).FirstOrDefault();

                            if (category != null)
                            {
                                categoryId = category.CategoryId;
                            }
                            else
                            {
                                categoryId = (from c in _db.DbCategories orderby c.CategoryId descending select c.CategoryId).FirstOrDefault() + 1;

                                var newCategory = new DbCategory
                                {
                                    CategoryId = categoryId,
                                    CategoryName = categoryName
                                };
                                _db.DbCategories.InsertOnSubmit(newCategory);
                                _db.SubmitChanges();
                            }

                            var unit = (from u in _db.DbUnits where u.UnitName == unitName select u).FirstOrDefault();

                            if (unit != null)
                            {
                                unitId = unit.UnitId;
                            }
                            else
                            {
                                unitId = (from u in _db.DbUnits orderby u.UnitId descending select u.UnitId).FirstOrDefault() + 1;

                                var newUnit = new DbUnit
                                {
                                    UnitId = unitId,
                                    UnitName = unitName
                                };
                                _db.DbUnits.InsertOnSubmit(newUnit);
                                _db.SubmitChanges();
                            }

                            if (!string.IsNullOrEmpty(itemCode))
                            {
                                var updateItem = (from item in _db.DbItems where item.ItemCode == itemCode select item).FirstOrDefault();

                                if (updateItem != null)
                                {
                                    updateItem.ItemName = itemName;
                                    updateItem.ItemDescription = itemDescription;
                                    updateItem.HsnSac = hsnSac;
                                    updateItem.TaxRate = taxRate;
                                    updateItem.Cess = cess;
                                    updateItem.PurchasePrice = purchasePrice;
                                    updateItem.PurchasePriceInclusiveTax = purchasePriceInclusiveTax;
                                    updateItem.SalesPrice = salesPrice;
                                    updateItem.SalesPriceInclusiveTax = salesPriceInclusiveTax;
                                    updateItem.CategoryId = categoryId;
                                    updateItem.UnitId = unitId;
                                    updateItem.StockQuantity = stockQuantity;
                                    updateItem.AlertQuantity = alertQuantity;
                                }
                                else
                                {
                                    var newItem = new DbItem
                                    {
                                        ItemCode = itemCode,
                                        ItemName = itemName,
                                        ItemDescription = itemDescription,
                                        HsnSac = hsnSac,
                                        TaxRate = taxRate,
                                        Cess = cess,
                                        PurchasePrice = purchasePrice,
                                        PurchasePriceInclusiveTax = purchasePriceInclusiveTax,
                                        SalesPrice = salesPrice,
                                        SalesPriceInclusiveTax = salesPriceInclusiveTax,
                                        CategoryId = categoryId,
                                        UnitId = unitId,
                                        StockQuantity = stockQuantity,
                                        AlertQuantity = alertQuantity
                                    };
                                    _db.DbItems.InsertOnSubmit(newItem);
                                }
                            }
                        }

                        _db.SubmitChanges();
                    }
                }

                var itemList = from i in _db.DbItems join u in _db.DbUnits on i.UnitId equals u.UnitId select new { i.ItemCode, i.ItemName, i.ItemDescription, i.HsnSac, i.TaxRate, i.Cess, i.PurchasePrice, PurchasePriceInclusiveTax = i.PurchasePriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", i.SalesPrice, SalesPriceInclusiveTax = i.SalesPriceInclusiveTax == true ? "Inclusive Tax" : "Exclusive Tax", Unit = u.UnitName, i.StockQuantity, i.AlertQuantity };

                return Json(new { success = true, ItemList = itemList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult PrintInventory()
        {
            var itemList = (from i in _db.DbItems select new InventoryViewModel { ItemCode = i.ItemCode, ItemName = i.ItemName , StockQuantity = i.StockQuantity, AlertQuantity = i.AlertQuantity, PurchasePrice = i.PurchasePriceInclusiveTax == true ? i.PurchasePrice / (1 + (i.TaxRate / 100)) : i.PurchasePrice, SalesPrice = i.SalesPriceInclusiveTax == true ? i.SalesPrice / (1 + (i.TaxRate / 100)) : i.SalesPrice }).ToList();

            var model = new InventoryPrintViewModel();

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

            model.CurrentDateTime = DateTime.Now.Date.ToString("dd-MM-yyyy");
            model.TotalItems = itemList.Count();
            model.TotalAlertItems = itemList.Where(x => x.AlertQuantity > 0 && x.StockQuantity <= x.AlertQuantity).Count();

            model.InventoryViewModel = itemList;

            return View(model);
        }
    }
}