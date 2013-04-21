using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excemplate.Core.ExcelUtils
{
    class ExcelManager
    {
        //****************** Public Properties ********************//
        public Excel.Application ExcelInstance { get; private set; }

        //****************** Public Methods ********************//
        public Excel.Workbook OpenWorkbook(string fileName)
        {
            return ExcelInstance.Workbooks.Open(fileName);
        }

        /// <summary>
        /// Create a new instance of ExcelManager and start a managed Excel application.
        /// </summary>
        /// <returns></returns>
        public static ExcelManager StartInstance(bool isVisible = false)
        {
            var manager = new ExcelManager();
            manager.Start(isVisible);
            return manager;
        }

        /// <summary>
        /// Start a managed Excel application.
        /// </summary>
        public Excel.Application Start(bool isVisible = false)
        {
            if (ExcelInstance != null)
            {
                //Cannot start when one is already running.
                throw new ExcelManagerException("Excel instance is already running.");
            }

            ExcelInstance = new Excel.Application();
            ExcelInstance.DisplayAlerts = false;
            ExcelInstance.Visible = isVisible;

            return ExcelInstance;
        }

        /// <summary>
        /// Force stop Excel instance.
        /// </summary>
        public void Stop()
        {
            // Force close all worksheets.
            foreach (Excel.Workbook workbook in ExcelInstance.Workbooks)
            {
                workbook.Close(SaveChanges: false); 
            }

            KillProcessById(ExcelInstance.Hwnd);
        }

        //****************** Private Functions ********************//
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static void KillProcessById(int hwnd)
        {
            uint processId;
            GetWindowThreadProcessId((IntPtr)hwnd, out processId);
            if (processId == 0)
            {
                throw new ArgumentException("Cannot find process for hwnd " + hwnd.ToString());
            }

            Process.GetProcessById((int)processId).Kill();
        }
    }
}
