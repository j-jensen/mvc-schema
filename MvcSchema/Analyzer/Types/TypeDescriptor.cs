
using System;
using System.Text.Json.Serialization;

namespace MvcSchema.Analyzer.Types
{
    public class TypeDescriptor
    {
        public TypeDescriptor(string typeName, Type type, DataType datatype, Kind kind = Kind.None)
        {
            TypeName = typeName;
            Kind = kind;
            DataType = datatype;
        }
        public TypeDescriptor(Type clrType, Kind kind = Kind.None)
        {
            TypeName = clrType.GetNamespacedName();
            Kind = kind;
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
        /// Typename.
        /// </summary>
        public string TypeName { get; private set; }
        /// <summary>
        /// Javascript datatype
        /// </summary>
        public DataType DataType { get; private set; }
        /// <summary>
        /// Type capability
        /// </summary>
        public Kind Kind { get; private set; }
    }
}
