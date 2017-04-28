using Counter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Counter.Controllers
{
    public class CounterValuesLastController : Controller
    {
        CounterContext db = new CounterContext();

        // GET: CounterValuesLast
        public ActionResult Index()
        {
            //db.Database.Log = s => System.Diagnostics.Debug.Write(s);
            if (db.Counters.Count() == 0)
                return Redirect("/Counters/Create");

            return View(GetLastValues());
        }

        [HttpPost]
        public ActionResult Index(List<CounterValuesLastData> counterValues)
        {
            db.Database.Log = w => System.Diagnostics.Debug.Write(w);

            List<CounterValuesLastData> actualValues = GetLastValues();
            if (ModelState.IsValid)
            {
                for (int i = 0; i < counterValues.Count; i++)
                {
                    var value = counterValues[i];
                    if (value.NewValue.HasValue)
                    {
                        var actualValue = actualValues.FirstOrDefault(v => v.CounterId == value.CounterId);
                        if (actualValue == null)
                        {
                            // TODO check summary
                            ModelState.AddModelError($"Summary {i}", $"Counter ({value.CounterName}) not exists");
                        }
                        else if (actualValue.Value > value.NewValue && actualValue.Date < DateTime.Now.Date)
                        {
                            ModelState.AddModelError($"[{i}].NewValue",
                                $"New value ({value.NewValue}) < old value ({actualValue.Value})");
                        }
                        else if (actualValue.Date == DateTime.Now.Date)
                        {
                            var prevValue = db.CounterValues
                                .Where(v => v.CounterId == value.CounterId &&
                                        v.Date < DbFunctions.TruncateTime(DateTime.Now))
                                .OrderByDescending(v => v.Value)
                                .FirstOrDefault();
                            if (prevValue?.Value > value.NewValue)
                            {
                                ModelState.AddModelError($"[{i}].NewValue",
                                    $"New value ({value.NewValue}) < last value ({prevValue.Value})");
                            }
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                foreach (var value in counterValues.Where(v => v.NewValue.HasValue))
                {
                    var actualValue = actualValues.FirstOrDefault(v => v.CounterId == value.CounterId);
                    if (!actualValue.Date.HasValue || actualValue.Date < DateTimeOffset.Now.Date)
                    {
                        db.CounterValues.Add(new CounterValue()
                        {
                            CounterId = value.CounterId,
                            Date = DateTimeOffset.Now.Date,
                            Value = value.NewValue.Value
                        });
                    }
                    else if (actualValue.Value.Value != value.NewValue.Value) // обновить текущее значение
                    {
                        var updatedValue = db.CounterValues
                            .Single(v => v.CounterId == value.CounterId && v.Date == actualValue.Date.Value);
                        updatedValue.Value = value.NewValue.Value;
                        db.Entry(updatedValue).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();
                return Redirect("/CounterValues");
            }

            if (counterValues?.Count > 0)
            {
                foreach (var actualValue in actualValues)
                {
                    var newValue = counterValues.FirstOrDefault(v => v.CounterId == actualValue.CounterId);
                    if (newValue != null && newValue.Value.HasValue)
                        actualValue.NewValue = newValue.NewValue;
                }
            }
            return View(actualValues);
        }

        List<CounterValuesLastData> GetLastValues()
        {
            var lastKeys = from v in db.CounterValues
                           group v by v.CounterId into temp
                           select new { id = temp.Key, m = temp.Max(v => v.Date) };

            var lastValues = from f in db.CounterValues
                             from k in lastKeys
                             where f.CounterId == k.id && f.Date == k.m
                             select f;

            var countersLastValues = (from c in db.Counters
                                      join v in lastValues on c.Id equals v.CounterId into temp
                                      from t in temp.DefaultIfEmpty()
                                      select new { c, t })
                           .ToList()
                           .Select(v => new CounterValuesLastData()
                           {
                               CounterId = v.c.Id,
                               CounterName = v.c.Name,
                               Date = v.t?.Date,
                               Value = v.t?.Value
                           });
            return countersLastValues.ToList();
        }
    }
}