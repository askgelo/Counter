using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Counter.Models;

namespace Counter.Controllers
{
    public class CountersController : Controller
    {
        private CounterContext db = new CounterContext();

        // GET: Counters
        public async Task<ActionResult> Index()
        {
            return View(await db.Counters.ToListAsync());
        }

        // GET: Counters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var counter = await db.Counters.FindAsync(id);
            if (counter == null)
            {
                return HttpNotFound();
            }
            return View(counter);
        }

        // GET: Counters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Counters/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Models.Counter counter)
        {
            if (db.Counters.Where(w => w.Name == counter.Name).Count() > 0)
                ModelState.AddModelError("Name", "Счетчик с таким именем уже существует");

            if (ModelState.IsValid)
            {
                db.Counters.Add(counter);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(counter);
        }

        // GET: Counters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var counter = await db.Counters.FindAsync(id);
            if (counter == null)
            {
                return HttpNotFound();
            }
            return View(counter);
        }

        // POST: Counters/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] Models.Counter counter)
        {
            var existCounter = db.Counters.FirstOrDefault(v => v.Name == counter.Name);
            if (existCounter != null && existCounter.Id != counter.Id)
            {
                ModelState.AddModelError("Name", "Счетчик с таким именем уже существует!");
            }

            if (ModelState.IsValid)
            {
                if (existCounter == null)
                {
                    db.Entry(counter).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            return View(counter);
        }

        // GET: Counters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var counter = await db.Counters.FindAsync(id);
            if (counter == null)
            {
                return HttpNotFound();
            }
            return View(counter);
        }

        // POST: Counters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var counter = await db.Counters.FindAsync(id);
            db.Counters.Remove(counter);
            await db.SaveChangesAsync();
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
