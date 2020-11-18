using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer.Types
{
    public static class TypeExtensions
    {
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
                name += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetID(typeParameters[i]);
                    name += (i == 0 ? typeParamName : "," + typeParamName);
                }
                name += ">";
            }

            return $"{type.Namespace}.{name}";
        }

    }
}
