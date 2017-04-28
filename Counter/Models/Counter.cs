using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Counter.Models
{
    public class Counter
    {
        public virtual int Id { get; set; }
        [MaxLength(256), Index(IsUnique = true)]
        public virtual string Name { get; set; }
        public virtual List<CounterValue> Values { get; set; }
    }
}