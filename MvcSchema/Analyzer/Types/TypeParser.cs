using Microsoft.AspNetCore.Mvc;
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
        private readonly Dictionary<Type, TypeDescriptor> _typeDescriptors;
        public TypeParser()
        {
            _typeDescriptors = new Dictionary<Type, TypeDescriptor>
            {
                { typeof(object), new TypeDescriptor(typeof(object)) },
                { typeof(void), new TypeDescriptor(typeof(void)) },

                // String is special - We want it to be a type string, not a type char[]
                { typeof(string), new TypeDescriptor(typeof(string)) }
            };
        }

        public Property ParseProperty(PropertyInfo pi, TypeDescriptor owner = null)
        {
            return new Property
            {
                Name = pi.Name,
                Type = owner?.ClrType == pi.PropertyType ? owner : ParseType(pi.PropertyType)
            };
        }

        public IEnumerable<TypeDescriptor> TypeDescriptors => _typeDescriptors.Select(td => td.Value);

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
            if (_typeDescriptors.TryGetValue(clrType, out TypeDescriptor type))
            {
                return type;
            }

            // Numbers, Char and bool
            if (clrType.IsPrimitive)
            {
                TypeDescriptor primitiveType = new TypeDescriptor(clrType);
                _typeDescriptors.Add(clrType, primitiveType);
                return primitiveType;
            }

            // Nullable types
            Type underlyingClrType = Nullable.GetUnderlyingType(clrType);
            if (underlyingClrType != null)
            {
                if (!_typeDescriptors.TryGetValue(underlyingClrType, out TypeDescriptor underlyingType))
                {
                    underlyingType = ParseType(underlyingClrType);
                }
                TypeDescriptor nullable = new TypeDescriptor(underlyingClrType, Kind.Nullable);
                _typeDescriptors.Add(clrType, nullable);
                return nullable;
            }

            // Enums
            if (clrType.IsEnum)
            {
                _typeDescriptors.Add(clrType, new EnumDescriptor(clrType));
                TypeDescriptor enumType = new TypeDescriptor(clrType, Kind.Enum);
                _typeDescriptors.Add(clrType, enumType);
                return enumType;
            }

            // Array
            if (clrType.IsArray)
            {
                Type arrayType = clrType.GetElementType();
                if (!_typeDescriptors.TryGetValue(arrayType, out TypeDescriptor underlyingType))
                {
                    underlyingType = ParseType(arrayType);
                }
                TypeDescriptor array = new TypeDescriptor(arrayType, Kind.Array);
                _typeDescriptors.Add(clrType, array);
                return array;
            }

            // Array like
            Type collectionInterface = clrType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (collectionInterface != null)
            {
                Type arrayType = collectionInterface.GetGenericArguments().First();
                if (!_typeDescriptors.TryGetValue(arrayType, out TypeDescriptor underlyingType))
                {
                    underlyingType = ParseType(arrayType);
                }
                TypeDescriptor array = new TypeDescriptor(arrayType, Kind.Array);
                _typeDescriptors.Add(clrType, array);
                return array;
            }

            // Async
            Type taskInterface = clrType.GetInterfaces()
                .FirstOrDefault(i => i.UnderlyingSystemType == typeof(IAsyncResult));
            if (taskInterface != null)
            {
                if (clrType == typeof(Task))
                {
                    return ParseType(typeof(void));
                }
                PropertyInfo resultPI = clrType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
                Type objType = resultPI.PropertyType;
                return ParseType(objType);
            }

            // ActionResult<T> - AspNetCore 3
            if (clrType.Name == "ActionResult`1" && clrType.Namespace == "Microsoft.AspNetCore.Mvc")
            {
                PropertyInfo valuePI = clrType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                return ParseType(valuePI.PropertyType);
            }
            // Derived from ActionResult MVC
            if (clrType.IsSubclassOf(typeof(ActionResult)))
            {
                if (clrType == typeof(ViewResult))
                {
                    return new TypeDescriptor("ViewString", typeof(string), DataType.String, Kind.None);
                }
                if (clrType == typeof(JsonResult))
                {
                    return new TypeDescriptor("JsonString", typeof(string), DataType.String, Kind.None);
                }
                return ParseType(typeof(string));
            }

            if (clrType.IsClass)
            {
                TypeDescriptor objectType = new TypeDescriptor(clrType);
                if (!_typeDescriptors.ContainsKey(clrType))
                {
                    _typeDescriptors.Add(clrType, new ObjectDescriptor(clrType, GetProperties(clrType, objectType)));
                }
                _typeDescriptors.Add(clrType, objectType);
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
