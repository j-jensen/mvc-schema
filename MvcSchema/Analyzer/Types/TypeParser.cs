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
        private readonly Dictionary<string, TypeDescriptor> _typeDescriptors;
        public TypeParser()
        {
            _typeDescriptors = new Dictionary<string, TypeDescriptor>
            {
                { typeof(object).GetID(), new TypeDescriptor(typeof(object)) },
                { typeof(void).GetID(), new TypeDescriptor(typeof(void)) },

                // String is special - We want it to be a type string, not a type char[]
                { typeof(string).GetID(), new TypeDescriptor(typeof(string)) }
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

        public TypeDescriptor ParseType(Type clrType, params string[] stack)
        {
            var ID = clrType.GetID();
            if (stack.Contains(ID))
            {
                return new Placeholder(ID);
            }
            stack = stack.Concat(new[] { ID }).ToArray();

            // Do we allready have the type?
            if (_typeDescriptors.TryGetValue(ID, out TypeDescriptor type))
            {
                return type;
            }

            // Numbers, Char and bool
            if (clrType.IsPrimitive)
            {
                TypeDescriptor primitiveType = new TypeDescriptor(clrType);
                _typeDescriptors.Add(ID, primitiveType);
                return primitiveType;
            }

            // Nullable types
            Type underlyingClrType = Nullable.GetUnderlyingType(clrType);
            if (underlyingClrType != null)
            {
                if (!_typeDescriptors.TryGetValue(underlyingClrType.GetID(), out TypeDescriptor underlyingType))
                {
                    underlyingType = ParseType(underlyingClrType, stack);
                }
                var nullable = new TypeDescriptor($"{underlyingType.ID}?", underlyingClrType, underlyingType.DataType, Kind.Nullable);
                _typeDescriptors.Add(ID, nullable);
                return nullable;
            }

            // Enums
            if (clrType.IsEnum)
            {
                var enumType = new EnumDescriptor(clrType);
                _typeDescriptors.Add(ID, enumType);
                return enumType;
            }

            // Array
            if (clrType.IsArray)
            {
                Type arrayType = clrType.GetElementType();
                return ParseArrayType(arrayType, stack);
            }

            // Array like
            Type collectionInterface = clrType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (collectionInterface != null || clrType.Name == "IEnumerable`1")
            {
                Type arrayType = (collectionInterface ?? clrType).GetGenericArguments().First();
                return ParseArrayType(arrayType, stack);
            }

            // Async
            Type taskInterface = clrType.GetInterfaces()
                .FirstOrDefault(i => i.UnderlyingSystemType == typeof(IAsyncResult));
            if (taskInterface != null)
            {
                if (clrType == typeof(Task))
                {
                    return ParseType(typeof(void), stack);
                }
                PropertyInfo resultPI = clrType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
                Type objType = resultPI.PropertyType;
                return ParseType(objType, stack);
            }

            // ActionResult<T> - AspNetCore 3
            if (clrType.Name == "ActionResult`1" && clrType.Namespace == "Microsoft.AspNetCore.Mvc")
            {
                PropertyInfo valuePI = clrType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                return ParseType(valuePI.PropertyType, stack);
            }
            // Derived from ActionResult MVC
            if (clrType == typeof(ActionResult) || clrType.IsSubclassOf(typeof(ActionResult)))
            {
                if (clrType == typeof(ViewResult))
                {
                    return new TypeDescriptor("ViewString", typeof(string), DataType.String, Kind.None);
                }
                if (clrType == typeof(JsonResult))
                {
                    return new TypeDescriptor("JsonString", typeof(string), DataType.String, Kind.None);
                }
                return ParseType(typeof(string), stack);
            }

            if (clrType.BaseType == typeof(ValueType))
            {
                var valueType = new TypeDescriptor(ID, clrType, DataType.String, Kind.None);
                _typeDescriptors.Add(ID, valueType);
                return valueType;
            }

            if (clrType.IsClass)
            {
                var objectType = new ObjectDescriptor(clrType, GetProperties(clrType, stack));
                _typeDescriptors.Add(ID, objectType);

                for (int i = 0; i < objectType.Properties.Length; i++)
                {
                    if (objectType.Properties[i].Type is Placeholder)
                    {
                        if(_typeDescriptors.TryGetValue(objectType.Properties[i].Type.ID, out TypeDescriptor td))
                        {
                            objectType.Properties[i].Type = td;
                        }
                    }
                }
                return objectType;
            }

            return new Placeholder(ID)
            {
                Description = $"Couldn't parse type: {ID}. Stack: {string.Join('/', stack)}"
            };
        }

        private TypeDescriptor ParseArrayType(Type arrayType, string[] stack)
        {
            var ID = $"{arrayType.GetID()}[]";
            if (_typeDescriptors.TryGetValue(ID, out var typeDescriptor))
            {
                return typeDescriptor;
            }
            if (!_typeDescriptors.TryGetValue(arrayType.GetID(), out TypeDescriptor underlyingType))
            {
                underlyingType = ParseType(arrayType, stack);
            }
            var array = new TypeDescriptor(ID,arrayType,underlyingType.DataType, Kind.Array);
            _typeDescriptors.Add(ID, array);
            return array;
        }

        private Property[] GetProperties(Type type, string[] stack)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Select(pi => ParseProperty(pi, stack))
                                                    .ToArray();
        }
        public Property ParseProperty(PropertyInfo pi, string[] stack)
        {
            return new Property
            {
                Name = pi.Name,
                Type = ParseType(pi.PropertyType, stack)
            };
        }
    }
}
