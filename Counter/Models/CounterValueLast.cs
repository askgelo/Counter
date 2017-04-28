using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Reflection;

namespace Counter.Models
{
    public class CounterValuesLastData
    {
        public int CounterId { get; set; }
        [Display(Name = "Counter")]
        public string CounterName { get; set; }
        [Display(Name = "Last date")]
        [DataType(DataType.Date)]
        public DateTimeOffset? Date { get; set; }
        [Display(Name = "Last value")]
        public double? Value { get; set; }

        [Display(Name = "New value")]
        //[CompareTo("Value", 1)]
        public double? NewValue { get; set; }
    }

    //public class CompareToAttribute : ValidationAttribute
    //{
    //    string otherProperty;
    //    int typeComparison;

    //    public CompareToAttribute(string otherProperty, int typeComparison)
    //    {
    //        this.otherProperty = otherProperty;
    //        this.typeComparison = typeComparison;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        if (value != null && value as IComparable != null)
    //        {
    //            var objectInstance = validationContext.ObjectInstance;
    //            var property = validationContext.ObjectType.GetProperty(otherProperty);
    //            var otherValue = property.GetValue(objectInstance);
    //            if (otherValue != null)
    //            {
    //                int resultCompare = ((IComparable)value).CompareTo(otherValue);
    //                if (!(resultCompare == typeComparison ||
    //                    (resultCompare < 0 && typeComparison < 0) ||
    //                    (resultCompare > 0 && typeComparison > 0)))
    //                {
    //                    string displayNameOtherProperty = otherProperty;
    //                    var attr = property.GetCustomAttribute<DisplayAttribute>();
    //                    if (attr != null)
    //                        displayNameOtherProperty = attr.Description;
    //                    return new ValidationResult(string.Format("{0} must be > {1}",
    //                        validationContext.DisplayName, displayNameOtherProperty));
    //                }
    //            }
    //        }
    //        return ValidationResult.Success;
    //    }
    //}
}