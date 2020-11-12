using System;

namespace MvcSchema.Analyzer.Types
{
    public class ObjectDescriptor : TypeDescriptor
    {
        public ObjectDescriptor(Type type, Property[] properties) : base(type)
        {
            Properties = properties;
        }

        public Property[] Properties { get; private set; }
    }
}
