using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Models
{
    public interface IValidationExceptionHandler
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValid
        {
            get;set;
        }
    }
}
