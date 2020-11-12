using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer.Types
{
    public class EnumDescriptor : TypeDescriptor
    {
        public EnumDescriptor(Type type) : base(type, Kind.Enum)
        {
            Values = Enum.GetNames(type);
        }

        public string[] Values { get; private set; }
    }
}
