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

        /// <summary>
        /// Convert row and column into a A1-style cell address.
        /// </summary>
        /// <param name="row">One-based row.</param>
        /// <param name="column">One-based column.</param>
        /// <returns></returns>
        public static string A1FromRowCol(int row, int column)
        {
            // Convert column to letters.
            var first = (column - 1) % 26;
            var second = ((column - 26 - 1) / 26) % 26;
            var third = (column - 26 * (26 + 1) - 1) / 26 / 26;

            var address = new StringBuilder();

            if (column > 702)
            {
                address.Append((char)('A' + third));
                address.Append((char)('A' + second));
            }
            else if (column > 26)
            {

                address.Append((char)('A' + second));
            }

            address.Append((char)('A' + first));
            address.Append(row);
            return address.ToString();
        }

        public static string A1FromRectangle(int minRow, int minCol, int maxRow, int maxCol)
        {
            var topLeft = A1FromRowCol(minRow, minCol);
            var bottomRight = A1FromRowCol(maxRow, maxCol);
            return topLeft + ":" + bottomRight;
        }
    }
}
