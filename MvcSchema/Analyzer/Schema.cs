﻿using MvcSchema.Analyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSchema.Analyzer
{
    public class Schema
    {
        public RouteInformation[] Routes { get; set; }

        public TypeDescriptor[] Types { get; set; }
    }
}