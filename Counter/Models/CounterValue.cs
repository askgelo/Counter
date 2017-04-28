using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Counter.Models
{
    public class CounterValue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [Required]
        [Display(Name = "Counter")]
        public virtual int CounterId { get; set; }
        public virtual Counter Counter { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public virtual DateTimeOffset Date { get; set; }
        [Required]
        public virtual double Value { get; set; }
    }
}