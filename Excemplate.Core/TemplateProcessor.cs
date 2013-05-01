using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

namespace Excemplate.Core
{
    public delegate object ProcessFunctionDelegate(string functionName, Dictionary<string, object> args);

    public class TemplateProcessor
    {

        //****************** Public Properties ********************//
        public ProcessFunctionDelegate FunctionHandler { get; set; }
        

        //****************** Constructor ********************//
        public TemplateProcessor() {
        }

        public TemplateProcessor(ProcessFunctionDelegate functionHandler) {
            FunctionHandler = functionHandler;
        }

        //****************** Public Methods ********************//
        public void Process(Excel.Worksheet sheet)
        {
        }

        public void Process(Excel.Workbook workbook)
        {
        }

        public void Process(Excel.Range range)
        {
        }

        public void ProcessFile(string templateFile, string outFileName)
        {
        }
    }
}
