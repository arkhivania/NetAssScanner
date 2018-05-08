using Microsoft.Extensions.DependencyInjection;
using Nailhang.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Services.ModulesMarks.Statistics
{
    public class Module
    {
        public void Load(IServiceCollection services)
        {
            services.AddTransient<IStartUp, StartUp>();
        }
    }
}
