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
            

            using (var kernel = new StandardKernel(
                new Nailhang.Mongodb.Module(),
                new SvnProcessor.Module()))
            {
                kernel.Bind<IConfiguration>().ToConstant(config);

                var hs = kernel.Get<IHistoryStorage>();

                if (args.Contains("/reindex", StringComparer.InvariantCultureIgnoreCase))
                {
                    hs.DropHistory();
                    var sec = config.GetSection("repositories");
                    
                    foreach (var rep in sec.Get<string[]>())
                    {
                        using (var svnConnection = kernel.Get<SvnProcessor.Base.ISvn>().Connect(rep))
                        {
                            foreach(var v in svnConnection.LastRevisions(int.MaxValue))
                            {
                                var updated = new HashSet<string>();
                                Console.WriteLine("Processing revision:" + new { v.Number, v.User, v.UtcDateTime });
                                foreach (var c in svnConnection.GetChanges(v.Number))
                                {   
                                    if (c.Path.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (c.ChangeType == ChangeType.Added ||
                                            c.ChangeType == ChangeType.Modify)
                                        {
                                            var content = svnConnection.Content(c.Path, c.Revision);
                                            var namespace_matches = Regex.Matches(content, @"\bnamespace\s+(?<namespace>(\w+\.)+\w+)");
                                            for(int i = 0; i < namespace_matches.Count; ++i)
                                            {
                                                var ns = namespace_matches[i].Groups["namespace"].Value;
                                                if (!updated.Contains(ns))
                                                {
                                                    hs.StoreChangeToNamespace(ns, new IndexBase.History.Base.Revision { UtcDateTime = v.UtcDateTime, Id = v.Number, User = v.User });
                                                    updated.Add(ns);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
