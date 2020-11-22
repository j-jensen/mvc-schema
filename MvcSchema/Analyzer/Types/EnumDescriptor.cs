using System;

namespace MvcSchema.Analyzer.Types
{
    public class EnumDescriptor : TypeDescriptor
    {
        public EnumDescriptor(Type type) : base(type)
        {
            Values = Enum.GetNames(type);
        }

        public string[] Values { get; private set; }
    }
}
