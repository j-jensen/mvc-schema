using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer.Types

{
    interface ITypeConverter : IConverter
    {
        Type TypeToConvert { get; }
    }
}
