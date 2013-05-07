using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excemplate.Tests.LanguageEvaluator
{
    public class FunctionEvaluator
    {
        public static List<object> ObjectList = new List<object>() {
            "test", 2, DateTime.Parse("2013-05-06T08:22:00"),
        };

        public static List<string> StringList = new List<string> {
            "one", "two", "three", "turtle",
        };

        // First dimension represents rows, second dimension represents columns.
        public static int[,] IntArray2D = new int[2, 3] {
            {2, 5, 0},
            {5, -6, 12},
        };

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

                case "CauseException":
                    throw new ArgumentException("I object!");

                case "GetFour": return 4;

                case "GetIntArray2D":
                    return IntArray2D;

                case "GetObjectArray":
                    return ObjectList.ToArray();

                case "GetObjectList":
                    return ObjectList;

                case "GetStringArray":
                    return StringList.ToArray();

                case "GetStringList":
                    return StringList;

                case "Month":
                    return ((DateTime)args["date"]).Month;

                case "MultiplyByThree":
                    return Convert.ToDouble(args["val"]) * 3;

                default:
                    throw new Exception("Unknown function \"" + functionName + "\"");
            }
        }
    }
}
