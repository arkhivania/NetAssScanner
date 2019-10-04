using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nailhang.Display.Tools.TextSearch.Base;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class StatBuilder : IWSBuilder
    {
        private static void NextWord(string word, Statistics res)
        {
            foreach (var c in word.ToLower())
            {
                if (!char.IsLetter(c))
                    continue;

                CharStat stat;
                if (!res.CharStats.TryGetValue(c, out stat))
                    res.CharStats[c] = stat = new CharStat(c);

                stat.Count++;
            }
        }

        public Statistics BuildStatistics(IEnumerable<string> lines)
        {
            var res = new Statistics();

            foreach (var name in lines)
                foreach (var s in name.Split(new char[] { ' ' }))
                    NextWord(s, res);

            res.Codes = res.CharStats
                .OrderByDescending(q => q.Value.Count)
                .Select(q => q.Value)
                .Take(255).ToArray();

            for (int i = 0; i < res.Codes.Length; ++i)
                res.ToCode[res.Codes[i].Char] = (byte)i;

            foreach (var i in res.Codes.Select(q => q.Char))
                res.Coded.Add(i);

            return res;
        }

        IStat IWSBuilder.BuildStat(IEnumerable<string> lines)
        {
            return BuildStatistics(lines);
        }

        public ISearch CreateSearch(IIndex index)
        {
            return new Search((Index)index);
        }

        public IIndex Index(IEnumerable<Bulk> bulks, IStat stat)
        {
            var index = new Index((Statistics)stat);
            foreach (var b in bulks)
                index.Run(b);
            return index;
        }
    }
}
