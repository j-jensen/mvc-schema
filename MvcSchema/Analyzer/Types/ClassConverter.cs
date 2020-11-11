using System;
using System.Linq;
using System.Reflection;

namespace MvcSchema.Analyzer.Types
{
    class ClassConverter : IConverter
    {
        Type _type;
        public ClassConverter(Type type, TypeParser typeParser)
        {
            _type = type;
            Type = new NamedType
            {
                Name = _type.FullName,
                Kind = TypeKind.Object,
                Fields = _type.GetFields(BindingFlags.Public)
                    .Select(pi => typeParser.ParseType(pi.FieldType))
                    .ToArray()
            };
        }

        public IType Type { get; private set; }

        public IType Convert()
        {
            return new NamedType
            {
                Name = _type.FullName,
                Kind = TypeKind.Object,
            };
        }

    }
}
