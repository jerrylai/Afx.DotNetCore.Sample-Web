using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Models
{
    public interface ICreateTime : IModel
    {
        DateTime CreateTime { get; set; }
    }
}
