using Counter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Counter.Controllers
{
    public class CounterGraphController : Controller
    {
        CounterContext db = new CounterContext();

        // GET: CounterGraph
        public ActionResult Index()
        {
            db.Database.Log = w => System.Diagnostics.Debug.Write(w);

            var values = db.CounterValues
                .GroupBy(v => v.CounterId)
                .Where(g => g.Count() > 1)
                .ToList();
            var graphValues = new Dictionary<string, List<Values>>();
            DateTime minDate = DateTime.MaxValue.Date;
            DateTime maxDate = DateTime.MinValue.Date;
            foreach (var groupValues in values)
            {
                var graphValue = new List<Values>();
                CounterValue prevValue = null;
                double avgValue = 0;
                double maxValue = 0;
                foreach (var value in groupValues.OrderBy(v => v.Date))
                {
                    if (minDate > value.Date.Date) minDate = value.Date.Date;
                    if (maxDate < value.Date.Date) maxDate = value.Date.Date;

                    if (prevValue != null)
                    {
                        avgValue = (value.Value - prevValue.Value) / (value.Date - prevValue.Date).Days;
                        if (maxValue < avgValue) maxValue = avgValue;
                        graphValue.Add(new Values() { Date = prevValue.Date, Percent = avgValue });
                    }
                    prevValue = value;
                }
                graphValue.Add(new Values() { Date = prevValue.Date, Percent = avgValue });
                
                foreach (var value in graphValue)
                {
                    value.Percent = value.Percent / maxValue * 100;
                }
                graphValues.Add(groupValues.First().Counter.Name, graphValue);
            }
            if (minDate > maxDate) minDate = maxDate;

            var data = graphValues.Select(v => new CounterGraph() { Counter = v.Key, Values = v.Value });
            return View(new Graph()
            {
                Counters = graphValues.Select(v => new CounterGraph() { Counter = v.Key, Values = v.Value }).ToArray(),
                LengthPeriodInMonths = (int)((maxDate - minDate).TotalDays / 30)
            });
        }
    }
}