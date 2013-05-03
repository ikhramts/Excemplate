using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Excemplate.Tests.TestingUtils
{
    class ReflectionUtils
    {
        public static string GetTestAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string GetTestFilePath(string relativePath)
        {
            return Path.Combine(GetTestAssemblyDirectory(), relativePath);
        }
    }
}
