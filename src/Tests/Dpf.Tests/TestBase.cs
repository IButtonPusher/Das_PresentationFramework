using System;
using System.IO;
using System.Threading.Tasks;

namespace Dpf.Tests
{
    public abstract class TestBase
    {
        protected static String GetFileContents(String fileName)
        {
            var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Files",
                fileName);

            return File.ReadAllText(fullName);

        }
    }
}
