﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SistemaControle.Models;
using SistemaControle.Classes;

namespace SistemaControle.Controllers
{
    public class UsuariosController : Controller
    {
        private ControleContext db = new ControleContext();

        // GET: Usuarios
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioView view)
        {
            if (ModelState.IsValid)
            {
                db.Usuarios.Add(view.Usuario);
                try
                {
                    //db.SaveChanges();
                    if(view.Fotos != null)
                    {
                        var pic = Ultilidades.UploadPhoto(view.Fotos);

                        if (!string.IsNullOrEmpty(pic))
                        {
                            view.Usuario.Photo = string.Format("~/Content/Fotos/{0}", pic);
                        }
                    }
                    db.SaveChanges();

                    Ultilidades.CreateUserASP(view.Usuario.UserName);

                    if (view.Usuario.Estudante)
                    {
                        Ultilidades.AddRoleToUser(view.Usuario.UserName,"Estudante");
                    }
                    if (view.Usuario.Professor)
                    {
                        Ultilidades.AddRoleToUser(view.Usuario.UserName, "Professor");
                    }

                    return RedirectToAction("Index");
                }
                catch (System.Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(view);

        }
    

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var view = new UsuarioView
            {
                Usuario = usuario
            };
            return View(view);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsuarioView view)
        {

            if (ModelState.IsValid)
            {
                var db2 = new ControleContext();
                var oldUser = db2.Usuarios.Find(view.Usuario.UserId);
                db2.Dispose();



                if (view.Fotos != null)
                {
                    var pic = Ultilidades.UploadPhoto(view.Fotos);

                    if (!string.IsNullOrEmpty(pic))
                    {
                        view.Usuario.Photo = string.Format("~/Content/Fotos/{0}", pic);
                    }
                }else
                {
                    view.Usuario.Photo = oldUser.Photo;
                }

                db.Entry(view.Usuario).State = EntityState.Modified;
                try
                {
                    if (oldUser.UserName != null && oldUser.UserName != view.Usuario.UserName)
                    {
                        Ultilidades.ChangeEmailUserASP(oldUser.UserName, view.Usuario.UserName);
                    }

                    db.SaveChanges();

                }
                catch (System.Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(view);
                }
                return RedirectToAction("Index");
            }
            return View(view.Usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
