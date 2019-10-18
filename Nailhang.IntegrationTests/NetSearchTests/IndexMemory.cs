using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.IntegrationTests.NetSearchTests
{
    [TestFixture]
    class IndexMemory
    {
        [Test]
        public void Run()
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Load<Nailhang.Mongodb.Module>();
                kernel.Load<Nailhang.Mongodb.PublicStorage.Module>();
                kernel.Load<TestModules.LocalMongo>();
                kernel.Load<Nailhang.Display.Tools.TextSearch.Module>();
                kernel.Load<Nailhang.Display.NetPublicSearch.Module>();

                var netSearch = kernel
                    .Get<Nailhang.Display.NetPublicSearch.Base.INetSearch>();

                var res = netSearch.Search("multivox vector3", 100).ToArray();
                Assert.That(res.Length > 0);
            }
        }
    }
}
