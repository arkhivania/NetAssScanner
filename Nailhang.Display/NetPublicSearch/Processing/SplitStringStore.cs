using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Processing
{
    class SplitStringStore
    {
        public class Item
        {
            public Item(Item parent, string part)
            {
                Parent = parent;
                this.Part = part;
            }
            public Item Parent { get; }
            public string Part { get; }
            public Dictionary<string, Item> Childs { get; } = new Dictionary<string, Item>();

            public IEnumerable<Item> Sequence
            {
                get
                {
                    var cur = this;
                    while (cur != null)
                    {
                        yield return cur;
                        cur = cur.Parent;
                    }
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                var cur = this;
                while (cur != null)
                {
                    sb.Insert(0, "." + cur.Part);
                    cur = cur.Parent;
                }

                if (sb.Length == 0)
                    return "";

                sb.Remove(0, 1);
                return sb.ToString();
            }
        }

        readonly Dictionary<string, Item> roots = new Dictionary<string, Item>();

        public Item Store(string name)
        {
            var split = name.Split('.');
            var rootString = split.Length > 0 ? split[0] : "";

            Item result;
            if (!roots.TryGetValue(rootString, out result))
                roots[rootString] = result = new Item(null, rootString);

            for (int i = 1; i < split.Length; ++i)
            {
                Item item;
                var part = split[i];
                if (!result.Childs.TryGetValue(part, out item))
                {
                    item = new Item(result, part);
                    result.Childs[part] = item;
                }

                result = item;
            }

            return result;
        }
    }
}
