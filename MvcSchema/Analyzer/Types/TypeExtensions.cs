using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer.Types
{
    public static class TypeExtensions
    {
        public static string GetNamespacedName(this Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }
        public static string GetID(this Type type)
        {
            string name = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = name.IndexOf('`');
                if (iBacktick > 0)
                {
                    name = name.Remove(iBacktick);
                }
                name += "[";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetID(typeParameters[i]);
                    name += (i == 0 ? typeParamName : "," + typeParamName);
                }
                name += "]";
            }

            return $"{type.GetKind()}:{type.Namespace}.{name}";
        }

        public static Kind GetKind(this Type type)
        {
            if (type.IsEnum)
            {
                return Kind.Enum;
            }
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return Kind.Nullable;
            }
            if (type != typeof(string) && (type.IsArray || type.Name == "IEnumerable`1" || type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)) != null))
            {
                return Kind.Array;
            }

            return Kind.None;
        }


        public static Type GetSimplifiedType(this Type type)
        {
            if (type.Name == "IEnumerable`1")
            {
                return type.GetGenericArguments().First().MakeArrayType();
            }
            if (type != typeof(string))
            {
                var collectionInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (collectionInterface != null)
                {
                    return collectionInterface.GetGenericArguments().First().MakeArrayType();
                }
            }

            return type;
        }
    }
}
