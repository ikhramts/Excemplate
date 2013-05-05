using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

using Excemplate.Core.ExcelUtils;
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
        //****************** Public Constants ********************//
        public const char COMMAND_CHAR = '|';

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
            ExpressionEvaluator.DeleteVariables();
        }

        public void Process(Excel.Range range)
        {

            var numRows = range.Rows.Count;
            var numColumns = range.Columns.Count;

            // Iterate over rows and columns, processing each cell individually.  We do not want
            // to grab all of the values in the range at once because the workbook may be 
            // recalculating the cells to the bottom or right of the cell we're updating.
            for (var row = 1; row <= numRows; row++)
            {
                for (var column = 1; column <= numColumns; column++)
                {
                    Excel.Range cell = range.Cells[row, column];
                    ProcessCell(cell);
                }
            }
        }

        public void Process(Excel.Worksheet sheet)
        {
            // Apply Process() to UsedRange.  But shouldn't process UsedRange directly
            // because it's a funky dynamic property.
            var usedRange = sheet.UsedRange;
            var startRow = usedRange.Row;
            var endRow = startRow + usedRange.Rows.Count - 1;
            var startColumn = usedRange.Column;
            var endColumn = startColumn + usedRange.Columns.Count - 1;

            var rangeAddress = DataConverter.A1FromRectangle(startRow, startColumn, endRow, endColumn);
            var rangeToProcess = sheet.Range[rangeAddress];
            Process(rangeToProcess);
        }

        public void Process(Excel.Workbook workbook)
        {
        }

        public void ProcessFile(string templateFile, string outFileName)
        {
        }

        //****************** Private Functions ********************//
        /// <summary>
        /// Process a range that is guaranteed to have only one cell.  This function 
        /// may affect cells to the right and bottom of the provided cell if the result
        /// is an array.
        /// </summary>
        /// <param name="cell"></param>
        private void ProcessCell(Excel.Range cell)
        {
            // Check if the cell has an Excemplate expression.
            object cellValue = cell.Value;

            if (cellValue == null || (cellValue.GetType() != typeof(string)))
            {
                return;
            }

            var stringValue = (string)cellValue;

            if (stringValue == "" || stringValue[0] != COMMAND_CHAR)
            {
                return;
            }

            // Process the expression.
            object result = null;

            try
            {
                result = ExpressionEvaluator.Evaluate(stringValue.Substring(1));
            }
            catch (Exception ex)
            {
                cell.Value = "Error: " + ex.Message;
                return;
            }

            // Start outputting the result.
            if (result == null)
            {
                cell.Value = null;
            }

            var resultType = result.GetType();

            // Convert all array-like types to arrays.
            if (!resultType.IsArray)
            {
                var toArrayMethod = resultType.GetMethod("ToArray", new Type[0] /* ToArray() without args */);

                if (toArrayMethod != null)
                {
                    result = toArrayMethod.Invoke(result, new object[0]);
                    resultType = result.GetType();
                }
            }

            // Output.
            if (resultType.IsArray)
            {
                var resultArray = (Array)result;

                if (resultArray.Rank == 1)
                {
                    // Output the single-dimentional array as a vertical vector.
                    var minRow = resultArray.GetLowerBound(0);
                    var maxRow = resultArray.GetUpperBound(0);

                    for (int row = minRow; row <= maxRow; row++)
                    {
                        cell.Offset[row - minRow].Value = PreProcessOutput(resultArray.GetValue(row));
                    }

                }
                else if (resultArray.Rank == 2)
                {
                    // Output first dimension along rows, second dimension along columns.
                    var minRow = resultArray.GetLowerBound(0);
                    var maxRow = resultArray.GetUpperBound(0);
                    var minColumn = resultArray.GetLowerBound(1);
                    var maxColumn = resultArray.GetUpperBound(1);

                    for (var row = minRow; row <= maxRow; row++)
                    {
                        for (var column = minColumn; column <= maxColumn; column++)
                        {
                            var offsetCell = cell.Offset[row - minRow, column - minColumn];
                            offsetCell.Value = PreProcessOutput(resultArray.GetValue(row, column));
                        }
                    }
                }
                else
                {
                    cell.Value = "Error: result array has more than 2 dimensions.";
                }
            }
            else
            {
                cell.Value = PreProcessOutput(result);
            }
        }

        private object PreProcessOutput(object value)
        {
            return DataConverter.ExcelFromDotNet(value);
        }
    }
}
