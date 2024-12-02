using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using webBinh.Models;

namespace webBinh.Controllers
{
    public class columnsController : Controller
    {
        private webNangCaoQLDA2Entities1 db = new webNangCaoQLDA2Entities1();

        // GET: columns
        public ActionResult Index()
        {
            var columns = db.columns.Include(c => c.Project);
            return View(columns.ToList());
        }

        // GET: columns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            column column = db.columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            return View(column);
        }

        // GET: columns/Create
        
       
        public async Task<ActionResult> CreateColumn([Bind(Include = "id_column,title,createdAt")] column column, string idpr)
        {
          
                using (var httpClient = new HttpClient())
                {
                    var url = "http://localhost:5224/createColumn";
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("id_column", column.id_column.ToString()),
                        new KeyValuePair<string, string>("IdProject",idpr),
                        new KeyValuePair<string, string>("Title", column.title),
                        new KeyValuePair<string, string>("CreatedAt", column.createdAt.ToString())
                    });

                    var response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        if (responseData.Contains("ok"))
                        {
                            return RedirectToAction("index", "project");
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Không thể kết nối với API.";
                    }
                }
            return View();
            
           
        }

        // GET: columns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            column column = db.columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_project = new SelectList(db.Projects, "id_project", "mota", column.id_project);
            return View(column);
        }

        // POST: columns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_column,id_project,title,createdAt")] column column)
        {
            if (ModelState.IsValid)
            {
                db.Entry(column).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_project = new SelectList(db.Projects, "id_project", "mota", column.id_project);
            return View(column);
        }

        // GET: columns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            column column = db.columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            return View(column);
        }

        // POST: columns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            column column = db.columns.Find(id);
            db.columns.Remove(column);
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
