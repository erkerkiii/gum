using System;

namespace Gum.Composer.CodeGen
{
    public readonly struct TypePrototype
    {
        public readonly string TypeName;

        public readonly Type Type;

        public TypePrototype(string typeName, Type type)
        {
            TypeName = typeName;
            Type = type;
        }
    }
}