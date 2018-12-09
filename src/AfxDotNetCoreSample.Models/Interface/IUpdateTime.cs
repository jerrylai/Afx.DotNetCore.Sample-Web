using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Models
{
    public interface IUpdateTime : IModel
    {
        DateTime UpdateTime { get; set; }
    }
}
