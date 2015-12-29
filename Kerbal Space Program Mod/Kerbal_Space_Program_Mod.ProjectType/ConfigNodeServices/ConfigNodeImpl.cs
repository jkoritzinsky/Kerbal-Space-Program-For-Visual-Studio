using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNodeServices
{
    public class ConfigNodeImpl
    {
        public string Name { get; set; }
        public MultiValueDictionary<string, string> NameValuePairs { get; } = new MultiValueDictionary<string, string>();
        public MultiValueDictionary<string, ConfigNodeImpl> NestedNodes { get; } = new MultiValueDictionary<string, ConfigNodeImpl>();

        internal static ConfigNodeImpl CreateFromPair(KeyValuePair<string, string> pair)
        {
            var node = new ConfigNodeImpl();
            node.NameValuePairs.Add(pair.Key, pair.Value);
            return node;
        }

        internal static ConfigNodeImpl MergeNodes(IList<ConfigNodeImpl> nodes, string name)
        {
            var impl = new ConfigNodeImpl { Name = name };
            foreach (var node in nodes)
            {
                foreach (var entries in node.NameValuePairs)
                {
                    impl.NameValuePairs.AddRange(entries.Key, entries.Value);
                }
                foreach (var entries in node.NestedNodes)
                {
                    impl.NestedNodes.AddRange(entries.Key, entries.Value);
                }
            }
            return impl;
        }
    }
}
