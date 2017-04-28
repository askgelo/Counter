using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Counter.Models
{
    public class Graph
    {
        public int LengthPeriodInMonths { get; set; }
        public CounterGraph[] Counters { get; set; }
    }

    public class CounterGraph
    {
        public string Counter { get; set; }
        public List<Values> Values { get; set; }
    }

    public class Values
    {
        public DateTimeOffset Date { get; set; }
        public double Percent { get; set; }
    }
}