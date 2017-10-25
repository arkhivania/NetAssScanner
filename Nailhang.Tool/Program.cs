using Nailhang.IndexBase.Storage;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Configuration;

namespace Nailhang.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new Nailhang.Mongodb.Module(),
                new Nailhang.Processing.CecilModule()))
            {

                var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
                var config = builder.Build();
                kernel.Bind<IConfiguration>().ToConstant(config);


                var storage = kernel.Get<IModulesStorage>();
                var processor = kernel.Get<IndexBase.Index.IIndexProcessor>();

                if (Environment.GetCommandLineArgs().Any(w => w.ToLower() == "-drop"))
                    storage.DropModules(namespaceStartsWith: "");

                foreach (var drop in Environment.GetCommandLineArgs().Where(w => w.ToLower().StartsWith("-drop:")))
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

                var filesBlock = new BufferBlock<string>();
                var getModulesBlock = new TransformManyBlock<string, IndexBase.Module>(w =>
                    {
                        try
                        {
                            return processor.ExtractModules(w).ToArray();
                        }
                        catch (BadImageFormatException)
                        {
                            Console.WriteLine("Skip bad image: " + w + Environment.NewLine);
                            return Enumerable.Empty<IndexBase.Module>();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Fail to extract from: " + w + Environment.NewLine + e.ToString());
                            return Enumerable.Empty<IndexBase.Module>();
                        }
                    }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

                var broadcastBlock = new BroadcastBlock<IndexBase.Module>(w => w);
                var printModuleBlock = new ActionBlock<IndexBase.Module>(m => Console.WriteLine(string.Format("Store module: {0}", m.FullName)));

                int stored = 0;
                var storeBlock = new ActionBlock<IndexBase.Module>(m =>
                {
                    storage.StoreModule(m);
                    System.Threading.Interlocked.Increment(ref stored);
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, BoundedCapacity = 2000 });

                filesBlock.LinkTo(getModulesBlock, new DataflowLinkOptions { PropagateCompletion = true });
                getModulesBlock.LinkTo(broadcastBlock, new DataflowLinkOptions { PropagateCompletion = true });
                broadcastBlock.LinkTo(printModuleBlock, new DataflowLinkOptions { PropagateCompletion = true });
                broadcastBlock.LinkTo(storeBlock, new DataflowLinkOptions { PropagateCompletion = true });

                var esw = new Stopwatch();
                var sw = new Stopwatch();
                sw.Start();
                esw.Start();

                foreach (var fp in targetFiles)
                    filesBlock.Post(fp);

                getModulesBlock.Completion.ContinueWith(w =>
                    {
                        esw.Stop();
                        Console.WriteLine(string.Format("Modules extracting complete: {0} ms", esw.ElapsedMilliseconds));
                    });

                filesBlock.Complete();
                storeBlock.Completion.Wait();

                sw.Stop();

                Console.WriteLine(string.Format("Complete: {0} ms", sw.ElapsedMilliseconds));
                Console.WriteLine("Modules stored:" + stored);
            }
        }
    }
}
