using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Excemplate.Tests.TestingUtils
{
    class ProcessUtils
    {
        public static void AssertKilled(int processId, int timeoutMs = 1000)
        {
            var isRunning = false;

            try
            {
                var process = Process.GetProcessById((int)processId);

                // It may be taking some time to kill the process.
                try
                {
                    isRunning = !process.WaitForExit(timeoutMs);

                }
                catch (SystemException)
                {
                    isRunning = false;
                }
            }
            catch (ArgumentException)
            {
                isRunning = false;

            }
            catch (Exception)
            {
                isRunning = true;
            }

            Assert.AreEqual(false, isRunning);
        }
    }
}
