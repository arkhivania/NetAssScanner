﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Services.Interfaces.History
{
    public struct Change
    {
        public Revision Revision { get; set; }
        public Modification Modification { get; set; }
    }
}
