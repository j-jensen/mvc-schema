using MvcSchema.Impl;
using System.Collections;
using System.Collections.Generic;

namespace MvcSchema
{
    public interface IMvcSchemaAnalyzer
    {
        IEnumerable<RouteInformation> GetSchema();
    }
}