using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MVCBASE.Models;
using MVCBASE.ViewModels;
using System.Net;
using System.Data.Entity;
using System.IO;
using System.Windows.Forms;

namespace MVCBASE.Controllers
{
    public class EmployeeController : Controller
    {
        private NorthwindChineseEntities db = new NorthwindChineseEntities();
        //每頁五筆
        private const int PageSize = 5;

        public ActionResult Index(int page =1)
        {
            var query = db.Employees.OrderBy(x => x.EmployeeID);
            //設定是否為編輯or新增回來的頁面
            if (Session["back"] == null)
            {
                Session["back"] = "false";
                Session["pageback"] = "1";
            }

            int pageIndex = page < 1 ? 1 : page;
            //初始化
            Session["path"] = "";

            //從編輯回來頁數回復
            if (Session["back"].ToString() == "true")
            {
                pageIndex = Convert.ToInt32(Session["pageback"].ToString());

            }

            var model = new EmployeeListViewModel
            {
                SearchParameter = new EmployeeSearchModel(),
                PageIndex = pageIndex,
                Employees = query.ToPagedList(pageIndex, PageSize)

            };

            //若從編輯or新增回來
            if (Session["back"].ToString() == "true")
            {
                if (Session["queryEmployeeName"] == null) { Session["queryEmployeeName"] = ""; }
                if (Session["queryTitle"] == null) { Session["queryTitle"] = ""; }

                model.SearchParameter.EmployeeName = Session["queryEmployeeName"].ToString();
                model.SearchParameter.Title = Session["queryTitle"].ToString();
                Session["back"] = "false";
                return  Index(model);
                
            }

            return View(model);
        }
        ///查詢
        [HttpPost]
        public ActionResult Index(EmployeeListViewModel model)
        {
            var query = db.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.SearchParameter.EmployeeName))
            {
                query = query.Where(
                    x => x.EmployeeName.Contains(model.SearchParameter.EmployeeName));
                Session["queryEmployeeName"] = model.SearchParameter.EmployeeName;
            }
            else
            {
                Session["queryEmployeeName"] = "";
            }
            if (!string.IsNullOrWhiteSpace(model.SearchParameter.Title))
            {
                query = query.Where(
                    x => x.Title.Contains(model.SearchParameter.Title));
                Session["queryTitle"] = model.SearchParameter.Title.ToString();
            }
            else
            {
                Session["queryTitle"] = "";
            }

          
            query = query.OrderBy(x => x.EmployeeID);

            int pageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
            Session["pageback"] = pageIndex;
            var result = new EmployeeListViewModel
            {
                SearchParameter = model.SearchParameter,
                PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex,
                Employees = query.ToPagedList(pageIndex, PageSize)
            };

            return View(result);

        }
        ///新增
        public ActionResult Create()
        {
            Session["back"] = "true";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,EmployeeName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,HomePhone,PhotoPath,Extension,Notes,ManagerID,Salary")] Employees employees, HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    //判斷檔案名稱
                    // 取得副檔名
                    string fileType = file.FileName.Split('.').Last().ToUpper();
                    if (!(fileType.Equals("JPG") || fileType.Equals("PNG") || fileType.Equals("GIF")))
                    {
                        TempData["Message"] = "只接受jpg,png,gif等圖片類型";
                        return View(employees);
                    }
                    TempData["Message"] = "";
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/UploadedImages"), fileName);
                    Session["path"] = fileName;//儲存檔案名稱
                    file.SaveAs(path);
                }
            }
           
            if (ModelState.IsValid)
            {
                employees.PhotoPath = Session["path"].ToString();
                db.Employees.Add(employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employees);
        }
        ///修改
        public ActionResult Edit(int? id)
        {
            Session["back"] = "true";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,EmployeeName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,HomePhone,Extension,PhotoPath,Notes,ManagerID,Salary")] Employees employees, HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    //判斷檔案名稱
                    // 取得副檔名
                    string fileType = file.FileName.Split('.').Last().ToUpper();
                    if (!(fileType.Equals("JPG") || fileType.Equals("PNG") || fileType.Equals("GIF")))
                    {
                        TempData["Message"] = "只接受jpg,png,gif等圖片類型";
                        return View(employees);
                    }
                    TempData["Message"] = "";
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/UploadedImages"), fileName);
                    Session["path"] = fileName;
                    file.SaveAs(path);
                }
            }

            if (ModelState.IsValid)
            {
                employees.PhotoPath = Session["path"].ToString();
                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employees);
        }


       ///直接刪除
        public ActionResult Delete(int? id)
        {
            Session["back"] = "true";
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            //跳出確認視窗
            DialogResult dialogResult = MessageBox.Show("您確定要刪除 "+ employees.EmployeeName +" 的資料嗎?", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                db.Employees.Remove(employees);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

 

    }
}