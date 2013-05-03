using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

using NUnit.Framework;

using Excemplate.Core.ExcelUtils;
using Excemplate.Tests.TestingUtils;

namespace Excemplate.Tests.Core.ExcelUtils
{
    [TestFixture]
    public class ExcelManagerTests
    {
        [Test]
        public void DontStartExcelByDefault()
        {
            var manager = new ExcelManager();
            Assert.AreEqual(null, manager.ExcelInstance);
        }

        [Test]
        public void StartAndKillExcel()
        {
            var manager = ExcelManager.StartInstance();
            Assert.AreNotEqual(null, manager.ExcelInstance);

            KillExcelAndAssertKilled(manager);
        }

        [Test]
        public void TryKillingAfterModifyingWorkbook()
        {
            var manager = ExcelManager.StartInstance();
            var excel = manager.ExcelInstance;
            var workbook = excel.Workbooks.Add();
            Excel.Worksheet sheet = workbook.ActiveSheet;
            sheet.Cells[2, 3] = "When you create a workbook programmatically,";

            KillExcelAndAssertKilled(manager);
        }

        [Test]
        public void StartNotVisibleByDefault()
        {
            var manager = ExcelManager.StartInstance();
            Assert.AreEqual(false, manager.ExcelInstance.Visible);

            KillExcelAndAssertKilled(manager);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void StartWithVisibilityOption(bool isVisisble)
        {
            var manager = ExcelManager.StartInstance(isVisisble);
            Assert.AreEqual(isVisisble, manager.ExcelInstance.Visible);

            KillExcelAndAssertKilled(manager);
        }

        [Test]
        public void StartExcelOnOpenWorkbook()
        {
            var manager = new ExcelManager();
            var path = ReflectionUtils.GetTestFilePath(@"Core\ExcelUtils\Test Workbook.xlsx");
            manager.OpenWorkbook(path);

            Assert.AreNotEqual(null, manager.ExcelInstance);
            Excel.Workbook activeWorkbook = manager.ExcelInstance.ActiveWorkbook;
            Excel.Worksheet activeSheet = activeWorkbook.ActiveSheet;
            Assert.AreEqual("1224754", activeSheet.Name);

            KillExcelAndAssertKilled(manager);
        }

        //****************** Private Helper  ********************//
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public void KillExcelAndAssertKilled(ExcelManager manager)
        {
            var processId = manager.GetProcessId();
            Assert.AreNotEqual(0, processId);
            manager.Stop();

            ProcessUtils.AssertKilled(processId);
            Assert.AreEqual(null, manager.ExcelInstance);
        }
    }
}
