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
    public class CounterValuesController : Controller
    {
        private CounterContext db = new CounterContext();

        // GET: CounterValues
        public async Task<ActionResult> Index()
        {
            var counterValues = db.CounterValues.Include(c => c.Counter)
                .OrderBy(v => v.Date).ThenBy(v => v.Counter.Name);
            return View(await counterValues.ToListAsync());
        }

        // GET: CounterValues/Create
        public ActionResult Create()
        {
            ViewBag.CounterId = new SelectList(db.Counters, "Id", "Name");
            ViewBag.Date = DateTime.Now;
            return View();
        }

        // POST: CounterValues/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CounterId,Date,Value")] CounterValue counterValue)
        {
            if (counterValue.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("Date", "Date must be <= Now");
            }
            else
            {
                var existsValue = db.CounterValues
                    .Where(v => v.CounterId == counterValue.CounterId && v.Date == counterValue.Date)
                    .FirstOrDefault()?.Value;
                if (existsValue.HasValue)
                    ModelState.AddModelError("Date", $"For this date exists value {existsValue.Value}");
            }

            if (ModelState.IsValid)
            {
                var maxValue = db.CounterValues
                    .Where(v => v.CounterId == counterValue.CounterId && v.Date < counterValue.Date)
                    .GroupBy(v => v.CounterId)
                    .Select(v => v.Max(gv => gv.Value))
                    .FirstOrDefault();

                if (counterValue.Value < maxValue)
                    ModelState.AddModelError("Value", $"Value ({counterValue.Value}) must be > max previous value ({maxValue})");
            }

            if (ModelState.IsValid)
            {

                db.CounterValues.Add(counterValue);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CounterId = new SelectList(db.Counters, "Id", "Name", counterValue.CounterId);
            ViewBag.Date = counterValue.Date;
            return View(counterValue);
        }

        // GET: CounterValues/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CounterValue counterValue = await db.CounterValues.FindAsync(id);
            if (counterValue == null)
            {
                return HttpNotFound();
            }
            ViewBag.CounterId = new SelectList(db.Counters, "Id", "Name", counterValue.CounterId);
            return View(counterValue);
        }

        // POST: CounterValues/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CounterId,Date,Value")] CounterValue counterValue)
        {
            if (counterValue.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("Date", "Date must be <= Now");
            }
            else
            {
                var existsValue = db.CounterValues
                    .Where(v => v.Id != counterValue.Id && v.CounterId == counterValue.CounterId && 
                    v.Date == counterValue.Date)
                    .FirstOrDefault()?.Value;
                if (existsValue.HasValue)
                    ModelState.AddModelError("Date", $"For this date exists value {existsValue.Value}");
            }

            var concurrentValue = db.CounterValues
                .Where(v => v.Id != counterValue.Id && v.CounterId == counterValue.CounterId && 
                (v.Date == counterValue.Date || 
                (v.Date < counterValue.Date && v.Value > counterValue.Value) ||
                (v.Date > counterValue.Date && v.Value < counterValue.Value)))
                .FirstOrDefault();

            if (concurrentValue != null)
                ModelState.AddModelError("Value",
                    string.Format("Exist concurrent value ({0}: {1})", 
                    concurrentValue.Date.ToString("dd.MM.yyyy"), concurrentValue.Value));

            if (ModelState.IsValid)
            {
                db.Entry(counterValue).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CounterId = new SelectList(db.Counters, "Id", "Name", counterValue.CounterId);
            return View(counterValue);
        }

        // GET: CounterValues/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CounterValue counterValue = await db.CounterValues.FindAsync(id);
            if (counterValue == null)
            {
                return HttpNotFound();
            }
            return View(counterValue);
        }

        // POST: CounterValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CounterValue counterValue = await db.CounterValues.FindAsync(id);
            db.CounterValues.Remove(counterValue);
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
