using Nailhang.IndexBase.Index;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.Tests.CecilProcessing
{
    [TestFixture]
    class ParseAssemblies
    {
        [Test]
        [TestCase(@".\Nailhang.Processing.dll")]
        [TestCase(@".\Nailhang.Mongodb.dll")]
        public void Parse(string path)
        {
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
