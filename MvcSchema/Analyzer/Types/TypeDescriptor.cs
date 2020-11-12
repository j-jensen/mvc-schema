
using System;
using System.Text.Json.Serialization;

namespace MvcSchema.Analyzer.Types
{
    public class TypeDescriptor
    {
        public TypeDescriptor(Type type, Kind kind = Kind.None)
        {
            Name = $"{type.Namespace}.{type.Name}";
            Kind = kind;
            ClrType = type;
            if (type.IsPrimitive)
            {
                if (type == typeof(bool))
                    JsType = DataType.Boolean;
                else if (type == typeof(char))
                    JsType = DataType.String;
                else
                    JsType = DataType.Number;
            }
            else if (type == typeof(string))
            {
                JsType = DataType.String;
            }
            else
            {
                JsType = DataType.Object;
            }
        }

        public string Name { get; private set; }
        [JsonIgnore]
        public Type ClrType { get; private set; }
        public DataType JsType { get; private set; }
        public Kind Kind { get; private set; }
    }
}
