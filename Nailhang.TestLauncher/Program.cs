using System;
using System.IO;
using System.Linq;

namespace Nailhang.TestLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Please select assembly");
                return;
            }

            var absolute = Path.GetFullPath(args[0]);
            new NUnitLite.AutoRun(System.Reflection.Assembly.LoadFile(absolute))
                .Execute(args.Skip(1).ToArray());
        }
    }
}
