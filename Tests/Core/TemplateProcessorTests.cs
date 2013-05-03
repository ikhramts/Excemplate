using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

using NUnit.Framework;

using Excemplate.Core;
using Excemplate.Core.ExcelUtils;
using Excemplate.Tests.TestingUtils;

namespace Excemplate.Tests.Core
{
    [TestFixture]
    public class TemplateProcessorTests : BaseTemplateTestFixture
    {
        private const string TEST_TEMPLATE = @"Core\Test Template.xlsm";
        private const string OUT_WORKBOOK = @"Core\Output.xlsm";

        public TemplateProcessorTests()
            : base(TEST_TEMPLATE)
        {
        }

        [Test]
        public void CheckWorkbookAccess()
        {
            Excel.Worksheet basicTestsSheet = ExcelManager.ExcelInstance.ActiveWorkbook.Sheets["Basic Tests"];
            var passed = (bool)basicTestsSheet.get_Range("Passed").Value;
            Assert.AreEqual(false, passed);
        }

        [Test]
        public void ProcessRangeTest()
        {
            Excel.Worksheet basicTestsSheet = ExcelManager.ExcelInstance.ActiveWorkbook.Sheets["Basic Tests"];
            Excel.Range fullRange = basicTestsSheet.get_Range("B4:H21");
            Processor.Process(fullRange);
            AssertWorksheetProcessed(basicTestsSheet);
        }

        [Test]
        public void ProcessSheetTest()
        {
            Excel.Worksheet basicTestsSheet = ExcelManager.ExcelInstance.ActiveWorkbook.Sheets["Basic Tests"];
            Processor.Process(basicTestsSheet);
            AssertWorksheetProcessed(basicTestsSheet);
        }

        [Test]
        public void ProcessWorkbookTests()
        {
            Excel.Workbook testWorkbook = ExcelManager.ExcelInstance.ActiveWorkbook;
            Processor.Process(testWorkbook);
            AssertWorkbookProcessed(testWorkbook);
        }

        [Test]
        public void ProcessWorkbookAndSaveResult()
        {
            var inWorkbook = ReflectionUtils.GetTestFilePath(TEST_TEMPLATE);
            var outWorkbook = ReflectionUtils.GetTestFilePath(OUT_WORKBOOK);

            Processor.ProcessFile(inWorkbook, outWorkbook);

            var excelManager = ExcelManager.StartInstance();

            try
            {
                var resultWorkbook = excelManager.OpenWorkbook(outWorkbook);
                AssertWorkbookProcessed(resultWorkbook);
            }
            finally
            {
                excelManager.Stop();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DeleteVariablesTest(bool deleteVariables)
        {
            // Create and delete a variable.
            Processor.Process(ExcelManager.ExcelInstance.ActiveWorkbook);

            if (deleteVariables)
            {
                Processor.DeleteVariables();
            }

            // Check whether it was deleted
            var excelManager = ExcelManager.StartInstance();

            try
            {
                var outWorkbook = ReflectionUtils.GetTestFilePath(@"Core\Cross Sheet Template.xlsx");
                var resultWorkbook = excelManager.OpenWorkbook(outWorkbook);
                Processor.Process(resultWorkbook);

                if (deleteVariables)
                {
                    Excel.Worksheet activeSheet = resultWorkbook.ActiveSheet;
                    var result = activeSheet.get_Range("Passed").Value;
                    Assert.AreEqual(false, result); 
                }
                else
                {
                    AssertWorkbookProcessed(resultWorkbook);
                }
            }
            finally
            {
                excelManager.Stop();
            }
        }

        //********************* Helper Functions **********************//
        private void AssertWorksheetProcessed(Excel.Worksheet sheet) {
            var passed = (bool) sheet.get_Range("Passed").Value;
            Assert.AreEqual(true, passed);
        }

        private void AssertWorkbookProcessed(Excel.Workbook workbook)
        {
            foreach (Excel.Worksheet sheet in workbook.Sheets) {
                AssertWorksheetProcessed(sheet);
            }
        }
        
    }
}
