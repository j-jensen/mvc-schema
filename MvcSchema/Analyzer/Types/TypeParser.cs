using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer.Types
{
    public class TypeParser
    {
        private readonly Dictionary<Type, TypeDescriptor> _types;
        private readonly Dictionary<Type, TypeDescriptor> _typeDescriptors;
        public TypeParser()
        {
            _typeDescriptors = new Dictionary<Type, TypeDescriptor>();
            _types = new Dictionary<Type, TypeDescriptor>();
            // String is special - We want it to be a type string, not a type char[]
            _types.Add(typeof(string), new TypeDescriptor(typeof(string)));
        }

        public Property ParseProperty(PropertyInfo pi, TypeDescriptor owner)
        {
            return new Property
            {
                Name = pi.Name,
                Type = owner.ClrType == pi.PropertyType ? owner : ParseType(pi.PropertyType)
            };
        }

        public IEnumerable<TypeDescriptor> TypeDescriptors
        {
            get
            {
                return _typeDescriptors.Select(d => d.Value);
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

        public TypeDescriptor ParseType(Type clrType)
        {
            // Do we allready have the type?
            if (_types.TryGetValue(clrType, out TypeDescriptor type))
            {
                return type;
            }

            // Numbers, Char and bool
            if (clrType.IsPrimitive)
            {
                var primitiveType = new TypeDescriptor(clrType);
                _types.Add(clrType, primitiveType);
                return primitiveType;
            }

            // Nullable types
            var underlyingClrType = Nullable.GetUnderlyingType(clrType);
            if (underlyingClrType != null)
            {
                if (!_types.TryGetValue(underlyingClrType, out TypeDescriptor underlyingType))
                {
                    underlyingType = ParseType(underlyingClrType);
                }
                var nullable = new TypeDescriptor(underlyingClrType, Kind.Nullable);
                _types.Add(clrType, nullable);
                return nullable;
            }

            // Enums
            if (clrType.IsEnum)
            {
                _typeDescriptors.Add(clrType, new EnumDescriptor(clrType));
                var enumType = new TypeDescriptor(clrType, Kind.Enum);
                _types.Add(clrType, enumType);
                return enumType;
            }

            // Array
            if (clrType.IsArray)
            {
                var arrayType = clrType.GetElementType();
                if (!_types.TryGetValue(arrayType, out TypeDescriptor underlyingType))
                {
                    underlyingType = ParseType(arrayType);
                }
                var array = new TypeDescriptor(arrayType, Kind.Array);
                _types.Add(clrType, array);
                return array;
            }
            // Array like
            var collectionInterface = clrType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (collectionInterface != null)
            {
                var arrayType = collectionInterface.GetGenericArguments().First();
                if (!_types.TryGetValue(arrayType, out TypeDescriptor underlyingType))
                {
                    underlyingType = ParseType(arrayType);
                }
                var array = new TypeDescriptor(arrayType, Kind.Array);
                _types.Add(clrType, array);
                return array;
            }

            if (clrType.IsClass)
            {
                var objectType = new TypeDescriptor(clrType);
                if (!_typeDescriptors.ContainsKey(clrType))
                {
                    _typeDescriptors.Add(clrType, new ObjectDescriptor(clrType, GetProperties(clrType, objectType)));
                }
                _types.Add(clrType, objectType);
                return objectType;
            }

            return null;
        }

        private Property[] GetProperties(Type type, TypeDescriptor owner)
        {
            // owner is here, to prevent stackoverflow for circular types
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Select(pi => ParseProperty(pi, owner))
                                                    .ToArray();
        }
    }
}
