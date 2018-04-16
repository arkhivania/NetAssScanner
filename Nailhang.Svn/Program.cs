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
            

            var psStatInfo = new ProcessStartInfo
            {
                FileName = "svn",
                Arguments = "log https://svn.multivox.ru:8443/svn/dev3 -l 100",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true, 
            };

            if (codePage != null)
                psStatInfo.StandardOutputEncoding = Encoding.GetEncoding(codePage.Value);

            var svnLog = Process.Start(psStatInfo);
            
            var output = svnLog.StandardOutput.ReadToEnd();
            svnLog.WaitForExit();
            Console.WriteLine("Hello World!");
        }
    }
}
