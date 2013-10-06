using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Excemplate.Language;

namespace Excemplate.Tests.LanguageEvaluator
{
    [TestFixture]
    public class LanguageEvaluatorTests
    {
        //TODO: Add tests for exceptions.

        private Evaluator evaluator;

        [SetUp]
        public void SetUp()
        {
            evaluator = new Evaluator();
            evaluator.DefaultHandler = FunctionEvaluator.EvaluateFunction;
            evaluator.SetVariable("testString1", "testing");
            evaluator.SetVariable("testInt_1", 54);
            evaluator.SetVariable("testDouble", 554.32);
        }

        [TearDown]
        public void TearDown()
        {
            evaluator = null;
        }

        //*******************************************
        [Test]
        // Literals
        [TestCase("0", 0)]
        [TestCase("\"\"", "")]
        [TestCase("\"hello\"", "hello")]
        [TestCase("12.3", 12.3)]

        // Variables
        [TestCase("testString1", "testing")]
        [TestCase("testInt_1", 54)]
        [TestCase("testDouble", 554.32)]

        // Functions
        [TestCase("GetFour()", 4)]
        [TestCase("\tGetFour\t(\t)\t", 4)]
        [TestCase("MultiplyByThree(val=6)", 18)]
        [TestCase("Add(first=2, second=2.5)", 4.5)]

        // Composite functions
        [TestCase("MultiplyByThree(val=Add(first=GetFour(), second=2.5))", 19.5)]
        [TestCase("MultiplyByThree \t(val =Add(first=GetFour() ,second= 2.5) ) ", 19.5)]

        // Dates
        [TestCase("Month(date= 2012-05-05T12:31 )", 5)]
        [TestCase("MultiplyByThree \t(val =Month(date= 2012-07-05T12:31\t ) ) ", 21)]

        // Assignments should return null.
        [TestCase("var=0", null)]
        [TestCase("var=\"hello\"", null)]
        [TestCase("var =12.3", null)]
        [TestCase("var= testString1", null)]
        [TestCase("var=\ttestInt_1", null)]
        [TestCase("var=testDouble", null)]
        [TestCase("var\t = GetFour()", null)]
        [TestCase(" var = GetFour ( ) ", null)]
        [TestCase("var=MultiplyByThree( val=6)", null)]
        [TestCase("var=Add(first=2, second=2.5 ) ", null)]
        [TestCase("var=MultiplyByThree(val= Add(first =GetFour () , second\t= 2.5))", null)]
        public void EvaluateExpression(string statement, object expected)
        {
            var result = evaluator.Evaluate(statement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ObjectListTest()
        {
            var result = evaluator.Evaluate("GetObjectList()");
            Assert.AreEqual(FunctionEvaluator.ObjectList, result);
            Assert.AreNotEqual(FunctionEvaluator.ObjectList.ToArray().GetType(), result.GetType());
        }

        [Test]
        [TestCase("2013-05-05")]
        [TestCase("2013-05-05T23:01")]
        [TestCase("2013-05-05T23:01:52")]
        [TestCase("2013-05-05T23:01:52.635")]
        [TestCase("1602-02-12")]
        [TestCase("2012-02-29")]
        public void EvaluateDate(string expression)
        {
            var expected = DateTime.Parse(expression);
            var result = evaluator.Evaluate(expression);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("2013-25-21")]
        [TestCase("2013-30-01")]
        [TestCase("2013-02-29")]
        [TestCase("2013-02-28T25:01")]
        [TestCase("2013-02-28T24:01")]
        [TestCase("2013-02-28T23:61")]
        [TestCase("2013-02-28T23:12:78")]
        [ExpectedException(typeof(InvalidDateException))]
        public void InvalidDate(string expression)
        {
            evaluator.Evaluate(expression);
        }

        [Test]
        [TestCase("&", 0)]
        [TestCase("2 &", 2)]
        [TestCase("2&", 1)]
        [TestCase("var\\t = GetFour()", 3)]
        public void UnrecognizedInput(string expression, int expectedCharNum)
        {
            try
            {
                evaluator.Evaluate(expression);
                Assert.Fail("Expected an UnrecognizedInputException.");
            }
            catch (UnrecognizedInputException ex)
            {
                Assert.AreEqual(expectedCharNum, ex.CharNum);
                Assert.AreEqual(0, ex.LineNum);
            }
        }

        [Test]
        [TestCase("2013-05", 4, "-05")]
        [TestCase("var=MultiplyByThree( (=6)", 21, "(")]
        [TestCase("2013-05-05T23", 10, "T23")]
        [TestCase("2013-05-05T23:01:52.635Z", 23, "Z")]
        [TestCase("20132-01-30", 5, "-01")]
        [TestCase("2013-05-05 23:01", 11, "23")]
        [TestCase("2013-05-05 23:01:05", 11, "23")]
        public void UnexpectedToken(string expression, int expectedCharNum, string badToken)
        {
            try
            {
                evaluator.Evaluate(expression);
                Assert.Fail("Expected an UnexpectedTokenException.");
            }
            catch (UnexpectedTokenException ex)
            {
                Assert.AreEqual(badToken, ex.Token);
                Assert.AreEqual(expectedCharNum, ex.CharNum);
                Assert.AreEqual(0, ex.LineNum);
            }
        }

        [Test]
        public void CauseFunctionEvaluatorException()
        {
            try
            {
                evaluator.Evaluate("CauseException(first=5)");
                Assert.Fail("Expected a FunctionEvaluatorException.");
            }
            catch (FunctionEvaluatorException ex)
            {
                Assert.AreEqual("CauseException", ex.FunctionName);
                Assert.AreNotEqual(null, ex.FunctionArgs);
                Assert.AreEqual(5, ex.FunctionArgs["first"]);
                Assert.AreEqual(1, ex.FunctionArgs.Count);
            }
        }
        

        //*******************************************
        [Test]
        // Literals
        [TestCase("var=0", 0)]
        [TestCase(" var =0", 0)]
        [TestCase(" \tvar\t =\t0", 0)]
        [TestCase("var=\"\"", "")]
        [TestCase("var=\"hello\"", "hello")]
        [TestCase("var=12.3", 12.3)]

        // Variables
        [TestCase("var=testString1", "testing")]
        [TestCase("var=testInt_1", 54)]
        [TestCase("var=testDouble", 554.32)]
        
        // Functions
        [TestCase("var=GetFour()", 4)]
        [TestCase("var=MultiplyByThree(val=6)", 18)]
        [TestCase("var=\tMultiplyByThree(val=6)", 18)]
        [TestCase("var=Add(first=2, second=2.5)", 4.5)]

        // Composite functions
        [TestCase("var=MultiplyByThree(val=Add(first=GetFour(), second=2.5))", 19.5)]

        public void Assignment(string statement, object expected)
        {
            var evaluationResult = evaluator.Evaluate(statement);
            Assert.AreEqual(null, evaluationResult);

            var result = evaluator.Evaluate("var");
            Assert.AreEqual(expected, result);
        }
    }
}
