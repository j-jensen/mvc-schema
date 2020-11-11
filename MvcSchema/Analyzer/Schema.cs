using MvcSchema.Analyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer
{
    public class Schema
    {
        public IEnumerable<RouteInformation> Routes { get; set; }

        public IEnumerable<IType> Types { get; set; }
    }
}
