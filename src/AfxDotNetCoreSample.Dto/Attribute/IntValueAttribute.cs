using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AfxDotNetCoreSample.Dto
{
    public class IntValueAttribute : ValidationAttribute
    {
        public int? Min { get; set; }
        public int? Max { get; set; }

        public IntValueAttribute(int min)
        {
            this.Min = min;
        }

        public IntValueAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        public override bool IsValid(object value)
        {
            if(value is int)
            {
                int v = (int)value;
                if(this.Min.HasValue && this.Min.Value >= v)
                {
                    return false;
                }

                if (this.Max.HasValue && this.Max.Value < v)
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
