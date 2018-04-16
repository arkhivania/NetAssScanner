using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Text;

namespace Nailhang.Svn
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
            var config = builder.Build();
            int? codePage = null;
            var pageSettings = config["codePage"];            
            if (!string.IsNullOrEmpty(pageSettings))
                codePage = int.Parse(pageSettings);
        }
    }
}
