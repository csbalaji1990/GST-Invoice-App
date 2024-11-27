using System;
using System.Linq;
using System.Web.Mvc;
using Invoice.Models;
using System.Data.SqlClient;

namespace Invoice.Controllers
{
    public class SettingController : Controller
    {
        private readonly EntitiesDataContext _db = new EntitiesDataContext();

        [CheckIfLoggedIn]
        public ActionResult Unit()
        {
            var unitList = from u in _db.DbUnits select new { u.UnitId, u.UnitName };

            ViewBag.UnitList = unitList;

            return View();
        }

        public JsonResult GetUnit(int UnitId)
        {
            try
            {
                var unit = (from u in _db.DbUnits where u.UnitId == UnitId select new { u.UnitId, u.UnitName }).FirstOrDefault();

                if (unit != null)
                    return Json(new { success = true, Unit = unit }, JsonRequestBehavior.AllowGet);

                return Json(new { success = false, Message = "Unit not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveUnit(int UnitId, string UnitName)
        {
            try
            {
                var updateUnit = (from u in _db.DbUnits where u.UnitId == UnitId select u).FirstOrDefault();

                if (updateUnit != null)
                {
                    updateUnit.UnitName = UnitName;
                }
                else
                {
                    var newUnit = new DbUnit
                    {
                        UnitId = (from u in _db.DbUnits orderby u.UnitId descending select u.UnitId).FirstOrDefault() + 1,
                        UnitName = UnitName
                    };
                    _db.DbUnits.InsertOnSubmit(newUnit);
                }

                _db.SubmitChanges();

                var unitList = from u in _db.DbUnits select new { u.UnitId, u.UnitName };

                return Json(new { success = true, UnitList = unitList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteUnit(int UnitId)
        {
            try
            {
                var deleteUnit = (from u in _db.DbUnits where u.UnitId == UnitId select u).FirstOrDefault();

                if (deleteUnit != null)
                {
                    _db.DbUnits.DeleteOnSubmit(deleteUnit);
                    _db.SubmitChanges();

                    var unitList = from u in _db.DbUnits select new { u.UnitId, u.UnitName };

                    return Json(new { success = true, UnitList = unitList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Unit not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))
                {
                    Int32 ErrorCode = ((SqlException)ex).Number;

                    if (ErrorCode == 547)
                        return Json(new { success = false, Message = "Cannot delete this unit" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckIfLoggedIn]
        public ActionResult Category()
        {
            var categoryList = from c in _db.DbCategories select new { c.CategoryId, c.CategoryName };

            ViewBag.CategoryList = categoryList;

            return View();
        }

        public JsonResult GetCategory(int CategoryId)
        {
            try
            {
                var category = (from c in _db.DbCategories where c.CategoryId == CategoryId select new { c.CategoryId, c.CategoryName }).FirstOrDefault();

                if (category != null)
                    return Json(new { success = true, Category = category }, JsonRequestBehavior.AllowGet);

                return Json(new { success = false, Message = "Category not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCategory(int CategoryId, string CategoryName)
        {
            try
            {
                var updateCategory = (from c in _db.DbCategories where c.CategoryId == CategoryId select c).FirstOrDefault();

                if (updateCategory != null)
                {
                    updateCategory.CategoryName = CategoryName;
                }
                else
                {
                    var newCategory = new DbCategory
                    {
                        CategoryId = (from c in _db.DbCategories orderby c.CategoryId descending select c.CategoryId).FirstOrDefault() + 1,
                        CategoryName = CategoryName
                    };
                    _db.DbCategories.InsertOnSubmit(newCategory);
                }

                _db.SubmitChanges();

                var categoryList = from c in _db.DbCategories select new { c.CategoryId, c.CategoryName };

                return Json(new { success = true, CategoryList = categoryList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteCategory(int CategoryId)
        {
            try
            {
                var deleteCategory = (from c in _db.DbCategories where c.CategoryId == CategoryId select c).FirstOrDefault();

                if (deleteCategory != null)
                {
                    _db.DbCategories.DeleteOnSubmit(deleteCategory);
                    _db.SubmitChanges();

                    var categoryList = from c in _db.DbCategories select new { c.CategoryId, c.CategoryName };

                    return Json(new { success = true, CategoryList = categoryList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, Message = "Category not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))
                {
                    Int32 ErrorCode = ((SqlException)ex).Number;

                    if (ErrorCode == 547)
                        return Json(new { success = false, Message = "Cannot delete this category" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
