﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Svn.SvnProcessor.Base
{
    public interface ISvnConnection : IDisposable
    {
        IEnumerable<Revision> LastRevisions(int count);
        IEnumerable<Change> GetChanges(int revision);
        string Content(string path, int revision);
    }
}
