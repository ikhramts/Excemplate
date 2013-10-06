using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Excemplate.Core;
using Excemplate.Core.ExcelUtils;
using Excemplate.Tests.LanguageEvaluator;
using Excemplate.Tests.TestingUtils;

namespace Excemplate.Tests.Core
{
    /// <summary>
    /// A base for all test fixtures that require opening an Excel workbook.
    /// </summary>
    public class BaseTemplateTestFixture
    {
        protected string TemplatePath { get; set; }
        protected ExcelManager ExcelManager { get; set; }
        protected TemplateProcessor Processor { get; set; }

        public BaseTemplateTestFixture(string templateFileName)
        {

            TemplatePath = ReflectionUtils.GetTestFilePath(templateFileName);
        }

        [SetUp]
        public virtual void SetUp()
        {
            ExcelManager = new ExcelManager();
            ExcelManager.OpenWorkbook(TemplatePath);
            Processor = new TemplateProcessor();
            Processor.ExpressionEvaluator.DefaultHandler = FunctionEvaluator.EvaluateFunction;
        }

        [TearDown]
        public virtual void TearDown()
        {
            ExcelManager.Stop();
            ExcelManager = null;
            Processor = null;
        }
    }
}
