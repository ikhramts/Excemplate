using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

using Excemplate.Language;

namespace Excemplate.Core
{
    public delegate object ProcessFunctionDelegate(string functionName, Dictionary<string, object> args);

    /// <summary>
    /// This class is responsible for executing Excemplate expressions in Excel cells.
    /// 
    /// By default, the TemplateProcessor will execute the expressions top to bottom and left to right.
    /// Any variables added to the execution context will be saved until the next execution unless 
    /// DeleteVariables() is called.
    /// </summary>
    public class TemplateProcessor
    {

        //****************** Public Properties ********************//
        public ProcessFunctionDelegate FunctionHandler
        {
            get
            {
                return new ProcessFunctionDelegate(ExpressionEvaluator.EvalFunc);
            }
            set
            {
                ExpressionEvaluator.EvalFunc = new FunctionCallHandlerDelegate(value);
            }
        }

        public Evaluator ExpressionEvaluator { get; private set; }

        //****************** Constructor ********************//
        public TemplateProcessor(ProcessFunctionDelegate functionHandler) {
            var evaluatorCallback = new FunctionCallHandlerDelegate(functionHandler);
            ExpressionEvaluator = new Evaluator(evaluatorCallback);
            FunctionHandler = functionHandler;
        }

        //****************** Public Methods ********************//
        public void DeleteVariables()
        {
        }

        public void Process(Excel.Range range)
        {
        }

        public void Process(Excel.Worksheet sheet)
        {
        }

        public void Process(Excel.Workbook workbook)
        {
        }

        public void ProcessFile(string templateFile, string outFileName)
        {
        }
    }
}
