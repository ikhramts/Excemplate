using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excemplate.Core.ExcelUtils
{
    public class ExcelManager
    {
        //****************** Public Properties ********************//
        public Excel.Application ExcelInstance { get; private set; }

        //****************** Public Methods ********************//
        public int GetProcessId()
        {
            if (ExcelInstance == null)
            {
                throw new ExcelManagerException("Excel process has not been started.");
            }

            uint processId;
            GetWindowThreadProcessId((IntPtr)ExcelInstance.Hwnd, out processId);
            return (int)processId;
        }

        public Excel.Workbook OpenWorkbook(string fileName)
        {
            if (ExcelInstance == null)
            {
                Start();
            }

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
            if (ExcelInstance == null)
            {
                return;
            }

            // Force close all worksheets.
            foreach (Excel.Workbook workbook in ExcelInstance.Workbooks)
            {
                workbook.Close(SaveChanges: false); 
            }

            var processId = GetProcessId();

            if (processId != 0)
            {
                Process.GetProcessById((int)processId).Kill();
            }

            ExcelInstance = null;
        }

        //****************** Private Functions ********************//
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}
