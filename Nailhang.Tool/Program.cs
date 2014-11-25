using Nailhang.IndexBase.Storage;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var kernel = new StandardKernel(
                new Nailhang.Mongodb.Module(),
                new Nailhang.Processing.CecilModule()))
            {
                var storage = kernel.Get<IModulesStorage>();
                var processor = kernel.Get<IndexBase.Index.IIndexProcessor>();

                foreach (var module in storage.GetModules())
                    PrintModule(module);

                foreach(var module in processor.ExtractModules(@"E:\Development\dev3\MultiVox\References\Release\Alda.MultiVox.Mode3D.Extensions.dll"))
                    storage.StoreModule(module);
            }
        }

        private static void PrintModule(IndexBase.Module module)
        {
            Console.WriteLine(new {module.Assembly, module.FullName, module.Description});
        }
    }
}
