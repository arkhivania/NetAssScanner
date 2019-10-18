using Nailhang.IndexBase.Storage;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Configuration;
using Nailhang.IndexBase.PublicApi;
using System.Text.RegularExpressions;

namespace Nailhang.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(
                new Nailhang.Mongodb.Module(),
                new Nailhang.Mongodb.ModuleDefault(),
                new Nailhang.Mongodb.ModulesStorage.Module(),
                new Nailhang.Mongodb.PublicStorage.Module(),
                new Nailhang.Processing.ModuleBuilder.Module(),
                new Nailhang.Processing.PublicExtract.Module(),
                new Nailhang.MVoxLease.AgarLease.Module()))
            {
                var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
                var config = builder.Build();
                kernel.Bind<IConfiguration>().ToConstant(config);

                var storage = kernel.Get<IModulesStorage>();
                var processor = kernel.Get<IndexBase.Index.IIndexProcessor>();
                var targetFiles = GetTargetFiles(Environment.GetCommandLineArgs());

                if (args[0] == "classes")
                {
                    var apiStorage = kernel.Get<IPublicApiStorage>();

                    if (HaveSwitch("drop"))
                    {
                        var dropped = apiStorage.Drop(new DropRequest { });
                        Console.WriteLine($"Dropped: {dropped} assemblies");
                    }

                    foreach(var dropP in GetParameters("drop"))
                    {
                        var dropped = apiStorage.Drop(new DropRequest { NameStartsWith = dropP });
                        Console.WriteLine($"Dropped: {dropped} assemblies");
                    }
                    
                    ClassesAssembliesPipeline(
                        apiStorage,
                        kernel.Get<IPublicProcessor>(), targetFiles);
                }
                else
                {
                    if (Environment.GetCommandLineArgs().Any(w => w.ToLower() == "-drop"))
                        storage.DropModules(namespaceStartsWith: "");

                    foreach (var drop in Environment.GetCommandLineArgs()
                        .Where(w => w.ToLower().StartsWith("-drop:")))
                        storage.DropModules(drop.Substring("-drop:".Length));

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

                    ModulesPipeLine(storage, processor, targetFiles);
                }
            }
        }

        static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
                            Replace("\\*", ".*").
                            Replace("\\?", ".") + "$";
        }

        static bool HaveSwitch(string name)
        {
            return HaveSwitch(Environment.GetCommandLineArgs(), name);
        }

        static bool HaveSwitch(string[] args, string name)
        {
            var ln = $"-{name.ToLower()}";
            return args.Any(w => w.ToLower() == ln);
        }

        static IEnumerable<string> GetParameters(string parameter)
        {
            return GetParameters(Environment.GetCommandLineArgs(), parameter);
        }

        static IEnumerable<string> GetParameters(string[] args, string parameter)
        {
            var prm = parameter.ToLower();
            var subL = $"-{prm}:".Length;
            return args
                .Where(w => w.ToLower().StartsWith($"-{prm}:"))
                .Select(w => w.Substring(subL));
        }

        private static List<string> GetTargetFiles(string[] args)
        {
            var targetFiles = new List<string>();

            var folderMask = GetParameters(args, "folderMask").SingleOrDefault();
            if (folderMask == null)
                folderMask = "*.dll";

            foreach (var envParam in args)
            {
                if (envParam.ToLower().StartsWith("-folder:"))
                    targetFiles.AddRange(Directory.GetFiles(envParam.Substring("-folder:".Length), folderMask, SearchOption.AllDirectories));
                if (envParam.ToLower().StartsWith("-file:"))
                    targetFiles.Add(envParam.Substring("-file:".Length));
            }

            foreach (var envParam in args)
            {
                if(envParam.ToLower().StartsWith("-ignore:"))
                {
                    var param = envParam.Substring("-ignore:".Length);
                    var reg = new Regex(WildcardToRegex(param));
                    for(int i = 0; i < targetFiles.Count; ++i)
                    {
                        if (reg.IsMatch(targetFiles[i]))
                        {
                            targetFiles.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            return targetFiles;
        }

        private static void ClassesAssembliesPipeline(IPublicApiStorage publicApiStorage, IPublicProcessor publicProcessor, List<string> targetFiles)
        {
            int index = 0;
            foreach (var fileName in targetFiles)
            {
                (AssemblyPublic, Class[])[] assemblies = null;
                try
                {
                    assemblies = publicProcessor.Extract(fileName).ToArray();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error processing: {fileName}: {e}");
                    index++;
                    continue;
                }

                publicApiStorage.UpdateAssemblies(assemblies);
                Console.WriteLine($"{index:D3}/{targetFiles.Count:D3} {fileName} processed.");
                index++;
            }
        }

        private static void ModulesPipeLine(IModulesStorage storage, IndexBase.Index.IIndexProcessor processor, List<string> targetFiles)
        {
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
            storeBlock.Completion
                .ConfigureAwait(false).GetAwaiter().GetResult();

            sw.Stop();

            Console.WriteLine(string.Format("Complete: {0} ms", sw.ElapsedMilliseconds));
            Console.WriteLine("Modules stored:" + stored);
        }
    }
}
