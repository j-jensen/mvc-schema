using Microsoft.AspNetCore.Mvc.Abstractions;
using MvcSchema.Analyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer
{
    public class TypeParser
    {
        private readonly Dictionary<Type, ITypeConverter> _simpleConverters;
        private readonly Dictionary<string, ClassConverter> _classConverters;
        public TypeParser()
        {
            _classConverters = new Dictionary<string, ClassConverter>();
            _simpleConverters = new Dictionary<Type, ITypeConverter>();
            Add(new Converter<bool>(TypeKind.Boolean));
            Add(new Converter<Int16>(TypeKind.Number));
            Add(new Converter<Int32>(TypeKind.Number));
            Add(new Converter<Int64>(TypeKind.Number));
            Add(new Converter<Single>(TypeKind.Number));
            Add(new Converter<UInt16>(TypeKind.Number));
            Add(new Converter<UInt32>(TypeKind.Number));
            Add(new Converter<UInt64>(TypeKind.Number));
            Add(new Converter<Decimal>(TypeKind.Number));
            Add(new Converter<string>(TypeKind.String));
            Add(new Converter<Guid>(TypeKind.String));

            void Add(ITypeConverter converter) => _simpleConverters.Add(converter.TypeToConvert, converter);
        }

        public IEnumerable<IType> Types
        {
            get
            {
                return _classConverters.Select(c=>c.Value.Type);
            }
        }

        public Argument ParseParameter(ParameterDescriptor propertyDescriptor)
        {
            return new Argument
            {
                Name = propertyDescriptor.Name,
                Type = ParseType(propertyDescriptor.ParameterType)
            };
        }

        public IType ParseType(Type type)
        {
            if (type.IsValueType)
            {
                if (_simpleConverters.TryGetValue(type, out ITypeConverter converter))
                {
                    return converter.Convert();
                }
            }
            if (type.IsClass)
            {
                if (_classConverters.TryGetValue(type.FullName, out ClassConverter converter))
                {
                    return converter.Convert();
                }
                else
                {
                    var newConverter = new ClassConverter(type, this);
                    _classConverters.Add(type.FullName, newConverter);
                    return newConverter.Convert();
                }
            }

            return null;
        }
    }
}
