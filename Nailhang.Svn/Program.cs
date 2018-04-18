using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Nailhang.IndexBase.History.Base;
using Nailhang.Svn.SvnProcessor.Base;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nailhang.Svn
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
            var config = builder.Build();

            bool haveArg(string argName)
            {
                return args.Contains($"/{argName}", StringComparer.InvariantCultureIgnoreCase);
            }

            string arg(string name)
            {
                return (from q in args
                        where q.StartsWith($"/{name}:")
                        select q.Substring($"/{name}:".Length)).FirstOrDefault();
            }

            int? numArg(string argName)
            {
                var p = arg(argName);
                return p != null ? int.Parse(p) : (int?)null;
            }

            using (var kernel = new StandardKernel(
                new Nailhang.Mongodb.Module(),
                new SvnProcessor.Module()))
            {
                kernel.Bind<IConfiguration>().ToConstant(config);

                var hs = kernel.Get<IHistoryStorage>();
                if (haveArg("dropHistory"))
                    hs.DropHistory();

                if (args.Contains("/reindex", StringComparer.InvariantCultureIgnoreCase))
                {
                    var sec = config.GetSection("repositories");

                    foreach (var rep in sec.Get<string[]>())
                    {
                        using (var svnConnection = kernel.Get<SvnProcessor.Base.ISvn>().Connect(rep))
                        {
                            void processChanges(int revision)
                            {
                                var rev = svnConnection.GetRevision(revision);
                                Console.WriteLine("Processing revision:" + new { rev.Number, rev.User, rev.UtcDateTime });

                                foreach (var c in svnConnection.GetChanges(revision))
                                {
                                    if (c.Path.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (c.ChangeType == ChangeType.Added ||
                                            c.ChangeType == ChangeType.Modify)
                                        {
                                            var modification = c.ChangeType == ChangeType.Modify ? Modification.Modification : Modification.Add;

                                            try
                                            {
                                                var content = svnConnection.Content(c.Path, c.Revision);
                                                var namespace_matches = Regex.Matches(content, @"\bnamespace\s+(?<namespace>(\w+\.)+\w+)");
                                                for (int i = 0; i < namespace_matches.Count; ++i)
                                                {
                                                    var ns = namespace_matches[i].Groups["namespace"].Value;
                                                    hs.StoreChangeToNamespace(ns, new IndexBase.History.Base.Change
                                                    {
                                                        Revision = new IndexBase.History.Base.Revision { UtcDateTime = rev.UtcDateTime, Id = rev.Number, User = rev.User },
                                                        Modification = modification
                                                    });
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                            }
                                        }
                                    }
                                }
                            }

                            var count = numArg("count");
                            

                            var revisionParameter = numArg("revision");
                            if (revisionParameter != null)
                                processChanges(revisionParameter.Value);
                            else
                                foreach (var v in svnConnection.LastRevisions(count ?? int.MaxValue))
                                {
                                    processChanges(v.Number);
                                }
                        }
                    }
                }
            }
        }
    }
}
