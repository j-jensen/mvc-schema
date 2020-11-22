
using System;
using System.Text.Json.Serialization;

namespace MvcSchema.Analyzer.Types
{
    public class TypeDescriptor
    {
        public TypeDescriptor(string typeName, Type type, DataType datatype)
        {
            TypeName = typeName;
            DataType = datatype;
        }
        public TypeDescriptor(Type clrType)
        {
            TypeName = clrType.GetNamespacedName();
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

        public string TypeName { get; private set; }
        public DataType DataType { get; private set; }
    }
}
