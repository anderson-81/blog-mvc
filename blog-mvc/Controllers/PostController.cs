using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using blog_mvc.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Data.Entity.Core.Objects;

namespace blog_mvc.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class PostController : Controller
    {
        private blogDBEntities db = new blogDBEntities();
        public static bool statusClearMessage = false;
        
        [AllowAnonymous]
        public ActionResult Index(int? _page)
        {
            this.Clear();
            int sizePage = 7;
            int numPage = _page ?? 1;
            var post = db.Post.OrderByDescending(p => p.DateCreate);
            return View(post.ToList().ToPagedList(numPage, sizePage));
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(string search, int? _page)
        {
            this.Clear();
            int sizePage = 7;
            int numPage = _page ?? 1;
            var post = db.Post.Where(p => p.Title.Contains(search)).OrderByDescending(p => p.DateCreate);
            return View(post.ToList().ToPagedList(numPage, sizePage));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            PostController.statusClearMessage = true;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            var base64 = Convert.ToBase64String(post.Picture);
            var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
            ViewData["postImg"] = imgSrc;

            return View(post);
        }

        [Authorize]
        public ActionResult Create()
        {
            PostController.statusClearMessage = true;
            ModelState.Clear();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Briefing,Textpost")] Post post, HttpPostedFileBase picture)
        {
            ModelState.Remove("Picture");

            post.Aspnetusers_Id = User.Identity.GetUserId();

            post.Id = this.GetID();

            if (post.Aspnetusers_Id == null)
            {
                ModelState.AddModelError("User", "The User is required.");
            }

            post.DateCreate = DateTime.Now;

            if (picture != null)
            {
                post.Picture = new byte[picture.ContentLength];
                picture.InputStream.Read(post.Picture, 0, picture.ContentLength);
            }
            else
            {
                ModelState.AddModelError("Picture", "The Picture is required.");
                return View(post);
            }

            if (ModelState.IsValid)
            {
                db.Post.Add(post);
                db.SaveChanges();
                this.CreateMessage("Successfully published.", "Information", 1);
                return RedirectToActionPermanent("Index");
            }
            return View(post);
        }

        [Authorize]
        public ActionResult Edit(int? id, int opc)
        {
            this.Clear();
            if (opc == 1)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Post post = db.Post.Find(id);
                if (post != null)
                {
                    if (post.Aspnetusers_Id == User.Identity.GetUserId())
                    {
                        var base64 = Convert.ToBase64String(post.Picture);
                        var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
                        ViewData["postImg"] = imgSrc;
                        return View(post);
                    }
                    else
                    {
                        this.CreateMessage("This Post only can be edited by user that did published.", "Information", 1);
                    }
                    return RedirectToAction("Details", new { id = post.Id });
                }
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Post post = db.Post.Find(id);
                if (post != null)
                {
                    if (post.Aspnetusers_Id == User.Identity.GetUserId())
                    {
                        db.Post.Remove(post);
                        db.SaveChanges();
                        this.CreateMessage("Successfully deleted.", "Information", 1);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.CreateMessage("This Post only can be deleted by user that did published.", "Information", 1);
                        return RedirectToAction("Details", new { id = post.Id });
                    }
                }
            }
            return HttpNotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Briefing,Textpost,DateCreate,Picture")] Post post, HttpPostedFileBase picture)
        {
            ModelState.Remove("DateCreate");
            ModelState.Remove("Picture");

            post.Aspnetusers_Id = User.Identity.GetUserId();

            if (post.Aspnetusers_Id == null)
            {
                ModelState.AddModelError("User", "The User is required.");
            }

            post.DateChange = DateTime.Now;

            if (picture != null)
            {
                post.Picture = new byte[picture.ContentLength];
                picture.InputStream.Read(post.Picture, 0, picture.ContentLength);
            }

            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                this.CreateMessage("Successfully edited.", "Information", 1);
                return RedirectToAction("Details", new { id = post.Id });
            }

            var base64 = Convert.ToBase64String(post.Picture);
            var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
            ViewData["postImg"] = imgSrc;
            return View(post);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize]
        private int GetID()
        {
            try
            {
                int id = db.Post.Max(post => post.Id) + 1;
                return id;

            }
            catch (Exception)
            {
                return 1;
            }
        }

        private void CreateMessage(string message, string title, int show)
        {
            TempData["message"] = message;
            TempData["title"] = title;
            TempData["show"] = show;
            PostController.statusClearMessage = false;
        }

        private void Clear()
        {
            if (PostController.statusClearMessage)
            {
                foreach (var key in TempData.Keys.ToList())
                {
                    TempData.Remove(key);
                }
            }

            try
            {
                Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
            }
            catch (Exception)
            {
            }
        }
    }
}
