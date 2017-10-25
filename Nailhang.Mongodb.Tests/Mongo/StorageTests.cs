using Nailhang.IndexBase.Storage;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Nailhang.Mongodb.Tests
{
    [TestFixture]
    class StorageTests
    {
        [Test]
        public void DoIt()
        {
            using (var kernel = new Ninject.StandardKernel())
            {
                kernel.Load<Nailhang.Mongodb.Module>();
                kernel.Rebind<Nailhang.Mongodb.MongoConnection>()
                .ToConstant(new MongoConnection
                {
                    ConnectionString = "mongodb://192.168.0.32",
                    DbName = "nail_tests"
                });

            
                var ms = kernel.Get<IModulesStorage>();
                ms.StoreModule(new IndexBase.Module
                {
                    Assembly = "lala",
                    Description = "big lala",
                    FullName = "very big lala",
                    InterfaceDependencies = new IndexBase.TypeReference[] { },
                    Interfaces = new IndexBase.ModuleInterface[] { },
                    ModuleBinds = new IndexBase.TypeReference[] { },
                    ObjectDependencies = new IndexBase.TypeReference[] { },
                    Objects = new IndexBase.ModuleObject[] { },
                    Significance = Significance.High
                });

                Assert.IsNotEmpty(ms.GetModules().ToArray());
                ms.DropModules("NOTREALNAMESPACE");
                Assert.IsNotEmpty(ms.GetModules().ToArray());
                ms.DropModules("");
                Assert.IsEmpty(ms.GetModules().ToArray());
            }
        }
    }
}
