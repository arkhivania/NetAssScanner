using Nailhang.IndexBase.Index;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.Tests.CecilProcessing
{
    [TestFixture]
    class ParseAssemblies
    {
        [Test]
        [TestCase(@".\Nailhang.Processing.dll")]
        public void Parse(string path)
        {
            var curD = System.IO.Directory.GetCurrentDirectory();

            var kc = new KernelConfiguration();
            kc.Load<Nailhang.Processing.CecilModule>();
            using (var kernel = kc.BuildReadonlyKernel())
            {
                var modules = kernel.Get<IIndexProcessor>()
                    .ExtractModules(path)
                    .ToArray();
                Assert.IsNotEmpty(modules);
            }
        }
    }
}
