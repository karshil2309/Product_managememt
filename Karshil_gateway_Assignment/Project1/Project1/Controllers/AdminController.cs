using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList;

using PagedList.Mvc;
namespace Project1.Controllers
{
    public class AdminController : Controller
    {
        DBProduct_mgmtEntities2 db = new DBProduct_mgmtEntities2();
        // GET: Admin can add the categories for different products: Eg- Food,Sports etc
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_admin avm)//admin table where they can login to add and view category
        {
            tbl_admin ad = db.tbl_admin.Where(x => x.admin_username == avm.admin_username && x.admin_password == avm.admin_password).SingleOrDefault();
            if (ad != null)
            {
                Session["ID"] = ad.Id.ToString();
                Session["admin_username"] = ad.admin_username;
                return RedirectToAction("welcome");
            }
            else
            {
                ViewBag.error = "Invalid Username or password";
            }
            return View();

        }
        public ActionResult welcome()
        {
            return View();
        }
        public ActionResult create() // this method is for holding the login page. i.e when user clicks back button it will not redirect.
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category cvm, HttpPostedFileBase imgfile)// method used to show the list of products and in their cateogry
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                Category cat = new Category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_status = 1;
                cat.cat_fk_ad = Convert.ToInt32(Session["ID"].ToString());
                db.Categories.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            return View();
        }
        public string uploadimgfile(HttpPostedFileBase file)//uploading file and save it to local dir
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }
            return path;
        }

        public ActionResult ViewCategory(int? page)// code for pagination
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Categories.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);




        }
    }
}