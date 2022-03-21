using System;
using System.Collections.Generic;

namespace Gum.Drafter.Model
{
    [Serializable]
    public class Entity
    {
        public string name;

        public int id;
        
        public List<Aspect> aspects = new();
    }
}