using Nailhang.IndexBase.History.Base;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Mongodb.History.Tests
{
    [TestFixture]
    class StoreHistory
    {
        [Test]
        [TestCase("mongodb://192.168.0.32", "f_tests")]
        public void Store(string cs, string dbName)
        {
            using (var kernel = new StandardKernel(new Module()))
            {
                kernel.Rebind<MongoConnection>().ToConstant(
                    new MongoConnection { ConnectionString = cs, DbName = dbName });
                var hs = kernel.Get<IHistoryStorage>();

                hs.DropHistory();
                hs.StoreChangeToNamespace("AA.BB", new Change { Revision = new Revision { UtcDateTime = DateTime.Now, Id = 0, User = "test_user" } });
                hs.StoreChangeToNamespace("AA.BB", new Change { Revision = new Revision { UtcDateTime = DateTime.Now, Id = 1, User = "test_user" } });
                hs.StoreChangeToNamespace("QQ.BB", new Change { Revision = new Revision { UtcDateTime = DateTime.Now, Id = 1, User = "test_user" } });

                {
                    var changes = hs.GetChanges("AA").ToArray();
                    Assert.That(changes.Length == 2);
                    Assert.That(changes.Select(q => q.Revision.Id).Distinct().Count() == 2);
                }

                {
                    var changes = hs.GetChanges("QQ.BB").ToArray();
                    Assert.That(changes.Length == 1);
                }

                {
                    var changes = hs.GetChanges("QQ").ToArray();
                    Assert.That(changes.Length == 1);
                }
            }
        }
    }
}
