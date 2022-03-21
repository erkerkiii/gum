using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gum.Drafter.Model
{
    [Serializable]
    public class Repository
    {
        public string Name { get; }

        public List<Entity> entities = new();

        public Repository(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}