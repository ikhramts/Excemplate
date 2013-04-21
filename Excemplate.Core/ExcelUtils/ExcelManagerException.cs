using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excemplate.Core.ExcelUtils
{
    class ExcelManagerException : Exception
    {
        public ExcelManagerException(string message)
        : base(message)
        {
        }
    }
}
