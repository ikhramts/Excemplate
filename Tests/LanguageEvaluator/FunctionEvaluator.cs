using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excemplate.Tests.LanguageEvaluator
{
    public class FunctionEvaluator
    {
        public static object EvaluateFunction(string name, Dictionary<string, object> args)
        {
            switch (name)
            {
                case "Add":
                    return Convert.ToDouble(args["first"]) + Convert.ToDouble(args["second"]);

                case "GetFour": return 4;

                case "MultiplyByThree":
                    return Convert.ToDouble(args["val"]) * 3;

                default:
                    throw new Exception("Unknown function \"" + name + "\"");
            }
        }
    }
}
