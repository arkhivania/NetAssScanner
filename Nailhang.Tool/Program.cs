using Nailhang.IndexBase.Storage;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
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

                if(Environment.GetCommandLineArgs().Any(w => w.ToLower() == "-drop"))
                    storage.DropModules("");

                foreach(var drop in Environment.GetCommandLineArgs().Where(w => w.ToLower().StartsWith("-drop:")))
                    storage.DropModules(drop.Substring("-drop:".Length));

                foreach (var envParam in Environment.GetCommandLineArgs())
                {
                    

                    if(envParam.ToLower().StartsWith("-folder:"))
                        foreach (var fp in Directory.GetFiles(envParam.Substring("-folder:".Length), "*.dll", SearchOption.AllDirectories))
                        {
                            Console.WriteLine("Extracting:" + fp);

                            try
                            {
                                foreach (var module in processor.ExtractModules(fp))
                                    storage.StoreModule(module);
                            }
                            catch(Exception e) 
                            {
                                Console.WriteLine("Fail to extract from: " + fp + Environment.NewLine + e.ToString());
                            }
                        }
                }
            }
        }

        private static void PrintModule(IndexBase.Module module)
        {
            Console.WriteLine(new {module.Assembly, module.FullName, module.Description});
        }
    }
}
