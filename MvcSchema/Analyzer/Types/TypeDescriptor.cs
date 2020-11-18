
using System;
using System.Text.Json.Serialization;

namespace MvcSchema.Analyzer.Types
{
    public class TypeDescriptor
    {
        public TypeDescriptor(string id, Type type, DataType datatype, Kind kind = Kind.None)
        {
            ClrType = type;
            ID = id;
            Kind = kind;
            DataType = datatype;
        }
        public TypeDescriptor(Type clrType, Kind kind = Kind.None)
        {
            ID = GetID(clrType);
            Kind = kind;
            ClrType = clrType;
            if (clrType.IsPrimitive)
            {
                if (clrType == typeof(bool))
                {
                    DataType = DataType.Boolean;
                }
                else if (clrType == typeof(char))
                {
                    DataType = DataType.String;
                }
                else
                {
                    DataType = DataType.Number;
                }
            }
            else if (clrType == typeof(string))
            {
                DataType = DataType.String;
            }
            else if (clrType == typeof(void))
            {
                DataType = DataType.Undefined;
            }
            else
            {
                DataType = DataType.Object;
            }
        }

        /// <summary>
        /// Unique id of type.
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// CLR type
        /// </summary>
        [JsonIgnore]
        public Type ClrType { get; private set; }
        /// <summary>
        /// Javascript datatype
        /// </summary>
        public DataType DataType { get; private set; }
        /// <summary>
        /// Type capability
        /// </summary>
        public Kind Kind { get; private set; }

        public static string GetID(Type type)
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
