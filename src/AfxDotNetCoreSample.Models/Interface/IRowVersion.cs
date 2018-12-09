using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AfxDotNetCoreSample.Models
{
    public interface IRowVersion : IModel
    {
        [ConcurrencyCheck]
        int RowVersion { get; set; }
    }
}
