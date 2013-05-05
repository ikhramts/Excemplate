using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Excemplate.Core.ExcelUtils;

namespace Excemplate.Tests.Core.ExcelUtils
{
    public class DataConverterTests
    {
        [Test]
        // Correct handling of general dates
        [TestCase("1949-04-03T07:26:24", 17991.31)]
        [TestCase("2117-03-18T11:02:24", 79336.46)]
        [TestCase("1973-08-02T15:21:36", 26878.64)]
        [TestCase("2032-12-06T05:02:24", 48554.21)]
        [TestCase("1999-03-23T03:36:00", 36242.15)]
        [TestCase("2026-06-08T07:40:48", 46181.32)]
        [TestCase("2006-06-25T05:31:12", 38893.23)]
        [TestCase("1908-01-26T11:31:12", 2948.48)]
        [TestCase("2054-02-02T15:36:00", 56282.65)]
        [TestCase("1995-10-02T16:04:48", 34974.67)]
        [TestCase("2076-05-24T17:45:36", 64429.74)]

        // Correct handling of intentional Lotus 1-2-3 compatibility bug.
        [TestCase("1900-01-01T00:00:00", 1.0)]
        [TestCase("1900-02-28T00:00:00", 59.0)]
        public void ConvertDateToExcel(string dateString, double excelDate)
        {
            var date = DateTime.Parse(dateString);
            var result = DataConverter.ExcelFromDotNet(date);
            Assert.AreEqual(excelDate, result);
        }

        [Test]
        [TestCase(1, 1, "A1")]
        [TestCase(7, 12, "L7")]
        [TestCase(10223, 9, "I10223")]
        [TestCase(7, 27, "AA7")]
        [TestCase(7, 218, "HJ7")]
        [TestCase(7, 702, "ZZ7")]
        [TestCase(7, 703, "AAA7")]
        [TestCase(7, 2795, "DCM7")]
        public void A1FromRowColTests(int row, int col, string expected)
        {
            var result = DataConverter.A1FromRowCol(row, col);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(1, 1, 1, 1, "A1:A1")]
        [TestCase(2, 3, 7, 2795, "C2:DCM7")]
        public void A1FromRectangleTests(int minRow, int minCol, int maxRow, int maxCol, string expected)
        {
            var result = DataConverter.A1FromRectangle(minRow, minCol, maxRow, maxCol);
            Assert.AreEqual(expected, result);
        }
    }
}
