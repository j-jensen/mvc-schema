﻿using MvcSchema.Analyzer.Types;

namespace MvcSchema.Analyzer
{
    public class ActionDescriptor
    {
        public string HttpMethod { get; set; } = "GET";
        public string Area { get; set; } = "";
        public string Path { get; set; } = "";
        public string Invocation { get; set; } = "";
        public Argument[] Arguments { get; set; } = new Argument[0];
        public TypeDescriptor ReturnType { get; internal set; }
    }
}