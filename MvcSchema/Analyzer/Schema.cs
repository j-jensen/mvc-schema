using MvcSchema.Analyzer.Types;

namespace MvcSchema.Analyzer
{
    public class Schema
    {
        public ActionDescriptor[] Actions { get; set; }

        public TypeDescriptor[] Types { get; set; }
    }
}
