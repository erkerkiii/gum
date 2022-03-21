using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gum.Drafter.Model
{
    [Serializable]
    public class Aspect
    {
        public string name;
        
        public Dictionary<string, object> properties = new();

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}