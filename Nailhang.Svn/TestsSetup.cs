using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Svn
{
    [SetUpFixture]
    class TestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
