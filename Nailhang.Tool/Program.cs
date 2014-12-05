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

                var targetFiles = new List<string>();

                if (Environment.GetCommandLineArgs().Any(w => w.ToLower() == "-list"))
                {
                    Console.WriteLine("Stored modules:");
                    Console.WriteLine(Environment.NewLine);
                    foreach (var m in storage.GetModules())
                    {

                        Console.WriteLine(string.Format("Assembly: {0}", m.Assembly));
                        Console.WriteLine(string.Format("ModuleName: {0}", m.FullName));
                        Console.WriteLine(string.Format("Description: {0}", m.Description));
                        Console.WriteLine(Environment.NewLine);
                    }
                }

                foreach (var envParam in Environment.GetCommandLineArgs())
                {
                    if (envParam.ToLower().StartsWith("-folder:"))
                        targetFiles.AddRange(Directory.GetFiles(envParam.Substring("-folder:".Length), "*.dll", SearchOption.AllDirectories));
                    if (envParam.ToLower().StartsWith("-file:"))
                        targetFiles.Add(envParam.Substring("-file:".Length));
                }

                foreach(var fp in targetFiles)
                    {
                            Console.WriteLine("Extracting:" + fp);

                            try
                            {
                                foreach (var module in processor.ExtractModules(fp))
                                    storage.StoreModule(module);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Fail to extract from: " + fp + Environment.NewLine + e.ToString());
                            }
                        }

                
            }
        }
    }
}
