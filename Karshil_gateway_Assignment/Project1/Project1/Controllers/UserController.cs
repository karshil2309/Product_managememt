using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project1.Models;

namespace WebApplication1.Controllers
{//user products and delete products page. Home page with Advertisment
    public class UserController : Controller
    {

        DBProduct_mgmtEntities2 db = new DBProduct_mgmtEntities2();
        // GET: User
        public ActionResult Index(int? page) // pagination for the categories.
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Categories.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }
        public ActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(tbl_user uvm, HttpPostedFileBase imgfile) //signup form
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");

            }

            return View();
        } //method......................... end.....................

        public ActionResult login()
        {
            return View();
        }


        [HttpPost] //login starts...............................................
        public ActionResult login(tbl_user avm) //login form for user and other was for admin
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {

                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }


        [HttpGet]
        public ActionResult CreateAd()
        {
            List<Category> li = db.Categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            return View();
        }

        [HttpPost]
        public ActionResult CreateAd(Product pvm, HttpPostedFileBase imgfile) //for posting products into categories
        {
            List<Category> li = db.Categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");


            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                Product p = new Product();
                p.pro_name = pvm.pro_name;
                p.pro_price = pvm.pro_price;
                p.pro_image = path;
                p.pro_fk_cat = pvm.pro_fk_cat;
                p.pro_des = pvm.pro_des;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                db.Products.Add(p);
                db.SaveChanges();
                Response.Redirect("index");

            }

            return View();
        }


        public ActionResult Ads(int? id, int? page) //pagination for products in descending
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search) // search bar in the top for searching the products
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }


        public ActionResult ViewAd(int? id) //to see the details of the product with other fields
        {
            Adviewmodel ad = new Adviewmodel();
            Product p = db.Products.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_image = p.pro_image;
            ad.pro_price = p.pro_price;
            ad.pro_des = p.pro_des;
            Category cat = db.Categories.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.pro_fk_user = u.u_id;




            return View(ad);
        }


        public ActionResult Signout() // logout option on the top right side . 
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }



        public ActionResult DeleteAd(int? id)// Delete query for the deleting the add
        {

            Product p = db.Products.Where(x => x.pro_id == id).SingleOrDefault();
            db.Products.Remove(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }









        public string uploadimgfile(HttpPostedFileBase file)// uploading image code
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
                    catch (Exception )
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







    }
}