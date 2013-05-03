using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

namespace Excemplate.Core.ExcelUtils
{
    /// <summary>
    /// This class is reponsible for converting various data types between
    /// .NET and Excel formats.
    /// </summary>
    public class DataConverter
    {
        public static object ExcelFromDotNet(object value)
        {
            var type = value.GetType();

            if (type == typeof(DateTime))
            {
                var oleDate = ((DateTime)value).ToOADate();

                // Adjust to reproduce intentional Lotus 1-2-3 compatibility bug.
                var excelDate = (oleDate > 60 ? oleDate : oleDate - 1);
                return excelDate;
            }
            else if (type == typeof(System.DBNull))
            {
                return Type.Missing;
            }

            return value;
        }
    }
}
