using MvcSchema.Analyzer.Types;
using System.Text.Json;

namespace MvcSchema.Analyzer
{
    public class RouteInformation
    {
        public string HttpMethod { get; set; } = "GET";
        public string Area { get; set; } = "";
        public string Path { get; set; } = "";
        public string Invocation { get; set; } = "";
        public Argument[] Arguments { get; set; } = new Argument[0];

    }
}