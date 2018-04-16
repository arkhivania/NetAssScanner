using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Nailhang.Svn.SvnProcessor.Base;

namespace Nailhang.Svn.SvnProcessor.Processing
{
    class SvnConnection : Base.ISvnConnection
    {
        private readonly string url;
        private readonly Settings settings;

        readonly Encoding encoding = Encoding.Default;

        public SvnConnection(string url, Base.Settings settings)
        {
            this.url = url;
            this.settings = settings;

            if (settings.CodePage != null)
                encoding = Encoding.GetEncoding(settings.CodePage.Value);
        }

        public void Dispose()
        {
            
        }

        public IEnumerable<Revision> LastRevisions(int count)
        {
            var psStatInfo = new ProcessStartInfo
            {
                FileName = "svn",
                Arguments = $"log {url} -l {count}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                StandardOutputEncoding = encoding
            };

            var svnLog = Process.Start(psStatInfo);

            var output = svnLog.StandardOutput.ReadToEnd();
            svnLog.WaitForExit();

            if(svnLog.ExitCode != 0)
                throw new InvalidOperationException("SVN process exit failed");

            var matches = Regex.Matches(output, @"r(?<revision>\d*)\s.\s(?<user>\w*)\s.\s(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<hour>\d{2}):(?<minutes>\d{2}):(?<seconds>\d{2})\s\+(?<utc>\d{4})");
            for(int index = 0; index < matches.Count; ++index)
            {
                var m = matches[index];
                int number(string groupName)
                {
                    return int.Parse(m.Groups[groupName].Value, CultureInfo.InvariantCulture);
                }

                var dateTime = new DateTime(number("year"), number("month"), number("day"), number("hour"), number("minutes"), number("seconds"));
                var offs = m.Groups["utc"].Value;
                
                int utc_hours = int.Parse(offs.Substring(0, 2), NumberStyles.Any);
                int utc_minutes = int.Parse(offs.Substring(2, 2), NumberStyles.Any);
                dateTime = dateTime.AddHours(-utc_hours);
                dateTime = dateTime.AddMinutes(-utc_minutes);


                yield return new Revision
                {
                    User = m.Groups["user"].Value,
                    Number = int.Parse(m.Groups["revision"].Value, CultureInfo.InvariantCulture),
                    UtcDateTime = dateTime
                };
            }
        }
    }
}
