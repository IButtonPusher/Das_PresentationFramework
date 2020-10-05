using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Extended;

namespace _3DF5Tests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var lodr = new CoreFbxLoader();
            var fi = new FileInfo(
                @"E:\src\master_clones\ThirdParty\FbxWriter-master\FbxTest\bin\Debug\cube.fbx");

            var sw = new Stopwatch();
            sw.Start();

            var res = await lodr.LoadModelAsync(fi);

            Debug.WriteLine("loaded in " + sw.ElapsedMilliseconds);

        }
    }
}
