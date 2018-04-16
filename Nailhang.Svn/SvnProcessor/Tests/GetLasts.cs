using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Svn.SvnProcessor.Tests
{
    [TestFixture]
    class GetLasts
    {
        [Test]
        [TestCase("https://192.168.0.4:8443/svn/dev3", 50, 1251)]
        public void GetLastsFromStorage(string url, int count, int codePage)
        {
            using (var kernel = new StandardKernel(new Module()))
            {
                kernel.Rebind<Base.Settings>().ToConstant(new Base.Settings
                {
                    CodePage = codePage
                });
                using (var connection = kernel.Get<Base.ISvn>().Connect(url))
                {
                    var revs = connection.LastRevisions(count).ToArray();
                    Assert.Greater(revs.Length, 0);
                }
            }
        }

        [Test]
        [TestCase("https://192.168.0.4:8443/svn/dev3", 20303)]
        public void GetChanges(string url, int revision)
        {
            using (var kernel = new StandardKernel(new Module()))
            {
                kernel.Rebind<Base.Settings>().ToConstant(new Base.Settings
                {
                    CodePage = 1251
                });
                using (var connection = kernel.Get<Base.ISvn>().Connect(url))
                {
                    var revs = connection.GetChanges(revision).ToArray();
                }
            }
        }
    }
}
