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
        private Evaluator evaluator;

        [SetUp]
        public void SetUp()
        {
            evaluator = new Evaluator(FunctionEvaluator.EvaluateFunction);
            evaluator.SetVariable("testString1", "testing");
            evaluator.SetVariable("testInt_1", 54);
            evaluator.SetVariable("testDouble", 554.32);
        }

        [TearDown]
        public void TearDown()
        {
            evaluator = null;
        }

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
        [TestCase("MultiplyByThree(val=6)", 18)]
        [TestCase("Add(first=2, second=2.5)", 4.5)]

        // Composite functions
        [TestCase("MultiplyByThree(val=Add(first=GetFour(), second=2.5))", 19.5)]

        // Assignments should return null.
        [TestCase("var=0", null)]
        [TestCase("var=\"hello\"", null)]
        [TestCase("var=12.3", null)]
        [TestCase("var=testString1", null)]
        [TestCase("var=testInt_1", null)]
        [TestCase("var=testDouble", null)]
        [TestCase("var=GetFour()", null)]
        [TestCase("var=MultiplyByThree(val=6)", null)]
        [TestCase("var=Add(first=2, second=2.5)", null)]
        [TestCase("var=MultiplyByThree(val=Add(first=GetFour(), second=2.5))", null)]
        public void EvaluateExpression(string statement, object expected)
        {
            var result = evaluator.Evaluate(statement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        // Literals
        [TestCase("var=0", 0)]
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
