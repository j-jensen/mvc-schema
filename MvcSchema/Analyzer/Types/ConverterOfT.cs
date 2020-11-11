using System;

namespace MvcSchema.Analyzer.Types
{
    class Converter<T> : ITypeConverter
    {
        private readonly TypeKind _kind;
        public Converter(TypeKind kind)
        {
            _kind = kind;
        }
        public Type TypeToConvert => typeof(T);

        public IType Convert()
        {
            return new NamedType
            {
                Name = TypeToConvert.FullName,
                Kind = _kind,
            };
        }
    }
}
