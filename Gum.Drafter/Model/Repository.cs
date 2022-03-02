using System;
using Newtonsoft.Json.Linq;

namespace Gum.Drafter.Model
{
    [Serializable]
    public class Repository
    {
        public string Name { get; }

        public JObject Source { get; }

        public Repository(string name, JObject source)
        {
            Name = name;
            Source = source;
            Source["repositoryName"] = Name;
        }

        public override string ToString() => Name;
    }
}