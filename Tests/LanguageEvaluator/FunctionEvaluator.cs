using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excemplate.Tests.LanguageEvaluator
{
    public class FunctionEvaluator
    {
        /// <summary>
        /// A dummy function call handler for the tests.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object EvaluateFunction(string functionName, Dictionary<string, object> args)
        {
            switch (functionName)
            {
                case "Add":
                    return Convert.ToDouble(args["first"]) + Convert.ToDouble(args["second"]);

                case "GetFour": return 4;

                case "MultiplyByThree":
                    return Convert.ToDouble(args["val"]) * 3;

                default:
                    throw new Exception("Unknown function \"" + functionName + "\"");
            }
        }
    }
}
