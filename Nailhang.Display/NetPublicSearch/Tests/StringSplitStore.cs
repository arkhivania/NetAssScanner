using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Tests
{
    [TestFixture]
    class StringSplitStore
    {
        [Test]
        public void Run()
        {
            var sss = new Processing.SplitStringStore();
            
            var rowsStore = sss.Store("Alda.MultiVox.Rows");
            var columnsStore = sss.Store("Alda.MultiVox.Columns");

            Assert.That(rowsStore.Parent == columnsStore.Parent);

            Assert.AreEqual("Alda.MultiVox.Rows", rowsStore.ToString());
            Assert.AreEqual("Alda.MultiVox.Columns", columnsStore.ToString());

            foreach (var testWords in new[] { "", ".", "...", 
                "abc", "abc", 
                "abc.abc", "abc.abc",
                "ddd", "QQQ", "QQQ.FFF" })
            {
                var r = sss.Store(testWords);
                Assert.AreEqual(testWords, r.ToString());
            }
        }
    }
}
